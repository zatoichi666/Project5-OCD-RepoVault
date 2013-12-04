using System;
using System.Configuration;
using System.ServiceModel.Channels;
using System.ServiceModel.Configuration;
using PgmTransport.Diagnostics;
using PgmTransport.Sockets;

namespace PgmTransport.Configuration
{
   class PgmTransportElement : BindingElementExtensionElement
   {
      [ConfigurationProperty(ConfigOptions.MaxBufferPoolSize, DefaultValue = PgmConstants.MaxBufferPoolSize)]
      [LongValidator(MinValue = 0)]
      public long MaxBufferPoolSize
      {
         get 
         {
            return (long)base[ConfigOptions.MaxBufferPoolSize]; 
         }
         set 
         {
            base[ConfigOptions.MaxBufferPoolSize] = value; 
         }
      }

      [ConfigurationProperty(ConfigOptions.MaxReceivedMessageSize, DefaultValue = PgmConstants.MaxReceivedMessageSize)]
      [IntegerValidator(MinValue = 1)]
      public int MaxReceivedMessageSize
      {
         get 
         {
            return (int)base[ConfigOptions.MaxReceivedMessageSize]; 
         }
         set 
         {
            base[ConfigOptions.MaxReceivedMessageSize] = value; 
         }
      }

      [ConfigurationProperty(ConfigOptions.ForwardErrorCorrection, DefaultValue = PgmConstants.DefaultFecMode)]
      public FecMode FecMode
      {
         get
         {
            return (FecMode)base[ConfigOptions.ForwardErrorCorrection];
         }
         set
         {
            base[ConfigOptions.ForwardErrorCorrection] = value;
         }
      }

      [ConfigurationProperty(ConfigOptions.SendRate, DefaultValue = PgmConstants.SendRate)]
      [LongValidator(MinValue = 56, MaxValue = 15 * 1024)]
      public long SendRate
      {
         get
         {
            return (long)base[ConfigOptions.SendRate];
         }
         set
         {
            base[ConfigOptions.SendRate] = value;
         }
      }

      [ConfigurationProperty(ConfigOptions.LateJoin, DefaultValue = PgmConstants.LateJoin)]
      [IntegerValidator(MinValue = 1, MaxValue = 75)]
      public int LateJoin
      {
         get
         {
            return (int)base[ConfigOptions.LateJoin];
         }
         set
         {
            base[ConfigOptions.LateJoin] = value;
         }
      }

      [ConfigurationProperty(ConfigOptions.SetMmgBoundary, DefaultValue = PgmConstants.SetMmgBoundary)]
      public bool SetMessageBoundary
      {
         get
         {
            return (bool)base[ConfigOptions.SetMmgBoundary];
         }
         set
         {
            base[ConfigOptions.SetMmgBoundary] = value;
         }
      }

      [ConfigurationProperty(ConfigOptions.DataMode, DefaultValue = PgmConstants.DefaultDataMode)]
      public DataMode DataMode
      {
         get
         {
            return (DataMode)base[ConfigOptions.DataMode];
         }
         set
         {
            base[ConfigOptions.DataMode] = value;
         }
      }

      [ConfigurationProperty(ConfigOptions.SenderWidnowAdvance, DefaultValue = PgmDefines.SENDER_DEFAULT_WINDOW_ADV_PERCENTAGE)]
      [IntegerValidator(MinValue = 1, MaxValue = 25)]
      public int SenderWidnowAdvance
      {
         get
         {
            return (int)base[ConfigOptions.SenderWidnowAdvance];
         }
         set
         {
            base[ConfigOptions.SenderWidnowAdvance] = value;
         }
      }

      [ConfigurationProperty(ConfigOptions.MulticastTTL, DefaultValue = PgmDefines.MAX_MCAST_TTL)]
      [IntegerValidator(MinValue = 1, MaxValue = 255)]
      public int MulticastTTL
      {
         get
         {
            return (int)base[ConfigOptions.MulticastTTL];
         }
         set
         {
            base[ConfigOptions.MulticastTTL] = value;
         }
      }

      [ConfigurationProperty(ConfigOptions.SenderInterface)]
      [IpValidator(PgmUtils.IPValidatorString)]
      public string SenderInterface
      {
         get
         {
            return (string)base[ConfigOptions.SenderInterface];
         }
         set
         {
            base[ConfigOptions.SenderInterface] = value;
         }
      }

      [ConfigurationProperty(ConfigOptions.Diagnostics, IsRequired = false)]
      public DiagnosticsConfiguration Diagnostics
      {
         get
         {
            return (DiagnosticsConfiguration)base[ConfigOptions.Diagnostics];
         }
         set
         {
            base[ConfigOptions.Diagnostics] = value;
         }
      }

      public override Type BindingElementType
      {
         get
         { 
            return typeof(PgmTransportBindingElement);
         }
      }

      protected override BindingElement CreateBindingElement()
      {
         PgmTransportBindingElement bindingElement = new PgmTransportBindingElement();
         this.ApplyConfiguration(bindingElement);
         DiagnosticsManager.Instance.Configure(Diagnostics);

         return bindingElement;
      }

