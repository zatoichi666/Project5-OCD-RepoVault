using System.ServiceModel.Channels;
using PgmTransport.Sockets;

namespace PgmTransport.Channels
{
   class PgmInputDatagramChannel : PgmInputChannel
   {
      public PgmInputDatagramChannel(ChannelListenerBase listener, PgmReceiver receiver, BufferManager mngr, MessageEncoder encoder, PgmTransportBindingElement bindingElement)
         : base(listener, receiver, mngr, encoder, bindingElement)
      {
         m_channelHandler = new DatagramHandler(receiver, mngr, Dispatch);
      }
   }
}
