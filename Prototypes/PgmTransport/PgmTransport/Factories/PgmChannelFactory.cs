using System;
using System.Collections.ObjectModel;
using System.ServiceModel;
using System.ServiceModel.Channels;
using Microsoft.ServiceModel.Samples;
using PgmTransport.Channels;
using PgmTransport.Sockets;

namespace PgmTransport
{
   class PgmChannelFactory : ChannelFactoryBase<IOutputChannel>
   {
      BufferManager m_bufferManager;
      MessageEncoderFactory m_messageEncoderFactory;
      PgmTransportBindingElement m_bindingElement;

      internal PgmChannelFactory(PgmTransportBindingElement bindingElement, BindingContext context)
      {
         m_bufferManager = BufferManager.CreateBufferManager(bindingElement.MaxBufferPoolSize, int.MaxValue);
         m_bindingElement = bindingElement;
         Collection<MessageEncodingBindingElement> messageEncoderBindingElements
             = context.BindingParameters.FindAll<MessageEncodingBindingElement>();

         m_messageEncoderFactory = messageEncoderBindingElements[0].CreateMessageEncoderFactory();
      }

      protected override IOutputChannel OnCreateChannel(EndpointAddress address, Uri via)
      {
         if (m_bindingElement.DataMode == DataMode.Message)
         {
            return new PgmOutputDatagramChannel(this, address, via, m_messageEncoderFactory.Encoder, m_bufferManager, m_bindingElement);
         }

         throw new NotSupportedException("Streaming data mode is not supported");
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
