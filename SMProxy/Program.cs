using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Sockets;
using System.Reflection;
using System.Text;

namespace SMProxy
{
    class Program
    {
        private static TcpListener Listener { get; set; }
        private static List<Proxy> Sessions { get; set; }

        static void Main(string[] args)
        {
            // Test code; will be more customizable later
            Listener = new TcpListener(IPAddress.Loopback, 25564);
            Sessions = new List<Proxy>();
            Listener.Start();
            Listener.BeginAcceptTcpClient(AcceptClient, null);

            Console.WriteLine("Press 'q' to exit.");
            ConsoleKeyInfo cki = new ConsoleKeyInfo();
            do
            {
                cki = Console.ReadKey();
            } while (cki.KeyChar != 'q');
        }

        static void AcceptClient(IAsyncResult result)
        {
            var client = Listener.EndAcceptTcpClient(result);
            Console.WriteLine("New connection from " + client.Client.RemoteEndPoint.ToString());
            // Connect to remote
            var server = new TcpClient();
            server.Connect(new IPEndPoint(IPAddress.Loopback, 25565));
            var proxy = new Proxy(client.GetStream(), server.GetStream());
            Sessions.Add(proxy);
            proxy.Start();
            Listener.BeginAcceptTcpClient(AcceptClient, null);
        }

#if DEBUG
        private static void CreatePacketArray()
        {
            // For generating the PacketReader array dynamically.
            // Not used under normal conditions.
            string array = "";
            var types = Assembly.GetExecutingAssembly().GetTypes().Where(
                t => typeof(IPacket).IsAssignableFrom(t) && !t.IsInterface && !t.IsAbstract);
            for (int i = 0; i < 256; i++)
            {
                var type = types.FirstOrDefault(t => ((IPacket)Activator.CreateInstance(t)).Id == i);
                if (type == null)
                    array += "null,";
                else
                    array += "typeof(" + type.Name + "),";
                array += " // 0x" + i.ToString("X2") + Environment.NewLine;
            }
        }
#endif
    }
}
