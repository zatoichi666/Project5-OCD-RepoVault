using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace SimpleSender
{
   class Program
   {
      static void Main(string[] args)
      {
         WorkServiceClient client = new WorkServiceClient();
         int counter = 0;
         Console.WriteLine("Starting to send...");
         while (true)
         {
            client.DoWork("Get to work");

            Console.Write("Sent " + counter++ +" messages\r");
         }
      }
   }
}
