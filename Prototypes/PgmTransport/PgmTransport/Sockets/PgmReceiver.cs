using System;
using System.Diagnostics;
using System.Net.Sockets;
using System.Runtime.InteropServices;
using PgmTransport.Diagnostics;

namespace PgmTransport.Sockets
{
   public class PgmReceiver : IDiagnose
   {
      Socket m_receiver;

      public PgmReceiver(Socket receiver)
      {
         m_receiver = receiver;
         Configure();
      }

      private void Configure()
      {
         SetHighSpeedNetwork();
      }

      private void SetHighSpeedNetwork()
      {
         try
         {
            m_receiver.SetSocketOption(PgmDefines.PgmSocketOption, (SocketOptionName)RmOptionName.RM_HIGH_SPEED_INTRANET_OPT, 1);
         }
         catch (SocketException ex)
         {
            Debug.WriteLine("Failed to set highspeed network : " + ex.Message);
         }
      }

      public int Receive(ArraySegment<byte> segment)
      {
         return m_receiver.Receive(segment.Array, segment.Offset, segment.Count, SocketFlags.None);
      }

      public IAsyncResult BeginReceive(ArraySegment<byte> segment, AsyncCallback callback, object state)
      {
         return m_receiver.BeginReceive(segment.Array, segment.Offset, segment.Count, SocketFlags.None, callback, state);
      }

      public int EndReceive(IAsyncResult result, out SocketError errorCode)
      {
         return m_receiver.EndReceive(result, out errorCode);
      }

      public void Close()
      {
         m_receiver.Shutdown(SocketShutdown.Both);
         m_receiver.Close();
      }

      public Socket PhysicalSocket 
      {
         get
         {
            return m_receiver;
         }
      }

      #region IDiagnose Members

      public object GetStatistics()
      {
         try
         {
            byte[] buf = new byte[Marshal.SizeOf(typeof(_RM_RECEIVER_STATS))];
            m_receiver.GetSocketOption(PgmDefines.PgmSocketOption, (SocketOptionName)RmOptionName.RM_RECEIVER_STATISTICS, buf);

            return PgmUtils.ByteArrayToStructure<_RM_RECEIVER_STATS>(buf);
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
            return string.Format("Receiver_{0}", m_receiver.Handle.ToString());
         }
      }

      #endregion
   }
}
