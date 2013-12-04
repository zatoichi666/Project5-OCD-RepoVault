using System.Net.Sockets;

namespace PgmTransport.Sockets
{
   class PgmDefines
   {
      const int IPPROTO_RM = 113;

      public const ProtocolType PgmProtocolType = (ProtocolType)IPPROTO_RM;
      public const SocketOptionLevel PgmSocketOption = (SocketOptionLevel)IPPROTO_RM;

      public const int SENDER_DEFAULT_RATE_KBITS_PER_SEC = 56;
      public const int SENDER_DEFAULT_WINDOW_SIZE_BYTES = 10 * 1000 * 1000;

      public const int SENDER_DEFAULT_WINDOW_ADV_PERCENTAGE = 15;
      public const int MAX_WINDOW_INCREMENT_PERCENTAGE = 25;

      public const int MAX_MCAST_TTL = 255;

      public const int SENDER_DEFAULT_LATE_JOINER_PERCENTAGE = 0;
      public const int SENDER_MAX_LATE_JOINER_PERCENTAGE = 75;

      public const int FEC_GROUP_SIZE = 64;
      public const int FEC_BLOCK_SIZE = 128;
      public const int FEC_PROACTIVE_PACKETS = 128;
   }

   public enum RmOptionName
   {
      RM_RATE_WINDOW_SIZE = 1001,
      RM_SET_MESSAGE_BOUNDARY,
      RM_FLUSHCACHE,
      RM_SENDER_WINDOW_ADVANCE_METHOD,
      RM_SENDER_STATISTICS,
      RM_LATEJOIN,
      RM_SET_SEND_IF,
      RM_ADD_RECEIVE_IF,
      RM_DEL_RECEIVE_IF,
      RM_SEND_WINDOW_ADV_RATE,
      RM_USE_FEC,
      RM_SET_MCAST_TTL,
      RM_RECEIVER_STATISTICS,
      RM_HIGH_SPEED_INTRANET_OPT,
   }

   public enum FecMode
   {
      Disabled,
      ProActive,
      OnDemand,
      Both
   }

   public enum DataMode
   {
      Message = SocketType.Rdm,
      Stream = SocketType.Stream
   }

   /// <summary>
   /// WindowSizeInBytes = (RateKbitsPerSec / 8) * WindowSizeInMSecs
   /// </summary>
   public struct _RM_SEND_WINDOW
   {
      public ulong RateKbitsPerSec;            // Send rate
      public ulong WindowSizeInMSecs;
      public ulong WindowSizeInBytes;
   }

   public struct _RM_FEC_INFO
   {
      public ushort FECBlockSize;
      public ushort FECProActivePackets;
      public byte FECGroupSize;
      public byte fFECOnDemandParityEnabled;
   }

   public struct _RM_SENDER_STATS
   {
      public ulong DataBytesSent;          // # client data bytes sent out so far
      public ulong TotalBytesSent;         // SPM, OData and RData bytes
      public ulong NaksReceived;           // # NAKs received so far
      public ulong NaksReceivedTooLate;    // # NAKs recvd after window advanced
      public ulong NumOutstandingNaks;     // # NAKs yet to be responded to
      public ulong NumNaksAfterRData;      // # NAKs yet to be responded to
      public ulong RepairPacketsSent;      // # Repairs (RDATA) sent so far
      public ulong BufferSpaceAvailable;   // # partial messages dropped
      public ulong TrailingEdgeSeqId;      // smallest (oldest) Sequence Id in the window
      public ulong LeadingEdgeSeqId;       // largest (newest) Sequence Id in the window
      public ulong RateKBitsPerSecOverall; // Internally calculated send-rate from the beginning
      public ulong RateKBitsPerSecLast;    // Send-rate calculated every INTERNAL_RATE_CALCULATION_FREQUENCY
      public ulong TotalODataPacketsSent;  // # ODATA packets sent so far
   }

   public struct _RM_RECEIVER_STATS
   {
      public ulong NumODataPacketsReceived;// # OData sequences received
      public ulong NumRDataPacketsReceived;// # RData sequences received
      public ulong NumDuplicateDataPackets;// # RData sequences received

      public ulong DataBytesReceived;      // # client data bytes received out so far
      public ulong TotalBytesReceived;     // SPM, OData and RData bytes
      public ulong RateKBitsPerSecOverall; // Internally calculated Receive-rate from the beginning
      public ulong RateKBitsPerSecLast;    // Receive-rate calculated every INTERNAL_RATE_CALCULATION_FREQUENCY

      public ulong TrailingEdgeSeqId;      // smallest (oldest) Sequence Id in the window
      public ulong LeadingEdgeSeqId;       // largest (newest) Sequence Id in the window
      public ulong AverageSequencesInWindow;
      public ulong MinSequencesInWindow;
      public ulong MaxSequencesInWindow;

      public ulong FirstNakSequenceNumber; // # First Outstanding Nak
      public ulong NumPendingNaks;         // # Sequences waiting for Ncfs
      public ulong NumOutstandingNaks;     // # Sequences for which Ncfs have been received, but no data
      public ulong NumDataPacketsBuffered; // # Data packets currently buffered by transport
      public ulong TotalSelectiveNaksSent; // # Selective NAKs sent so far
      public ulong TotalParityNaksSent;    // # Parity NAKs sent so far
   }
}
