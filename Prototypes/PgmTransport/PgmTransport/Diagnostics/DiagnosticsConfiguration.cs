using System;
using System.Configuration;

namespace PgmTransport.Diagnostics
{
   class DiagnosticsConfiguration : ConfigurationElement
   {

      [ConfigurationProperty(ConfigOptions.DiagnosticsEnabled, DefaultValue = false)]
      public bool Enabled
      {
         get
         {
            return (bool)base[ConfigOptions.DiagnosticsEnabled];
         }
         set
         {
            base[ConfigOptions.DiagnosticsEnabled] = value;
         }
      }

      [ConfigurationProperty(ConfigOptions.Interval, DefaultValue = "00:00:05")]
      [TimeSpanValidator]
      public TimeSpan Interval
      {
         get
         {
            return (TimeSpan)base[ConfigOptions.Interval];
         }
         set
         {
            base[ConfigOptions.Interval] = value;
         }
      }

      [ConfigurationProperty(ConfigOptions.Directory)]
      public string OutputDirectory
      {
         get
         {
            return (string)base[ConfigOptions.Directory];
         }
         set
         {
            base[ConfigOptions.Directory] = value;
         }
      }
   }
}
