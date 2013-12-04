using System;

namespace PgmTransport
{
   public delegate void FrameReceivedEventHandler(ArraySegment<byte> buffer);
}
