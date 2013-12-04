using System;
using System.ServiceModel.Channels;
using PgmTransport.Sockets;
using PgmTransport.Channels;

namespace PgmTransport
{
   class PgmInputSessionChannel : PgmInputChannel, IInputSessionChannel
   {
      InputSession session = new InputSession();

      public PgmInputSessionChannel(PgmSessionChannelListener listener, PgmReceiver client, BufferManager mngr, MessageEncoder encoder, PgmTransportBindingElement bindingElement)
         : base(listener, client, mngr, encoder, bindingElement)
      {
         m_channelHandler = new DatagramHandler(client, mngr, Dispatch);
      }

      #region ISessionChannel<IInputSession> Members

      public IInputSession Session
      {
         get 
         { 
            return session; 
         }
      }

      #endregion

      private class InputSession : IInputSession, ISession
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
