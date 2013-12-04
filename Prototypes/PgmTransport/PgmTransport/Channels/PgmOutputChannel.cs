using System;
using System.ServiceModel;
using System.ServiceModel.Channels;
using Microsoft.ServiceModel.Samples;
using PgmTransport.Diagnostics;
using PgmTransport.Sockets;

namespace PgmTransport
{
   abstract class PgmOutputChannel : ChannelBase, IOutputChannel
   {
      protected PgmSender m_sender;
      protected MessageEncoder m_encoder;
      protected EndpointAddress m_remoteAddress;
      Uri m_via;
      protected BufferManager m_bufMngr;
      protected PgmTransportBindingElement m_bindingElement;

      public PgmOutputChannel(ChannelManagerBase factory, EndpointAddress remoteAddress, Uri via, MessageEncoder encoder, BufferManager bufMngr, PgmTransportBindingElement bindingElement)
         : base(factory)
      {
         m_sender = new PgmSender(remoteAddress.Uri.Host, remoteAddress.Uri.Port, bindingElement.DataMode);
         m_encoder = encoder;
         m_remoteAddress = remoteAddress;
         m_via = via;
         m_bufMngr = bufMngr;
         m_bindingElement = bindingElement;

         SetSenderOptions();

         DiagnosticsManager.Instance.AddWriter(m_sender);
      }

      private void SetSenderOptions()
      {
         m_sender.FecMode = m_bindingElement.FecMode;
         m_sender.SendRateKbps = (ulong)m_bindingElement.SendRate;
         m_sender.LateJoinPercentage = m_bindingElement.LateJoin;
         m_sender.SetMessageBoundary = m_bindingElement.SetMessageBoundary;
         m_sender.WindowIncrementPercentage = m_bindingElement.SenderWidnowAdvance;
         m_sender.MulticastTTL = m_bindingElement.MulticastTTL;
         m_sender.SenderInterface = m_bindingElement.SenderInterface;

         //m_sender.PhysicalSocket.SendBufferSize = (int)m_bindingElement.MaxReceivedMessageSize;
      }

      protected override void OnAbort()
      {
         if (this.State == CommunicationState.Opened)
         {
            m_sender.Close();
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
            m_sender.Close();
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
         m_sender.Open();
      }

      #region IOutputChannel Members

      public IAsyncResult BeginSend(Message message, TimeSpan timeout, AsyncCallback callback, object state)
      {
         this.Send(message, timeout);
         return new CompletedAsyncResult(callback, state);
      }

      public IAsyncResult BeginSend(Message message, AsyncCallback callback, object state)
      {
         this.Send(message);
         return new CompletedAsyncResult(callback, state);
      }

      public void EndSend(IAsyncResult result)
      {
         CompletedAsyncResult.End(result);
      }

      public EndpointAddress RemoteAddress
      {
         get 
         {
            return m_remoteAddress;
         }
      }

      public void Send(Message message, TimeSpan timeout)
      {
         OnSend(message, timeout);
      }

      public void Send(Message message)
      {
         Send(message, this.DefaultSendTimeout);
      }

      public Uri Via
      {
         get 
         {
            return null;
         }
      }

      #endregion

      protected virtual void OnSend(Message message, TimeSpan timeout)
      {
         m_remoteAddress.ApplyTo(message);
         ArraySegment<byte> buf = m_encoder.WriteMessage(message, (int)m_bindingElement.MaxReceivedMessageSize, m_bufMngr);
         m_sender.Send(buf, timeout);
      }
   }
}
