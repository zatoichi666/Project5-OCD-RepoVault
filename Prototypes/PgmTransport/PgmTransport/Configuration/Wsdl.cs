using System;
using System.Collections.Generic;
using System.IO;
using System.ServiceModel;
using System.ServiceModel.Channels;
using System.ServiceModel.Description;
using System.Web.Services.Description;
using System.Xml;

namespace PgmTransport.Configuration
{
   class Wsdl
   {
      private static XmlDocument XmlDoc;

      static Wsdl()
      {
         NameTable nameTable = new NameTable();
         nameTable.Add("Policy");
         nameTable.Add("All");
         nameTable.Add("ExactlyOne");
         nameTable.Add("PolicyURIs");
         nameTable.Add("Id");
         nameTable.Add("UsingAddressing");
         nameTable.Add("UsingAddressing");
         XmlDoc = new XmlDocument(nameTable);
      }

      public static void ExportEndpoint(WsdlExporter exporter, WsdlEndpointConversionContext context)
      {
         BindingElementCollection bindingElements = context.Endpoint.Binding.CreateBindingElements();
         MessageEncodingBindingElement encodingBindingElement = bindingElements.Find<MessageEncodingBindingElement>();

         if (encodingBindingElement == null)
         {
            encodingBindingElement = new TextMessageEncodingBindingElement();
         }

         // Set SoapBinding Transport URI

         SoapBinding soapBinding = GetSoapBinding(context, exporter);

         if (soapBinding != null)
         {
            soapBinding.Transport = "http://sample.schemas.microsoft.com/policy/pgm";
         }

         if (context.WsdlPort != null)
         {
            AddAddressToWsdlPort(context.WsdlPort, context.Endpoint.Address, encodingBindingElement.MessageVersion.Addressing);
         }
      }

      public static void ExportPolicy(MetadataExporter exporter, PolicyConversionContext context)
      {
         if (exporter == null)
         {
            throw new ArgumentNullException("exporter");
         }

         if (context == null)
         {
            throw new ArgumentNullException("context");
         }

         ICollection<XmlElement> bindingAssertions = context.GetBindingAssertions();
         XmlDocument xmlDocument = new XmlDocument();
         bindingAssertions.Add(xmlDocument.CreateElement("pgm", "net.pgm", "http://sample.schemas.microsoft.com/policy/pgm"));

         bool createdNew = false;
         MessageEncodingBindingElement encodingBindingElement = context.BindingElements.Find<MessageEncodingBindingElement>();
         if (encodingBindingElement == null)
         {
            createdNew = true;
            encodingBindingElement = new TextMessageEncodingBindingElement();
         }

         if (createdNew && encodingBindingElement is IPolicyExportExtension)
         {
            ((IPolicyExportExtension)encodingBindingElement).ExportPolicy(exporter, context);
         }

         AddWSAddressingAssertion(context, encodingBindingElement.MessageVersion.Addressing);
      }

      private static void AddAddressToWsdlPort(Port wsdlPort, EndpointAddress endpointAddress, AddressingVersion addressing)
      {
         if (addressing == AddressingVersion.None)
         {
            return;
         }

         MemoryStream memoryStream = new MemoryStream();
         XmlWriter xmlWriter = XmlWriter.Create(memoryStream);
         xmlWriter.WriteStartElement("temp");

         if (addressing == AddressingVersion.WSAddressing10)
         {
            xmlWriter.WriteAttributeString("xmlns", "wsa10", null, "http://www.w3.org/2005/08/addressing");
         }
         else if (addressing == AddressingVersion.WSAddressingAugust2004)
         {
            xmlWriter.WriteAttributeString("xmlns", "wsa", null, "http://schemas.xmlsoap.org/ws/2004/08/addressing");
         }
         else
         {
            throw new InvalidOperationException("This addressing version is not supported:\n" + addressing.ToString());
         }

         endpointAddress.WriteTo(addressing, xmlWriter);
         xmlWriter.WriteEndElement();

         xmlWriter.Flush();
         memoryStream.Seek(0, SeekOrigin.Begin);

         XmlReader xmlReader = XmlReader.Create(memoryStream);
         xmlReader.MoveToContent();

         XmlElement endpointReference = (XmlElement)XmlDoc.ReadNode(xmlReader).ChildNodes[0];

         wsdlPort.Extensions.Add(endpointReference);
      }

      private static SoapBinding GetSoapBinding(WsdlEndpointConversionContext endpointContext, WsdlExporter exporter)
      {
         EnvelopeVersion envelopeVersion = null;
         SoapBinding existingSoapBinding = null;
         object versions = null;
         object SoapVersionStateKey = new object();

         //get the soap version state
         if (exporter.State.TryGetValue(SoapVersionStateKey, out versions))
         {
            if (versions != null && ((Dictionary<System.Web.Services.Description.Binding, EnvelopeVersion>)versions).ContainsKey(endpointContext.WsdlBinding))
            {
               envelopeVersion = ((Dictionary<System.Web.Services.Description.Binding, EnvelopeVersion>)versions)[endpointContext.WsdlBinding];
            }
         }

         if (envelopeVersion == EnvelopeVersion.None)
         {
            return null;
         }

         //get existing soap binding
         foreach (object o in endpointContext.WsdlBinding.Extensions)
         {
            if (o is SoapBinding)
            {
               existingSoapBinding = (SoapBinding)o;
            }
         }

         return existingSoapBinding;
      }

      private static void AddWSAddressingAssertion(PolicyConversionContext context, AddressingVersion addressing)
      {
         XmlElement addressingAssertion = null;

         if (addressing == AddressingVersion.WSAddressing10)
         {
            addressingAssertion = XmlDoc.CreateElement("wsaw", "UsingAddressing", "http://www.w3.org/2006/05/addressing/wsdl");
         }
         else if (addressing == AddressingVersion.WSAddressingAugust2004)
         {
            addressingAssertion = XmlDoc.CreateElement("wsap", "UsingAddressing", "http://schemas.xmlsoap.org/ws/2004/08/addressing" + "/policy");
         }
         else if (addressing == AddressingVersion.None)
         {
            // do nothing
            addressingAssertion = null;
         }
         else
         {
            throw new InvalidOperationException("This addressing version is not supported:\n" + addressing.ToString());
         }

         if (addressingAssertion != null)
         {
            context.GetBindingAssertions().Add(addressingAssertion);
         }
      }
   }
}
