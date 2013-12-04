using System;
using System.Diagnostics;
using System.Net;
using System.Net.Sockets;
using System.Runtime.InteropServices;
using PgmTransport.Diagnostics;

namespace PgmTransport.Sockets
{
   public class PgmSender : SocketBase, IDiagnose
   {
      public PgmSender(string ip, int port, DataMode mode)
         : base(ip, port, mode)
      {
         SetDefaults();
      }

      private void SetDefaults()
      {
         FecMode = PgmConstants.DefaultFecMode;
         SendRateKbps = PgmConstants.SendRate;
         LateJoinPercentage = PgmDefines.SENDER_DEFAULT_LATE_JOINER_PERCENTAGE;
         MulticastTTL = PgmDefines.MAX_MCAST_TTL;
         WindowIncrementPercentage = PgmDefines.SENDER_DEFAULT_WINDOW_ADV_PERCENTAGE;
      }

      public override void Open()
      {
         IPEndPoint any = new IPEndPoint(IPAddress.Any, 0);
         m_socket.Bind(any);
         Configure();
         m_socket.Connect(m_multicastGroup);
      }

      protected override void Configure()
      {
         SetSendWindow();
         SetFec();
         SetLateJoin();
         SetWindowIncrement();
         SetMulticastTTL();
         SetSenderInterface();
      }

      private void SetSenderInterface()
      {
         if (string.IsNullOrEmpty(SenderInterface))
         {
            return;
         }

         byte[] ipBytes = IPAddress.Parse(SenderInterface).GetAddressBytes();
         m_socket.SetSocketOption(PgmDefines.PgmSocketOption, (SocketOptionName)RmOptionName.RM_SET_SEND_IF, ipBytes);        
      }

      private void SetSendWindow()
      {
         _RM_SEND_WINDOW sndWnd = new _RM_SEND_WINDOW();
         sndWnd.WindowSizeInMSecs = PgmDefines.SENDER_DEFAULT_WINDOW_SIZE_BYTES;
         sndWnd.RateKbitsPerSec = SendRateKbps;

         byte[] structBuf = PgmUtils.StructureToByteArray(sndWnd);
         m_socket.SetSocketOption(PgmDefines.PgmSocketOption, (SocketOptionName)RmOptionName.RM_RATE_WINDOW_SIZE, structBuf);        
      }

      private void SetLateJoin()
      {
         m_socket.SetSocketOption(PgmDefines.PgmSocketOption, (SocketOptionName)RmOptionName.RM_LATEJOIN, LateJoinPercentage);
      }

      private void SetFec()
      {
         _RM_FEC_INFO fec = new _RM_FEC_INFO();
         fec.FECGroupSize = PgmDefines.FEC_GROUP_SIZE;
         fec.FECBlockSize = PgmDefines.FEC_BLOCK_SIZE;

         switch (FecMode)
         {
            case FecMode.Disabled:
               return;
            case FecMode.ProActive:
               fec.FECProActivePackets = PgmDefines.FEC_PROACTIVE_PACKETS;
               break;
            case FecMode.OnDemand:
               fec.fFECOnDemandParityEnabled = 1;
               break;
            case FecMode.Both:
               fec.FECProActivePackets = PgmDefines.FEC_PROACTIVE_PACKETS;
               fec.fFECOnDemandParityEnabled = 1;
               break;
            default:
               return;
         }

         byte[] structBuf = PgmUtils.StructureToByteArray(fec);
         m_socket.SetSocketOption(PgmDefines.PgmSocketOption, (SocketOptionName)RmOptionName.RM_USE_FEC, structBuf);
      }

      public int Send(ArraySegment<byte> segment, TimeSpan timeout)
      {
         BeforeSend(segment.Count - segment.Offset, timeout);
         return m_socket.Send(segment.Array, segment.Offset, segment.Count, SocketFlags.None);
      }

      private void BeforeSend(int msgSize, TimeSpan timeout)
      {
         if (SetMessageBoundary)
         {
            SetMsgBoundary(msgSize);
         }

         m_socket.SendTimeout = (int)timeout.TotalMilliseconds;
      }

      public IAsyncResult BeginSend(TimeSpan timeout, ArraySegment<byte> buffer, AsyncCallback callback, object state)
      {
         BeforeSend(buffer.Count - buffer.Offset, timeout);
         return m_socket.BeginSend(buffer.Array, buffer.Offset, buffer.Count, SocketFlags.None, callback, state);
      }

      public int EndSend(IAsyncResult result, out SocketError errCode)
      {
         return m_socket.EndSend(result, out errCode);
      }

      private void SetMsgBoundary(int boundary)
      {
         m_socket.SetSocketOption(PgmDefines.PgmSocketOption, (SocketOptionName)RmOptionName.RM_SET_MESSAGE_BOUNDARY, boundary);
      }

      private void SetWindowIncrement()
      {
         m_socket.SetSocketOption(PgmDefines.PgmSocketOption, (SocketOptionName)RmOptionName.RM_SEND_WINDOW_ADV_RATE, WindowIncrementPercentage);
      }

      private void SetMulticastTTL()
      {
         m_socket.SetSocketOption(PgmDefines.PgmSocketOption, (SocketOptionName)RmOptionName.RM_SET_MCAST_TTL, MulticastTTL);
      }

      public FecMode FecMode { get; set; }
      public ulong SendRateKbps { get; set; }
      public bool SetMessageBoundary { get; set; }
      public int LateJoinPercentage { get; set; }
      public int WindowIncrementPercentage { get; set; }
      public int MulticastTTL { get; set; }
      public string SenderInterface { get; set; }

      public override void Close()
      {
         m_socket.Shutdown(SocketShutdown.Both);
         m_socket.Close();
      }

      #region IDiagnose Members

      public object GetStatistics()
      {
         try
         {
            byte[] buf = new byte[Marshal.SizeOf(typeof(_RM_SENDER_STATS))];
            m_socket.GetSocketOption(PgmDefines.PgmSocketOption, (SocketOptionName)RmOptionName.RM_SENDER_STATISTICS, buf);

            return PgmUtils.ByteArrayToStructure<_RM_SENDER_STATS>(buf);
         }
         catch (Exception ex)
         {
            Debug.WriteLine(ex.Message);
            return null;
         }
      }
      
      public string ID
      {
         get 
         {
            return string.Format("Sender_{0}", m_socket.Handle.ToString());
         }
      }

      #endregion
   }
}
