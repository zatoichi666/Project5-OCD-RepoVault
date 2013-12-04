using System;
using System.Net;
using System.Net.Sockets;
using System.Text;

namespace multiCastComm
{
	public class send
	{
		public send(string mcastGroup, string port, string ttl, string packetContent) 
		{   
			IPAddress ip;
			try 
			{
				Console.WriteLine("MCAST Send on Group: {0} Port: {1} TTL: {2}",mcastGroup,port,ttl);
				ip=IPAddress.Parse(mcastGroup);
				
				Socket s=new Socket(AddressFamily.InterNetwork, 
								SocketType.Dgram, ProtocolType.Udp);
				
				s.SetSocketOption(SocketOptionLevel.IP, 
					SocketOptionName.AddMembership, new MulticastOption(ip));

				s.SetSocketOption(SocketOptionLevel.IP, 
					SocketOptionName.MulticastTimeToLive, int.Parse(ttl));
			


                byte[] b = Encoding.ASCII.GetBytes(packetContent);

				IPEndPoint ipep=new IPEndPoint(IPAddress.Parse(mcastGroup),int.Parse(port));
				
				Console.WriteLine("Connecting...");

				s.Connect(ipep);

					s.Send(b,b.Length,SocketFlags.None);
               

				Console.WriteLine("Closing Connection...");
				s.Close();
			} 
			catch(System.Exception e) { Console.Error.WriteLine(e.Message); }
		}

		static void Main(string[] args)
		{
            System.Threading.Thread.Sleep(1000);
            new send("224.5.6.7", "5000", "1", "2");
		}
	}
}
