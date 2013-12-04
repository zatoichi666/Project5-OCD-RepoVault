using System;
using System.Net;
using System.Net.Sockets;

namespace PgmTransport.Sockets
{
   public abstract class SocketBase
   {
      protected Socket m_socket;
      protected IPEndPoint m_multicastGroup;
      
      const int SO_MAX_MSG_SIZE = 0x2003;

      protected SocketBase(string ip, int port, DataMode mode)
      {
         PgmUtils.ValidateMulticastIP(ip);
         m_socket = CreateSocket((SocketType)(int)mode);
         m_multicastGroup = new IPEndPoint(IPAddress.Parse(ip), port);
         m_socket.SetSocketOption(SocketOptionLevel.Socket, SocketOptionName.ReuseAddress, true);
      }

      private Socket CreateSocket(SocketType mode)
      {
         if (mode != SocketType.Rdm && mode != SocketType.Stream)
         {
            throw new ArgumentException("PGM socket type must be set to SocketType.Rdm or SocketType.Stream", "mode");
         }

         return new Socket(AddressFamily.InterNetwork, mode, PgmDefines.PgmProtocolType);
      }

      public abstract void Open();
      public abstract void Close();
      protected abstract void Configure();

      public Socket PhysicalSocket 
      {
         get
         {
            return m_socket;
         }
      }

      public int ProtocolMaxMessageSize
      {
         get
         {
            return (int)m_socket.GetSocketOption(SocketOptionLevel.Socket, (SocketOptionName)SO_MAX_MSG_SIZE);
         }
      }
   }
}
