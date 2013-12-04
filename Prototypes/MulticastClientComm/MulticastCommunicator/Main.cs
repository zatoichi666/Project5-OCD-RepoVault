using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;

namespace multiCastComm
{
    class main
    {
        static void Main(string[] args)
        {
            recv rr = new recv("224.5.6.7", "5000");
            McastComm mc = new McastComm(rr);
            Thread.Sleep(5000);
            
            for (int i=0;i<10;i++)
            {
                mc.broadcastPacket(i.ToString(), "5000");
            }

            Thread.Sleep(5000);
            Console.WriteLine("Done");
        }
    }
}
