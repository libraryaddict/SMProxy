using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.ComponentModel;

namespace SMProxy
{
    public interface IPacket
    {
        int Id { get; }
        void ReadPacket(MinecraftStream stream);
        void WritePacket(MinecraftStream stream);
    }

    [Description("Used by the server to ensure the connection is alive.")]
    public struct KeepAlivePacket : IPacket
    {
        [Description("Clients respond with the same keep alive value to indicate that they are still connected.")]
        public int KeepAlive;

        public int Id { get { return 0x00; } }

        public void ReadPacket(MinecraftStream stream)
        {
            KeepAlive = stream.ReadInt32();
        }

        public void WritePacket(MinecraftStream stream)
        {
            stream.WriteInt32(KeepAlive);
        }
    }

    [Description("Send by the server to confirm a login and spawn the player.")]
    public struct LoginRequestPacket : IPacket
    {
        [Description("The client's assigned entity ID.")]
        public int EntityId;
        [Description("Level type from server.properties on vanilla")]
        public string LevelType;
        public GameMode GameMode;
        public Dimension Dimension;
        public Difficulty Difficulty;
        [EditorBrowsable(EditorBrowsableState.Never)] // TODO: Is there a better attribute for this? I don't want to create a new one.
        public byte Discarded;
        public byte MaxPlayers;

        public int Id { get { return 0x01; } }

        public void ReadPacket(MinecraftStream stream)
        {
            EntityId = stream.ReadInt32();
            LevelType = stream.ReadString();
            GameMode = (GameMode)stream.ReadUInt8();
            Dimension = (Dimension)stream.ReadInt8();
            Difficulty = (Difficulty)stream.ReadUInt8();
            Discarded = stream.ReadUInt8();
            MaxPlayers = stream.ReadUInt8();
        }

        public void WritePacket(MinecraftStream stream)
        {
            stream.WriteInt32(EntityId);
            stream.WriteString(LevelType);
            stream.WriteUInt8((byte)GameMode);
            stream.WriteInt8((sbyte)Dimension);
            stream.WriteUInt8((byte)Difficulty);
            stream.WriteUInt8(Discarded);
            stream.WriteUInt8(MaxPlayers);
        }
    }
}
