using System;
using System.ServiceModel;
using System.ServiceModel.Channels;
using Microsoft.ServiceModel.Samples;
using PgmTransport.Channels;
using PgmTransport.Diagnostics;
using PgmTransport.Sockets;

namespace PgmTransport
{
   abstract class PgmInputChannel : ChannelBase, IInputChannel
   {
      InputQueue<Message> m_messageQueue;
      MessageEncoder m_encoder;
      PgmTransportBindingElement m_bindingElement;
      RemoteEndpointMessageHandler m_propHandler;
      BufferManager m_bufMngr;
      
      protected DataHandler m_channelHandler;
        
      protected PgmInputChannel(ChannelListenerBase listener, PgmReceiver receiver, BufferManager mngr, MessageEncoder encoder, PgmTransportBindingElement bindingElement)
         : base(listener)
      {
         m_messageQueue = new InputQueue<Message>();
         m_encoder = encoder;
         m_bufMngr = mngr;
         m_bindingElement = bindingElement;
         m_propHandler = new RemoteEndpointMessageHandler(receiver.PhysicalSocket.RemoteEndPoint);
         //receiver.PhysicalSocket.ReceiveBufferSize = (int)m_bindingElement.MaxReceivedMessageSize;
         
         DiagnosticsManager.Instance.AddWriter(receiver);
      }

      protected override void OnAbort()
      {
         if (State == CommunicationState.Opened)
         {
            m_channelHandler.Close();
            m_messageQueue.Shutdown();
         }
      }

      protected void Dispatch(ArraySegment<byte> ready)
      {
         lock (ThisLock)
         {
            Message message = m_encoder.ReadMessage(ready, m_bufMngr);
            m_propHandler.ApplyTo(message);
            m_messageQueue.EnqueueAndDispatch(message); 
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
         if (State == CommunicationState.Opened)
         {
            m_channelHandler.Close();
            m_messageQueue.Shutdown();
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
         m_channelHandler.Closed += new EventHandler(m_channelHandler_Closed);
         m_channelHandler.Open();
      }

      private void m_channelHandler_Closed(object sender, EventArgs e)
      {
         this.Close();
      }

      #region IInputChannel Members

      public IAsyncResult BeginReceive(TimeSpan timeout, AsyncCallback callback, object state)
      {
         return this.BeginTryReceive(timeout, callback, state);
      }

      public IAsyncResult BeginReceive(AsyncCallback callback, object state)
      {
         return this.BeginTryReceive(this.DefaultReceiveTimeout, callback, state);
      }

      public IAsyncResult BeginTryReceive(TimeSpan timeout, AsyncCallback callback, object state)
      {
         return m_messageQueue.BeginDequeue(timeout, callback, state);
      }

      public IAsyncResult BeginWaitForMessage(TimeSpan timeout, AsyncCallback callback, object state)
      {
         return m_messageQueue.BeginWaitForItem(timeout, callback, state);
      }

      public Message EndReceive(IAsyncResult result)
      {
         return m_messageQueue.EndDequeue(result);
      }

      public bool EndTryReceive(IAsyncResult result, out Message message)
      {
         return m_messageQueue.EndDequeue(result, out message);
      }

      public bool EndWaitForMessage(IAsyncResult result)
      {
         return m_messageQueue.EndWaitForItem(result);
      }

      public EndpointAddress LocalAddress
      {
         get
         {
            return null;
         }
      }

      public Message Receive(TimeSpan timeout)
      {
         Message message;

         if (this.TryReceive(timeout, out message))
         {
            return message;
         }
         else
         {
            throw CreateReceiveTimedOutException(this, timeout);
         }
      }

      public Message Receive()
      {
         return this.Receive(this.DefaultReceiveTimeout);
      }

      public bool TryReceive(TimeSpan timeout, out Message message)
      {
         return m_messageQueue.Dequeue(timeout, out message);
      }

      public bool WaitForMessage(TimeSpan timeout)
      {
         return m_messageQueue.WaitForItem(timeout);
      }

      #endregion

      static TimeoutException CreateReceiveTimedOutException(IInputChannel channel, TimeSpan timeout)
      {
         if (channel.LocalAddress != null)
         {
            return new TimeoutException(
                string.Format("Receive on local address {0} timed out after {1}. The time allotted to this operation may have been a portion of a longer timeout.",
                channel.LocalAddress.Uri.AbsoluteUri, timeout));
         }
         else
         {
            return new TimeoutException(
                string.Format("Receive timed out after {0}. The time allotted to this operation may have been a portion of a longer timeout.",
                timeout));
         }
      } 
   }
}
