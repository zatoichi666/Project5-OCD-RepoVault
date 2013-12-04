using System;
using System.Net;
using System.Runtime.InteropServices;
using PgmTransport.Configuration;

namespace PgmTransport.Sockets
{
   class PgmUtils
   {
      public const string IPValidatorString = @"^([1-9]|[1-9][0-9]|1[0-9][0-9]|2[0-4][0-9]|25[0-5])(\.([0-9]|[1-9][0-9]|1[0-9][0-9]|2[0-4][0-9]|25[0-5])){3}$";
      
      public static byte[] StructureToByteArray(object obj)
      {
         int len = Marshal.SizeOf(obj);
         byte[] result = new byte[len];
         IntPtr ptr = Marshal.AllocHGlobal(len);
         Marshal.StructureToPtr(obj, ptr, true);
         Marshal.Copy(ptr, result, 0, len);
         Marshal.FreeHGlobal(ptr);
         
         return result;
      }

      public static T ByteArrayToStructure<T>(byte[] array)
      {
         T result = default(T);
         int len = Marshal.SizeOf(typeof(T));
         IntPtr ptr = Marshal.AllocHGlobal(len);
         Marshal.Copy(array, 0, ptr, len);
         result = (T)Marshal.PtrToStructure(ptr, typeof(T));
         Marshal.FreeHGlobal(ptr);

         return result;
      }

      public static void ValidateMulticastIP(string ip)
      {
         ValidateIP(ip);

         IPAddress ipAddr = IPAddress.Parse(ip);
         byte[] bytes = ipAddr.GetAddressBytes();
         if((bytes[0] & 0xE0) != 0xE0)
         {
            throw new ArgumentException("Multicast IP must be in range 224.0.0.0 - 239.255.255.255");
         }
      }

      public static void ValidateIP(string ip)
      {
         new IpValidator(IPValidatorString).Validate(ip);
      }
   }
}
