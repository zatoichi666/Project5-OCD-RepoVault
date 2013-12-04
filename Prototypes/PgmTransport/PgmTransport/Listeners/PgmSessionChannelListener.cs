using System;
using System.ServiceModel;
using System.ServiceModel.Channels;
using Microsoft.ServiceModel.Samples;
using PgmTransport.Sockets;

namespace PgmTransport
{
   class PgmSessionChannelListener : ChannelListenerBase<IInputSessionChannel>
   {
      BufferManager m_bufferManager;
      PgmListener m_listener;
      MessageEncoderFactory m_messageEncoderFactory;
      Uri m_uri;
      PgmTransportBindingElement m_bindingElement;
      
      internal PgmSessionChannelListener(PgmTransportBindingElement bindingElement, BindingContext context)
      {
         this.m_bufferManager = BufferManager.CreateBufferManager(bindingElement.MaxBufferPoolSize, (int)bindingElement.MaxReceivedMessageSize);
         MessageEncodingBindingElement messageEncoderBindingElement = context.BindingParameters.Remove<MessageEncodingBindingElement>();
         this.m_messageEncoderFactory = messageEncoderBindingElement.CreateMessageEncoderFactory();
         m_bindingElement = bindingElement;

         m_uri = new Uri(context.ListenUriBaseAddress, context.ListenUriRelativeAddress);
         this.m_listener = new PgmListener(m_uri.Host, m_uri.Port, m_bindingElement.DataMode);
      }

      protected override IInputSessionChannel OnAcceptChannel(TimeSpan timeout)
      {
         throw new NotImplementedException();
      }

      protected override IAsyncResult OnBeginAcceptChannel(TimeSpan timeout, AsyncCallback callback, object state)
      {
         return m_listener.BeginAccept(callback, state);
      }

      protected override IInputSessionChannel OnEndAcceptChannel(IAsyncResult result)
      {
         lock (ThisLock)
         {
            PgmReceiver client = m_listener.EndAccept(result);
            return new PgmInputSessionChannel(this, client, m_bufferManager, m_messageEncoderFactory.Encoder, m_bindingElement); 
         }
      }

      protected override IAsyncResult OnBeginWaitForChannel(TimeSpan timeout, AsyncCallback callback, object state)
      {
         throw new NotImplementedException();
      }

      protected override bool OnEndWaitForChannel(IAsyncResult result)
      {
         throw new NotImplementedException();
      }

      protected override bool OnWaitForChannel(TimeSpan timeout)
      {
         throw new NotImplementedException();
      }

      public override Uri Uri
      {
         get 
         { 
            return m_uri; 
         }
      }

      protected override void OnAbort()
      {
         if (this.State == CommunicationState.Opened)
         {
            m_listener.Close();
         }
      }

      protected override IAsyncResult OnBeginClose(TimeSpan timeout, AsyncCallback callback, object state)
      {
         this.OnClose(timeout);
         return new CompletedAsyncResult(callback, state);
      }

      protected override IAsyncResult OnBeginOpen(TimeSpan timeout, AsyncCallback callback, object state)
      {
         this.OnOpen(timeout);
         return new CompletedAsyncResult(callback, state);
      }

      protected override void OnClose(TimeSpan timeout)
      {
         if (this.State == CommunicationState.Opened)
         {
            m_listener.Close();
         }
      }

      protected override void OnEndClose(IAsyncResult result)
      {
         CompletedAsyncResult.End(result);
      }

      protected override void OnEndOpen(IAsyncResult result)
      {
         CompletedAsyncResult.End(result);
      }

      protected override void OnOpen(TimeSpan timeout)
      {
         m_listener.Open();
      }
   }
}
