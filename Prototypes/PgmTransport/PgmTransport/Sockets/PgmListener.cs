using System;
using System.Diagnostics;
using System.Net;
using System.Net.Sockets;

namespace PgmTransport.Sockets
{
   public class PgmListener : SocketBase
   {
      public PgmListener(string ip, int port, DataMode mode)
         : base(ip, port, mode)
      {
      }

      public override void Open()
      {
         m_socket.Bind(m_multicastGroup);
         //Configure();        
         m_socket.Listen(10);   
      }

      protected override void Configure()
      {
         SetHighSpeedNetwork(); 
      }

      private void SetHighSpeedNetwork()
      {
         try
         {
            PhysicalSocket.SetSocketOption(PgmDefines.PgmSocketOption, (SocketOptionName)RmOptionName.RM_HIGH_SPEED_INTRANET_OPT, 1);
         }
         catch (SocketException ex)
         {
            Debug.WriteLine("Failed to set highspeed network : " + ex.Message);
         }
      }

      public void AddReceiverInterface(string ip)
      {
         byte[] ipbytes = IPAddress.Parse(ip).GetAddressBytes();
         m_socket.SetSocketOption(PgmDefines.PgmSocketOption, (SocketOptionName)RmOptionName.RM_ADD_RECEIVE_IF, ipbytes);
      }

      public void DeleteReceiverInterface(string ip)
      {
         byte[] ipbytes = IPAddress.Parse(ip).GetAddressBytes();
         m_socket.SetSocketOption(PgmDefines.PgmSocketOption, (SocketOptionName)RmOptionName.RM_DEL_RECEIVE_IF, ipbytes);
      }

      public PgmReceiver Accept()
      {
         Socket receiver = m_socket.Accept();
         return new PgmReceiver(receiver);
      }

      public IAsyncResult BeginAccept(AsyncCallback callback, object state)
      {
         return m_socket.BeginAccept(callback, state);
      }

      public PgmReceiver EndAccept(IAsyncResult result)
      {
         Socket receiver = m_socket.EndAccept(result);
         return new PgmReceiver(receiver);
      }

      public override void Close()
      {
         m_socket.Close();
      }
   }
}
