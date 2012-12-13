using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;

namespace SMProxy
{
    public interface IPacket
    {
        int Id { get; }
        void ReadPacket(MinecraftStream stream);
        void WritePacket(MinecraftStream stream);
    }

    public struct KeepAlivePacket : IPacket
    {
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

    public struct LoginRequestPacket : IPacket
    {
        public int EntityId;
        public string LevelType;
        public GameMode GameMode;
        public Dimension Dimension;
        public Difficulty Difficulty;
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

    public struct HandshakePacket : IPacket
    {
        public byte ProtocolVersion;
        public string Username;
        public string ServerHostname;
        public int ServerPort;

        public int Id { get { return 0x02; } }

        public void ReadPacket(MinecraftStream stream)
        {
            ProtocolVersion = stream.ReadUInt8();
            Username = stream.ReadString();
            ServerHostname = stream.ReadString();
            ServerPort = stream.ReadInt32();
        }

        public void WritePacket(MinecraftStream stream)
        {
            stream.WriteUInt8(ProtocolVersion);
            stream.WriteString(Username);
            stream.WriteString(ServerHostname);
            stream.WriteInt32(ServerPort);
        }
    }

    public struct ChatMessagePacket : IPacket
    {
        public string Message;

        public int Id { get { return 0x03; } }

        public void ReadPacket(MinecraftStream stream)
        {
            Message = stream.ReadString();
        }

        public void WritePacket(MinecraftStream stream)
        {
            stream.WriteString(Message);
        }
    }

    public struct TimeUpdatePacket : IPacket
    {
        public long WorldAge, TimeOfDay;

        public int Id { get { return 0x04; } }

        public void ReadPacket(MinecraftStream stream)
        {
            WorldAge = stream.ReadInt64();
            TimeOfDay = stream.ReadInt64();
        }

        public void WritePacket(MinecraftStream stream)
        {
            stream.WriteInt64(WorldAge);
            stream.WriteInt64(TimeOfDay);
        }
    }

    public struct EntityEquipmentPacket : IPacket
    {
        public int EntityId;
        public short SlotIndex;
        public Slot Slot;

        public int Id { get { return 0x05; } }

        public void ReadPacket(MinecraftStream stream)
        {
            EntityId = stream.ReadInt32();
            SlotIndex = stream.ReadInt16();
            Slot = Slot.FromStream(stream);
        }

        public void WritePacket(MinecraftStream stream)
        {
            stream.WriteInt32(EntityId);
            stream.WriteInt16(SlotIndex);
            Slot.WriteTo(stream);
        }
    }

}
