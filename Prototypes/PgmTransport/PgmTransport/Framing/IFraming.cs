using System;

namespace PgmTransport.Framing
{
   public interface IFraming
   {
      ArraySegment<byte> PrepareBuffer(ArraySegment<byte> buffer);
      void Parse(ArraySegment<byte> buffer);
   }
}