      public override void ApplyConfiguration(BindingElement bindingElement)
      {
         base.ApplyConfiguration(bindingElement);

         PgmTransportBindingElement pgmBindingElement = (PgmTransportBindingElement)bindingElement;
         pgmBindingElement.MaxBufferPoolSize = this.MaxBufferPoolSize;
         pgmBindingElement.MaxReceivedMessageSize = this.MaxReceivedMessageSize;
         pgmBindingElement.FecMode = this.FecMode;
         pgmBindingElement.SendRate = this.SendRate;
         pgmBindingElement.LateJoin = this.LateJoin;
         pgmBindingElement.SetMessageBoundary = this.SetMessageBoundary;
         pgmBindingElement.DataMode = this.DataMode;
         pgmBindingElement.SenderWidnowAdvance = this.SenderWidnowAdvance;
         pgmBindingElement.MulticastTTL = this.MulticastTTL;
         pgmBindingElement.SenderInterface = this.SenderInterface;
      }

      public override void CopyFrom(ServiceModelExtensionElement from)
      {
         base.CopyFrom(from);

         PgmTransportElement source = (PgmTransportElement)from;
         this.MaxBufferPoolSize = source.MaxBufferPoolSize;
         this.MaxReceivedMessageSize = source.MaxReceivedMessageSize;
         this.FecMode = source.FecMode;
         this.SendRate = source.SendRate;
         this.LateJoin = source.LateJoin;
         this.SetMessageBoundary = source.SetMessageBoundary;
         this.DataMode = source.DataMode;
         this.SenderWidnowAdvance = source.SenderWidnowAdvance;
         this.MulticastTTL = source.MulticastTTL;
         this.SenderInterface = source.SenderInterface;
         this.Diagnostics = source.Diagnostics;
      }

      protected override void InitializeFrom(BindingElement bindingElement)
      {
         base.InitializeFrom(bindingElement);

         PgmTransportBindingElement pgmBindingElement = (PgmTransportBindingElement)bindingElement;
         this.MaxBufferPoolSize = pgmBindingElement.MaxBufferPoolSize;
         this.MaxReceivedMessageSize = (int)pgmBindingElement.MaxReceivedMessageSize;
         this.FecMode = pgmBindingElement.FecMode;
         this.SendRate = pgmBindingElement.SendRate;
         this.LateJoin = pgmBindingElement.LateJoin;
         this.SetMessageBoundary = pgmBindingElement.SetMessageBoundary;
         this.DataMode = pgmBindingElement.DataMode;
         this.SenderWidnowAdvance = pgmBindingElement.SenderWidnowAdvance;
         this.MulticastTTL = pgmBindingElement.MulticastTTL;
         this.SenderInterface = pgmBindingElement.SenderInterface;
      }

      protected override ConfigurationPropertyCollection Properties
      {
         get
         {
            ConfigurationPropertyCollection properties = base.Properties;

            properties.Add(new ConfigurationProperty(ConfigOptions.MaxBufferPoolSize,
                typeof(long), PgmConstants.MaxBufferPoolSize, null, new LongValidator(0, Int64.MaxValue), ConfigurationPropertyOptions.None));
            properties.Add(new ConfigurationProperty(ConfigOptions.MaxReceivedMessageSize,
                typeof(int), PgmConstants.MaxReceivedMessageSize, null, new IntegerValidator(1, Int32.MaxValue), ConfigurationPropertyOptions.None));
            properties.Add(new ConfigurationProperty(ConfigOptions.ForwardErrorCorrection,
                typeof(FecMode), PgmConstants.DefaultFecMode, null, null, ConfigurationPropertyOptions.None));
            properties.Add(new ConfigurationProperty(ConfigOptions.SendRate,
                typeof(long), PgmConstants.SendRate, null, new LongValidator(0, 15360), ConfigurationPropertyOptions.None));
            properties.Add(new ConfigurationProperty(ConfigOptions.LateJoin,
                typeof(int), PgmConstants.LateJoin, null, new IntegerValidator(0, 75), ConfigurationPropertyOptions.None));
            properties.Add(new ConfigurationProperty(ConfigOptions.SetMmgBoundary,
                typeof(bool), PgmConstants.SetMmgBoundary, null, null, ConfigurationPropertyOptions.None));
            properties.Add(new ConfigurationProperty(ConfigOptions.DataMode,
                typeof(DataMode), PgmConstants.DefaultDataMode, null, null, ConfigurationPropertyOptions.None));
            properties.Add(new ConfigurationProperty(ConfigOptions.SenderWidnowAdvance,
                typeof(int), PgmDefines.SENDER_DEFAULT_WINDOW_ADV_PERCENTAGE, null, new IntegerValidator(1, 25), ConfigurationPropertyOptions.None));
            properties.Add(new ConfigurationProperty(ConfigOptions.SenderInterface,
                typeof(string), string.Empty, null, new IpValidator(PgmUtils.IPValidatorString), ConfigurationPropertyOptions.None));
            properties.Add(new ConfigurationProperty(ConfigOptions.MulticastTTL,
                typeof(int), PgmDefines.MAX_MCAST_TTL, null, new IntegerValidator(1, 255), ConfigurationPropertyOptions.None));
            properties.Add(new ConfigurationProperty(ConfigOptions.Diagnostics,
                typeof(DiagnosticsConfiguration), null, null, null, ConfigurationPropertyOptions.None));
            
            return properties;
         }
      }
   }
}
