using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Sockets;
using System.Reflection;
using System.Text;
using System.Globalization;

namespace SMProxy
{
    class Program
    {
        private static TcpListener Listener { get; set; }
        private static List<Proxy> Sessions { get; set; }
        private static ProxySettings ProxySettings { get; set; }

        static void Main(string[] args)
        {
            ProxySettings = new ProxySettings();
            bool remoteSet = false, localSet = false;
            // Interpret command line args
            for (int i = 0; i < args.Length; i++)
            {
                var arg = args[i];
                if (arg.StartsWith("-"))
                {
                    switch (arg)
                    {
                        case "--local-endpoint":
                            ProxySettings.LocalEndPoint = ParseEndPoint(args[++i]);
                            break;
                        case "--filter":
                            ProxySettings.PacketFilter = ParseFilter(args[++i]);
                            break;
                        case "--unfilter":
                            var filter = ParseFilter(args[++i]);
                            ProxySettings.PacketFilter.RemoveAll(p => filter.Contains(p));
                            break;
                        case "--omit-client":
                            ProxySettings.LogClient = false;
                            break;
                        case "--omit-server":
                            ProxySettings.LogServer = false;
                            break;
                        case "--username":
                            ProxySettings.Username = args[++i];
                            break;
                        case "--password":
                            ProxySettings.Password = args[++i];
                            break;
                        default:
                            Console.WriteLine("Invalid command line arguments. Use --help for more information.");
                            return;
                    }
                }
                else
                {
                    if (!remoteSet)
                        ProxySettings.RemoteEndPoint = ParseEndPoint(arg);
                    else if (!localSet)
                        ProxySettings.LocalEndPoint = ParseEndPoint(arg);
                    else
                    {
                        Console.WriteLine("Invalid command line arguments. Use --help for more information.");
                        return;
                    }
                }
            }

            if (ProxySettings.Password == null)
            {
                // Grab lastlogin if possible
                var login = Minecraft.GetLastLogin();
                if (login != null)
                {
                    if (ProxySettings.Username == null)
                        ProxySettings.Username = login.Username;
                    ProxySettings.Password = login.Password;
                }
            }

            Listener = new TcpListener(ProxySettings.LocalEndPoint);
            Sessions = new List<Proxy>();
            Listener.Start();
            Listener.BeginAcceptTcpClient(AcceptClient, null);

            Console.WriteLine("Press 'q' to exit.");

            ConsoleKeyInfo cki = new ConsoleKeyInfo();
            do { cki = Console.ReadKey(); } while (cki.KeyChar != 'q');
        }

        private static void AcceptClient(IAsyncResult result)
        {
            var client = Listener.EndAcceptTcpClient(result);
            Console.WriteLine("New connection from " + client.Client.RemoteEndPoint.ToString());
            // Connect to remote
            var server = new TcpClient();
            server.Connect(ProxySettings.RemoteEndPoint);
            var proxy = new Proxy(client.GetStream(), server.GetStream(),
                new Log(new StreamWriter(GetLogName())));
            Sessions.Add(proxy);
            proxy.Start();
            Listener.BeginAcceptTcpClient(AcceptClient, null);
        }

        private static List<byte> ParseFilter(string filter)
        {
            var result = new List<byte>();
            var packets = filter.Split(',');
            foreach (var p in packets)
                result.Add(byte.Parse(p, NumberStyles.HexNumber));
            return result;
        }

        private static IPEndPoint ParseEndPoint(string arg)
        {
            if (arg.Contains(':'))
            {
                // Both IP and port are specified
                var parts = arg.Split(':');
                return new IPEndPoint(IPAddress.Parse(parts[0]), int.Parse(parts[1]));
            }
            if (arg.Contains('.')) // IP specified
                return new IPEndPoint(IPAddress.Parse(arg), 25565);
            return new IPEndPoint(IPAddress.Loopback, int.Parse(arg));
        }

        private static string GetLogName()
        {
            return "log_" + DateTime.Now.ToString("dd-MM-yyyy_HH-mm-ss") + ".txt";
        }
    }
}
