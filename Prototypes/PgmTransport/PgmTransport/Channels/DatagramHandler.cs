using System;
using System.ServiceModel.Channels;
using PgmTransport.Sockets;

namespace PgmTransport.Channels
{
   internal class DatagramHandler : DataHandler
   {
      FrameReceivedEventHandler m_callback;
      
      internal DatagramHandler(PgmReceiver receiver, BufferManager bufMngr, FrameReceivedEventHandler callback)
         : base(receiver, bufMngr)
      {
         m_callback = callback;
      }

      protected override void DataReceived(ArraySegment<byte> data)
      {
         m_callback(data);
      }
   }
}
