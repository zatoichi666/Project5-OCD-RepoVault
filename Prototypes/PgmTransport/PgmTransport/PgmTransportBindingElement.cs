using System;
using System.ServiceModel.Channels;
using System.ServiceModel.Description;
using PgmTransport.Configuration;
using PgmTransport.Sockets;


namespace PgmTransport
{
   class PgmTransportBindingElement : TransportBindingElement, IWsdlExportExtension, IPolicyExportExtension
   {
      public PgmTransportBindingElement()
      {
      }

      public PgmTransportBindingElement(PgmTransportBindingElement other)
         : base(other)
      {
         this.FecMode = other.FecMode;
         this.SendRate = other.SendRate;
         this.LateJoin = other.LateJoin;
         this.SetMessageBoundary = other.SetMessageBoundary;
         this.DataMode = other.DataMode;
         this.SenderWidnowAdvance = other.SenderWidnowAdvance;
         this.MulticastTTL = other.MulticastTTL;
         this.SenderInterface = other.SenderInterface;
      }

      public override string Scheme
      {
         get 
         {
            return PgmConstants.Scheme;
         }
      }

      public FecMode FecMode { get; set; }
      public long SendRate { get; set; }
      public int LateJoin { get; set; }
      public bool SetMessageBoundary { get; set; }
      public DataMode DataMode { get; set; }
      public int SenderWidnowAdvance { get; set; }
      public string SenderInterface { get; set; }
      public int MulticastTTL { get; set; }

      public override T GetProperty<T>(BindingContext context)
      {
         if (context == null)
         {
            throw new ArgumentNullException("context");
         }

         return context.GetInnerProperty<T>();
      }

      public override IChannelFactory<TChannel> BuildChannelFactory<TChannel>(BindingContext context)
      {
         if (typeof(TChannel) == typeof(IOutputChannel))
         {
            return (IChannelFactory<TChannel>)(object)new PgmChannelFactory(this, context);
         }
         else if (typeof(TChannel) == typeof(IOutputSessionChannel))
         {
            if (DataMode == DataMode.Stream)
            {
               throw new InvalidOperationException("Sessionful contracts cannot use Stream mode. Either configure dataMode to Message or change the contract to non sessionful");
            }

            return (IChannelFactory<TChannel>)(object)new PgmSessionChannelFactory(this, context);
         }
         else
         {
            throw new ArgumentException("Unsupported channel type " + typeof(TChannel).Name);
         }
      }

      public override IChannelListener<TChannel> BuildChannelListener<TChannel>(BindingContext context)
      {
         if (typeof(TChannel) == typeof(IInputChannel))
         {
            return (IChannelListener<TChannel>)(object)new PgmChannelListener(this, context);
         }
         else if (typeof(TChannel) == typeof(IInputSessionChannel))
         {
            if (DataMode == DataMode.Stream)
            {
               throw new InvalidOperationException("Sessionful contracts cannot use Stream mode. Either configure dataMode to Message or change the contract to non sessionful");
            }

            return (IChannelListener<TChannel>)(object)new PgmSessionChannelListener(this, context);
         }
         else
         {
            throw new ArgumentException("Unsupported channel type " + typeof(TChannel).Name);
         }
      }

      public override bool CanBuildChannelListener<TChannel>(BindingContext context)
      {
         return typeof(TChannel) == typeof(IInputChannel) ||
                typeof(TChannel) == typeof(IInputSessionChannel);
      }

      public override bool CanBuildChannelFactory<TChannel>(BindingContext context)
      {
         return typeof(TChannel) == typeof(IOutputChannel) ||
                typeof(TChannel) == typeof(IOutputSessionChannel);
      }

      public override BindingElement Clone()
      {
         return new PgmTransportBindingElement(this);
      }

      #region IWsdlExportExtension Members

      public void ExportContract(WsdlExporter exporter, WsdlContractConversionContext context)
      {
         
      }

      public void ExportEndpoint(WsdlExporter exporter, WsdlEndpointConversionContext context)
      {
         Wsdl.ExportEndpoint(exporter, context);
      }

      #endregion

      #region IPolicyExportExtension Members

      public void ExportPolicy(MetadataExporter exporter, PolicyConversionContext context)
      {
         Wsdl.ExportPolicy(exporter, context);
      }

      #endregion
   }
}
