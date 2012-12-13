using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;

namespace SMProxy
{
    public interface IPacket
    {
        IPacket ReadPacket(MinecraftStream stream);
        void WritePacket(MinecraftStream stream);
        PacketDirection ValidDirections { get; }
        PacketDirection PacketDirection { get; set; }
    }

    [Flags]
    public enum PacketDirection
    {
        ClientToServer,
        ServerToClient
    }
}
