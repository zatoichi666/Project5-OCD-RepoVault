///////////////////////////////////////////////////////////////////
//                                                               //
// MulticastCommunicator.cs - Multicast receiver/sender          //
//                                                               //
//   Matt Synborski                                              //
// Derived from:                                                 //
// http://www.codeproject.com/Articles/1705/IP-Multicasting-in-C //
///////////////////////////////////////////////////////////////////

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;

namespace multiCastComm
{
    public class McastComm
    {
        private send s = null;
        private recv r = null;

        public McastComm(recv _r)
        {
            Console.WriteLine("Firing up a communicator ");
            r = _r;
            r.incomingPacketEvent +=
                new recv.incomingPacketEventHandler(DisplayContents);

            new Thread(r.start).Start();       
                   
        }

        public void broadcastPacket(String packetContent, String port)
        {
            s = new send("224.5.6.7", port, "10", packetContent);
        }

        public void DisplayContents(object obj, EventArgs seva)
        {
            String filename = ((someEventArgs)seva).msg;
            Console.WriteLine("{" + filename + "}");
            Console.WriteLine();
        }
    }
}
