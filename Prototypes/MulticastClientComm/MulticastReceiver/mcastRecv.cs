using System.Net.Sockets;
using System.Net;
using System.Threading;
using System;

namespace multiCastComm 
{
    public class someEventArgs : EventArgs
    {
        private string _msg;

        public someEventArgs(string msg)
        {
            _msg = msg;
        }

        public string msg
        {
            get
            {
                return _msg;
            }
            set
            {
                _msg = value;
            }
        }
    }

	public class recv 
	{
        Socket s = null;
        public delegate void incomingPacketEventHandler(object sender, EventArgs seva);
        public event incomingPacketEventHandler incomingPacketEvent;

        private int port;        

		public recv(string mcastGroup, string port) 
		{
            this.port = int.Parse(port);
			s=new Socket(AddressFamily.InterNetwork, SocketType.Dgram, ProtocolType.Udp);
            
            IPEndPoint ipep = new IPEndPoint(IPAddress.Any, this.port);
            IPAddress ip = IPAddress.Parse(mcastGroup);
            s.SetSocketOption(SocketOptionLevel.IP, SocketOptionName.AddMembership, new MulticastOption(ip, IPAddress.Any));
            // Allows multiple sockets listening on a single port, on a given machine
            s.SetSocketOption(SocketOptionLevel.Socket, SocketOptionName.ReuseAddress, true);                                   
			s.Bind(ipep);
            Console.WriteLine("Starting multicast receiver on port {0}", this.port.ToString());
		}

        public void start()
        {
            
            while (true)
            {
                String message;
                byte[] b = new byte[10];
                Console.WriteLine("Waiting for data..");
                s.Receive(b);
                string str = System.Text.Encoding.ASCII.GetString(b, 0, b.Length);
                Console.WriteLine("RX: " + str.Trim());
                message = str.Trim();
                someEventArgs seva
                        = new someEventArgs(message);
                if (incomingPacketEvent != null)
                    incomingPacketEvent(this, seva);
            }
        }

		public static void Main(string[] args) 
		{            
			new recv("224.5.6.7","5000");
		}
	}
}