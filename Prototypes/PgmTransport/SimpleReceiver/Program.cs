using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.ServiceModel;

namespace SimpleReceiver
{
   class Program
   {
      static void Main(string[] args)
      {
         WorkService service = new WorkService();
         ServiceHost host = new ServiceHost(service);

         host.Open();
         Console.WriteLine("Service is running...");
         Console.ReadLine();

         host.Close();
      }
   }
}
