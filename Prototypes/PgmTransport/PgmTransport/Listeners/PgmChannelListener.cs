using System;
using System.ServiceModel;
using System.ServiceModel.Channels;
using Microsoft.ServiceModel.Samples;
using PgmTransport.Channels;
using PgmTransport.Sockets;

namespace PgmTransport
{
   class PgmChannelListener : ChannelListenerBase<IInputChannel>
   {
      BufferManager m_bufferManager;
      PgmListener m_listener;
      MessageEncoderFactory m_messageEncoderFactory;   
      Uri m_uri;
      PgmTransportBindingElement m_bindingElement;
      
      internal PgmChannelListener(PgmTransportBindingElement bindingElement, BindingContext context)
      {
         this.m_bufferManager = BufferManager.CreateBufferManager(bindingElement.MaxBufferPoolSize, (int)bindingElement.MaxReceivedMessageSize);
         MessageEncodingBindingElement messageEncoderBindingElement = context.BindingParameters.Remove<MessageEncodingBindingElement>();
         this.m_messageEncoderFactory = messageEncoderBindingElement.CreateMessageEncoderFactory();
         this.m_bindingElement = bindingElement;
         m_uri = new Uri(context.ListenUriBaseAddress, context.ListenUriRelativeAddress);
         this.m_listener = new PgmListener(m_uri.Host, m_uri.Port, m_bindingElement.DataMode);
      }

      protected override IInputChannel OnAcceptChannel(TimeSpan timeout)
      {
         throw new NotImplementedException();
      }

      protected override IAsyncResult OnBeginAcceptChannel(TimeSpan timeout, AsyncCallback callback, object state)
      {
         return m_listener.BeginAccept(callback, state);
      }

      protected override IInputChannel OnEndAcceptChannel(IAsyncResult result)
      {
         lock (ThisLock)
         {
            PgmReceiver client = m_listener.EndAccept(result);
            if (m_bindingElement.DataMode == DataMode.Message)
            {
               return new PgmInputDatagramChannel(this, client, m_bufferManager, m_messageEncoderFactory.Encoder, m_bindingElement);
            }

            throw new NotSupportedException("Streaming data mode is not supported");
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
