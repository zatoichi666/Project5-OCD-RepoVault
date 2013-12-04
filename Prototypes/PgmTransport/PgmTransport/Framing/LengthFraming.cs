using System;
using System.ServiceModel.Channels;

namespace PgmTransport.Framing
{
   public class LenghtFraming : IFraming
   {
      const short HeaderSize = 8;
      byte[] m_header;
      long m_dataSize;
      byte[] m_data;
      int m_dataPosition = 0;
      int m_headerPositon = 0;
      BufferManager m_bufMngr = null;

      FrameReceivedEventHandler m_callback;
      ParserStatus m_state = ParserStatus.AwaitingHeader;

      public LenghtFraming(FrameReceivedEventHandler callback)
      {
         m_header = Alloc(HeaderSize);
         m_callback = callback;
      }

      public LenghtFraming(FrameReceivedEventHandler callback, BufferManager bufMngr)
         : this(callback)
      {
         m_bufMngr = bufMngr;
      }

      #region IFraming Members

      public ArraySegment<byte> PrepareBuffer(ArraySegment<byte> buffer)
      {
         long size = buffer.Count - buffer.Offset;

         byte[] newBuffer = Alloc(HeaderSize + size);

         Array.Copy(BitConverter.GetBytes(size), newBuffer, HeaderSize);
         Array.Copy(buffer.Array, buffer.Offset, newBuffer, HeaderSize, size);

         return new ArraySegment<byte>(newBuffer, 0, (int)(HeaderSize + size));
      }

      public void Parse(ArraySegment<byte> buffer)
      {
         byte[] array = buffer.Array;
         int lenght = buffer.Count + buffer.Offset;

         for (long i = buffer.Offset; i < lenght; i++)
         {
            if (m_state == ParserStatus.AwaitingHeader)
            {
               m_header[m_headerPositon] = array[i];
               m_headerPositon++;
               if (m_headerPositon == HeaderSize)
               {
                  HeaderReady();
               }
            }
            else if (m_state == ParserStatus.AwaitingData)
            {
               m_data[m_dataPosition] = array[i];
               m_dataPosition++;
               if (m_dataPosition == m_dataSize)
               {
                  DataReady();
               }
            }
         }
      }

      #endregion

      private void HeaderReady()
      {
         m_dataSize = BitConverter.ToInt64(m_header, 0);
         m_data = Alloc(m_dataSize);
         m_state = ParserStatus.AwaitingData;
         m_headerPositon = 0;
      }

      private void DataReady()
      {
         m_callback(new ArraySegment<byte>(m_data, 0, m_dataPosition));
         m_dataPosition = 0;
         Free(m_data);
         m_state = ParserStatus.AwaitingHeader;
      }

      private byte[] Alloc(long p)
      {
         if (m_bufMngr == null)
         {
            return new byte[p];
         }

         return m_bufMngr.TakeBuffer((int)p);
      }

      private void Free(byte[] buf)
      {
         if (m_bufMngr != null)
         {
            m_bufMngr.ReturnBuffer(buf);
         }
      }

      private enum ParserStatus
      {
         AwaitingHeader,
         AwaitingData
      }
   }
}
