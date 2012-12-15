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
            ClientStream = new MinecraftStream(new BufferedStream(Client));
            ServerStream = new MinecraftStream(new BufferedStream(Server));
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
            // We do the timer this way in case there's some enormous packet or something that takes more than
            // MillisecondsBetweenUpdates to deal with. This way, we don't have a race condition where Tick is
            // running several times simultaneously.
            Timer.Change(MillisecondsBetweenUpdates, Timeout.Infinite);
        }

        private void UpdateClient()
        {
            if (!Client.DataAvailable)
                return;
            var packet = PacketReader.ReadPacket(ClientStream);
            // WritePacket doesn't include the packet ID, just the payload
            ServerStream.WriteUInt8(packet.Id);
            packet.WritePacket(ServerStream);
            // We use a BufferedStream to make sure packets get sent in one piece, rather than
            // a field at a time. Flushing it here sends the assembled packet.
            ServerStream.Flush();
            Console.WriteLine(packet.GetType().Name);
        }

        private void UpdateServer()
        {
            if (!Server.DataAvailable)
                return;
            var packet = PacketReader.ReadPacket(ServerStream);
            ClientStream.WriteUInt8(packet.Id);
            packet.WritePacket(ClientStream);
            ClientStream.Flush();
            Console.WriteLine(packet.GetType().Name);
        }
    }
}
