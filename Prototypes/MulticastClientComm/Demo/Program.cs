using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;

using multiCastComm;

namespace Demo
{
    class Program
    {
        static void Main(string[] args)
        {
            Console.WriteLine("Enter the name of the user");
            String ParticipantName = Console.ReadLine();
            
            recv rr = new recv("224.5.6.7", "5000");
            McastComm mc = new McastComm(rr);
            Thread.Sleep(5000);

            for (int i = 0; i < 10; i++)
            {
                mc.broadcastPacket(ParticipantName + ": " + i.ToString(), "5000");
            }

            Thread.Sleep(5000);
            Console.WriteLine("Done");
        }
    }
}
