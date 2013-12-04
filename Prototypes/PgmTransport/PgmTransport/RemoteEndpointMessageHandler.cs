using System.Net;
using System.ServiceModel.Channels;

namespace PgmTransport
{
   class RemoteEndpointMessageHandler
   {
      RemoteEndpointMessageProperty m_prop;

      public RemoteEndpointMessageHandler(EndPoint ep)
      {
         string[] parts = ep.ToString().Split(':');
         m_prop = new RemoteEndpointMessageProperty(parts[0], int.Parse(parts[1]));
      }

      public void ApplyTo(Message message)
      {
         message.Properties.Add(RemoteEndpointMessageProperty.Name, m_prop);
      }
   }
}
