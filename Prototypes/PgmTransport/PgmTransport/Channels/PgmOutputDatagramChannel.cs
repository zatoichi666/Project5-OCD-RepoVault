using System;
using System.ServiceModel;
using System.ServiceModel.Channels;

namespace PgmTransport.Channels
{
   class PgmOutputDatagramChannel : PgmOutputChannel
   {
      public PgmOutputDatagramChannel(ChannelManagerBase factory, EndpointAddress remoteAddress, Uri via, MessageEncoder encoder, BufferManager bufMngr, PgmTransportBindingElement bindingElement)
         : base(factory, remoteAddress, via, encoder, bufMngr, bindingElement)
      {
      }
   }
}
