using System;
using System.ServiceModel;
using System.ServiceModel.Channels;

namespace PgmTransport
{
   class PgmOutputSessionChannel : PgmOutputChannel, IOutputSessionChannel
   {
      OutputSession session = new OutputSession();

      public PgmOutputSessionChannel(ChannelManagerBase factory, EndpointAddress remoteAddress, Uri via, MessageEncoder encoder, BufferManager bugMngr, PgmTransportBindingElement bindingElement)
         : base(factory, remoteAddress, via, encoder, bugMngr, bindingElement)
      {
      }
      
      public IOutputSession Session
      {
         get 
         {
            return session; 
         }
      }

      private class OutputSession : IOutputSession, ISession
      {
         private string id = "pgm-session-" + Guid.NewGuid().ToString();

         public string Id
         {
            get 
            { 
               return this.id; 
            }
         }
      }
   }
}
