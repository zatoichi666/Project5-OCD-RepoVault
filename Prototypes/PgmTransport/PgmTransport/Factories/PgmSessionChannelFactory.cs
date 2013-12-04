using System;
using System.Collections.ObjectModel;
using System.ServiceModel;
using System.ServiceModel.Channels;
using Microsoft.ServiceModel.Samples;

namespace PgmTransport
{
   class PgmSessionChannelFactory : ChannelFactoryBase<IOutputSessionChannel>
   {
      BufferManager m_bufferManager;
      MessageEncoderFactory m_messageEncoderFactory;
      PgmTransportBindingElement m_bindingElement;

      internal PgmSessionChannelFactory(PgmTransportBindingElement bindingElement, BindingContext context)
      {
         m_bufferManager = BufferManager.CreateBufferManager(bindingElement.MaxBufferPoolSize, int.MaxValue);
         m_bindingElement = bindingElement;
         Collection<MessageEncodingBindingElement> messageEncoderBindingElements
             = context.BindingParameters.FindAll<MessageEncodingBindingElement>();

         m_messageEncoderFactory = messageEncoderBindingElements[0].CreateMessageEncoderFactory();
      }

      protected override IOutputSessionChannel OnCreateChannel(EndpointAddress address, Uri via)
      {
         return new PgmOutputSessionChannel(this, address, via, m_messageEncoderFactory.Encoder, m_bufferManager, m_bindingElement);
      }

      protected override IAsyncResult OnBeginOpen(TimeSpan timeout, AsyncCallback callback, object state)
      {
         OnOpen(timeout);
         return new CompletedAsyncResult(callback, state);
      }

      protected override void OnEndOpen(IAsyncResult result)
      {
         CompletedAsyncResult.End(result);
      }

      protected override void OnOpen(TimeSpan timeout)
      {
      }
   }
}
