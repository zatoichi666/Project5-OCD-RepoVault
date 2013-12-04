using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;

using multiCastComm;

namespace DemoB
{
    class Program
    {
        static void Main(string[] args)
        {
            recv rr = new recv("224.5.6.7", "5000");
            McastComm mc = new McastComm(rr);
        
            Thread.Sleep(5000);
            Console.WriteLine("Done");
        }
    }
}
