using System;
using System.Net.Sockets;
using System.ServiceModel.Channels;
using System.Threading;
using PgmTransport.Sockets;

namespace PgmTransport.Channels
{
   abstract class DataHandler
   {
      PgmReceiver m_receiver;
      byte[] m_buffer;
      volatile bool m_open;
      BufferManager m_bufMngr;
      AutoResetEvent m_recvEvent = new AutoResetEvent(false);

      public event EventHandler Closed;

      protected DataHandler(PgmReceiver receiver, BufferManager bufMngr)
      {
         m_receiver = receiver;
         m_bufMngr = bufMngr;
         m_buffer = m_bufMngr.TakeBuffer(receiver.PhysicalSocket.ReceiveBufferSize);
      }

      internal void Open()
      {
         if (!m_open)
         {
            m_open = true;
            Thread t = new Thread(StartReceive);
            t.IsBackground = true;
            t.Start();
         }
      }

      internal void Close()
      {
         if (m_open)
         {
            m_open = false;
            m_receiver.Close();
            m_bufMngr.ReturnBuffer(m_buffer);
         }
      }

      private void StartReceive(object state)
      {
         ArraySegment<byte> bufArray = new ArraySegment<byte>(m_buffer, 0, m_buffer.Length);
         
         while (m_open)
         {
            try
            {
               m_receiver.BeginReceive(bufArray, OnDataReceived, null);
               m_recvEvent.WaitOne();
               //int size = m_receiver.Receive(bufArray);
               //if (size > 0)
               //{
               //   DataReceived(new ArraySegment<byte>(m_buffer, 0, size));
               //}
            }
            catch (SocketException ex)
            {
               Console.WriteLine(ex.Message);
               HandleSocketError(ex.SocketErrorCode);
            }
            catch (ObjectDisposedException)
            {
               OnClosed();
            }
         }
      }

      private void OnDataReceived(IAsyncResult result)
      {
         SocketError err = SocketError.Success;
         int size = m_receiver.EndReceive(result, out err);
         if (err == SocketError.Success && size > 0)
         {
            DataReceived(new ArraySegment<byte>(m_buffer, 0, size));
         }
         m_recvEvent.Set();
      }

      private void OnClosed()
      {
         Close();
         Closed(this, EventArgs.Empty);
      }

      protected abstract void DataReceived(ArraySegment<byte> data);
      
      private void HandleSocketError(SocketError sockErr)
      {
         switch (sockErr)
         {
            case SocketError.Disconnecting:
               OnClosed();
               break;

            default:
               break;
         }
      }
   }
}
