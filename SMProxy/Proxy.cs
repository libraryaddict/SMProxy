using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Sockets;
using System.Text;
using System.Threading;

namespace SMProxy
{
    public class Proxy
    {
        public const int ProtocolVersion = 49;
        public const int MillisecondsBetweenUpdates = 10;
        
        public NetworkStream Client { get; set; }
        public NetworkStream Server { get; set; }
        private MinecraftStream ClientStream { get; set; }
        private MinecraftStream ServerStream { get; set; }
        public Timer Timer { get; set; }

        public Proxy(NetworkStream client, NetworkStream server)
        {
            Client = client;
            Server = server;
            ClientStream = new MinecraftStream(Client);
            ServerStream = new MinecraftStream(Server);
        }

        public void Start()
        {
            Timer = new Timer(Tick, null, MillisecondsBetweenUpdates, Timeout.Infinite);
        }

        public void Stop()
        {
            Timer.Change(Timeout.Infinite, Timeout.Infinite);
        }

        private void Tick(object discarded)
        {
            // TODO: Fallback to raw proxy
            UpdateServer();
            UpdateClient();
            Timer.Change(MillisecondsBetweenUpdates, Timeout.Infinite);
        }

        private void UpdateClient()
        {
            if (!Client.DataAvailable)
                return;
            var packet = PacketReader.ReadPacket(ClientStream);
            packet.WritePacket(ServerStream);
        }

        private void UpdateServer()
        {
            if (!Server.DataAvailable)
                return;
            var packet = PacketReader.ReadPacket(ServerStream);
            packet.WritePacket(ClientStream);
        }
    }
}
