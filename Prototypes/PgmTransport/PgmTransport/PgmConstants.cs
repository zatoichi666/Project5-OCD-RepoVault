using PgmTransport.Sockets;

namespace PgmTransport
{
   class PgmConstants
   {
      public const string Scheme = "net.pgm";
      public const long MaxBufferPoolSize = 64 * 1024;
      public const int MaxReceivedMessageSize = 64 * 1024;
      public const long SendRate = 5 * 1024; // 5Mbps
      public const FecMode DefaultFecMode = FecMode.Disabled;
      public const int LateJoin = 50;
      public const bool SetMmgBoundary = false;
      public const DataMode DefaultDataMode = DataMode.Message;
   }

   class ConfigOptions
   {
      public const string MaxBufferPoolSize = "maxBufferPoolSize";
      public const string MaxReceivedMessageSize = "maxMessageSize";
      public const string ForwardErrorCorrection = "fecMode";
      public const string SendRate = "sendRate";
      public const string LateJoin = "lateJoin";
      public const string SetMmgBoundary = "setMessageBoundary";
      public const string DataMode = "dataMode";
      public const string SenderWidnowAdvance = "senderWidnowAdvance";
      public const string MulticastTTL = "multicastTtl";
      public const string SenderInterface = "senderInterface";

      public const string Diagnostics = "diagnostics";
      public const string DiagnosticsEnabled = "enabled";
      public const string Interval = "interval";
      public const string Directory = "directory";
   }
}
