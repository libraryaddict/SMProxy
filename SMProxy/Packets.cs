using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using SMProxy.Metadata;

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

    public struct SpawnPositionPacket : IPacket
    {
        public int X, Y, Z;

        public int Id { get { return 0x06; } }

        public void ReadPacket(MinecraftStream stream)
        {
            X = stream.ReadInt32();
            Y = stream.ReadInt32();
            Z = stream.ReadInt32();
        }

        public void WritePacket(MinecraftStream stream)
        {
            stream.WriteInt32(X);
            stream.WriteInt32(Y);
            stream.WriteInt32(Z);
        }
    }

    public struct UseEntityPacket : IPacket
    {
        public int User, Target;
        public bool MouseButton;

        public int Id { get { return 0x07; } }

        public void ReadPacket(MinecraftStream stream)
        {
            User = stream.ReadInt32();
            Target = stream.ReadInt32();
            MouseButton = stream.ReadBoolean();
        }

        public void WritePacket(MinecraftStream stream)
        {
            stream.WriteInt32(User);
            stream.WriteInt32(Target);
            stream.WriteBoolean(MouseButton);
        }
    }

    public struct UpdateHealthPacket : IPacket
    {
        public short Health, Food;
        public float FoodSaturation;

        public int Id { get { return 0x08; } }

        public void ReadPacket(MinecraftStream stream)
        {
            Health = stream.ReadInt16();
            Food = stream.ReadInt16();
            FoodSaturation = stream.ReadSingle();
        }

        public void WritePacket(MinecraftStream stream)
        {
            stream.WriteInt16(Health);
            stream.WriteInt16(Food);
            stream.WriteSingle(FoodSaturation);
        }
    }

    public struct RespawnPacket : IPacket
    {
        public Dimension Dimension;
        public Difficulty Difficulty;
        public GameMode GameMode;
        public short WorldHeight;
        public string LevelType;

        public int Id { get { return 0x09; } }

        public void ReadPacket(MinecraftStream stream)
        {
            Dimension = (Dimension)stream.ReadInt32();
            Difficulty = (Difficulty)stream.ReadInt8();
            GameMode = (GameMode)stream.ReadInt8();
            WorldHeight = stream.ReadInt16();
            LevelType = stream.ReadString();
        }

        public void WritePacket(MinecraftStream stream)
        {
            stream.WriteInt32((int)Dimension);
            stream.WriteInt8((sbyte)Difficulty);
            stream.WriteInt8((sbyte)GameMode);
            stream.WriteInt16(WorldHeight);
            stream.WriteString(LevelType);
        }
    }

    public struct PlayerPacket : IPacket
    {
        public bool OnGround;

        public int Id { get { return 0x0A; } }

        public void ReadPacket(MinecraftStream stream)
        {
            OnGround = stream.ReadBoolean();
        }

        public void WritePacket(MinecraftStream stream)
        {
            stream.WriteBoolean(OnGround);
        }
    }

    public struct PlayerPositionPacket : IPacket
    {
        public double X, Y, Stance, Z;
        public bool OnGround;

        public int Id { get { return 0x0B; } }

        public void ReadPacket(MinecraftStream stream)
        {
            X = stream.ReadDouble();
            Y = stream.ReadDouble();
            Stance = stream.ReadDouble();
            Z = stream.ReadDouble();
            OnGround = stream.ReadBoolean();
        }

        public void WritePacket(MinecraftStream stream)
        {
            stream.WriteDouble(X);
            stream.WriteDouble(Y);
            stream.WriteDouble(Stance);
            stream.WriteDouble(Z);
            stream.WriteBoolean(OnGround);
        }
    }

    public struct PlayerLookPacket : IPacket
    {
        public float Yaw, Pitch;
        public bool OnGround;

        public int Id { get { return 0x0C; } }

        public void ReadPacket(MinecraftStream stream)
        {
            Yaw = stream.ReadSingle();
            Pitch = stream.ReadSingle();
            OnGround = stream.ReadBoolean();
        }

        public void WritePacket(MinecraftStream stream)
        {
            stream.WriteSingle(Yaw);
            stream.WriteSingle(Pitch);
            stream.WriteBoolean(OnGround);
        }
    }

    public struct PlayerPositionAndLookPacket : IPacket
    {
        public double X, Y, Stance, Z;
        public float Yaw, Pitch;
        public bool OnGround;

        public int Id { get { return 0x0D; } }

        public void ReadPacket(MinecraftStream stream)
        {
            // TODO: Investigate if Y/Stance are indeed swapped
            X = stream.ReadDouble();
            Y = stream.ReadDouble();
            Stance = stream.ReadDouble();
            Z = stream.ReadDouble();
            Yaw = stream.ReadSingle();
            Pitch = stream.ReadSingle();
            OnGround = stream.ReadBoolean();
        }

        public void WritePacket(MinecraftStream stream)
        {
            stream.WriteDouble(X);
            stream.WriteDouble(Y);
            stream.WriteDouble(Stance);
            stream.WriteDouble(Z);
            stream.WriteSingle(Yaw);
            stream.WriteSingle(Pitch);
            stream.WriteBoolean(OnGround);
        }
    }

    public struct PlayerDiggingPacket : IPacket
    {
        public byte Status;
        public int X;
        public byte Y;
        public int Z;
        public byte Face;

        public int Id { get { return 0x0E; } }

        public void ReadPacket(MinecraftStream stream)
        {
            Status = stream.ReadUInt8();
            X = stream.ReadInt32();
            Y = stream.ReadUInt8();
            Z = stream.ReadInt32();
            Face = stream.ReadUInt8();
        }

        public void WritePacket(MinecraftStream stream)
        {
            stream.WriteUInt8(Status);
            stream.WriteInt32(X);
            stream.WriteUInt8(Y);
            stream.WriteInt32(Z);
            stream.WriteUInt8(Face);
        }
    }

    public struct PlayerBlockPlacementPacket : IPacket
    {
        public int X;
        public byte Y;
        public int Z;
        public byte Direction;
        public Slot HeldItem;
        public byte CursorX;
        public byte CursorY;
        public byte CursorZ;

        public int Id { get { return 0x0F; } }

        public void ReadPacket(MinecraftStream stream)
        {
            X = stream.ReadInt32();
            Y = stream.ReadUInt8();
            Z = stream.ReadInt32();
            Direction = stream.ReadUInt8();
            HeldItem = Slot.FromStream(stream);
            CursorX = stream.ReadUInt8();
            CursorY = stream.ReadUInt8();
            CursorZ = stream.ReadUInt8();
        }

        public void WritePacket(MinecraftStream stream)
        {
            stream.WriteInt32(X);
            stream.WriteUInt8(Y);
            stream.WriteInt32(Z);
            stream.WriteUInt8(Direction);
            HeldItem.WriteTo(stream);
            stream.WriteUInt8(CursorX);
            stream.WriteUInt8(CursorY);
            stream.WriteUInt8(CursorZ);
        }
    }

    public struct HeldItemChangePacket : IPacket
    {
        public short SlotIndex;

        public int Id { get { return 0x10; } }

        public void ReadPacket(MinecraftStream stream)
        {
            SlotIndex = stream.ReadInt16();
        }

        public void WritePacket(MinecraftStream stream)
        {
            stream.WriteInt16(SlotIndex);
        }
    }

    public struct UseBedPacket : IPacket
    {
        public int EntityId;
        public byte Unknown;
        public int X;
        public byte Y;
        public int Z;

        public int Id { get { return 0x11; } }

        public void ReadPacket(MinecraftStream stream)
        {
            EntityId = stream.ReadInt32();
            Unknown = stream.ReadUInt8();
            X = stream.ReadInt32();
            Y = stream.ReadUInt8();
            Z = stream.ReadInt32();
        }

        public void WritePacket(MinecraftStream stream)
        {
            stream.WriteInt32(EntityId);
            stream.WriteUInt8(Unknown);
            stream.WriteInt32(X);
            stream.WriteUInt8(Y);
            stream.WriteInt32(Z);
        }
    }

    public struct AnimationPacket : IPacket
    {
        public int EntityId;
        public byte Animation;

        public int Id { get { return 0x12; } }

        public void ReadPacket(MinecraftStream stream)
        {
            EntityId = stream.ReadInt32();
            Animation = stream.ReadUInt8();
        }

        public void WritePacket(MinecraftStream stream)
        {
            stream.WriteInt32(EntityId);
            stream.WriteUInt8(Animation);
        }
    }

    public struct EntityActionPacket : IPacket
    {
        public int EntityId;
        public byte ActionId;

        public int Id { get { return 0x13; } }

        public void ReadPacket(MinecraftStream stream)
        {
            EntityId = stream.ReadInt32();
            ActionId = stream.ReadUInt8();
        }

        public void WritePacket(MinecraftStream stream)
        {
            stream.WriteInt32(EntityId);
            stream.WriteUInt8(ActionId);
        }
    }

    public struct SpawnNamedEntityPacket : IPacket
    {
        public int EntityId;
        public string PlayerName;
        public int X, Y, Z;
        public byte Yaw, Pitch;
        public short HeldItem;
        public MetadataDictionary Metadata;

        public int Id { get { return 0x14; } }

        public void ReadPacket(MinecraftStream stream)
        {
            EntityId = stream.ReadInt32();
            PlayerName = stream.ReadString();
            X = stream.ReadInt32();
            Y = stream.ReadInt32();
            Z = stream.ReadInt32();
            Yaw = stream.ReadUInt8();
            Pitch = stream.ReadUInt8();
            HeldItem = stream.ReadInt16();
            Metadata = MetadataDictionary.FromStream(stream);
        }

        public void WritePacket(MinecraftStream stream)
        {
            stream.WriteInt32(EntityId);
            stream.WriteString(PlayerName);
            stream.WriteInt32(X);
            stream.WriteInt32(Y);
            stream.WriteInt32(Z);
            stream.WriteUInt8(Yaw);
            stream.WriteUInt8(Pitch);
            stream.WriteInt16(HeldItem);
            Metadata.WriteTo(stream);
        }
    }

    public struct SpawnDroppedItemPacket : IPacket
    {
        public int EntityId;
        public Slot Item;
        public int X, Y, Z;
        public byte Rotation;
        public byte Pitch;
        public byte Roll;

        public int Id { get { return 0x15; } }

        public void ReadPacket(MinecraftStream stream)
        {
            EntityId = stream.ReadInt32();
            Item = Slot.FromStream(stream);
            X = stream.ReadInt32();
            Y = stream.ReadInt32();
            Z = stream.ReadInt32();
            Rotation = stream.ReadUInt8();
            Pitch = stream.ReadUInt8();
            Roll = stream.ReadUInt8();
        }

        public void WritePacket(MinecraftStream stream)
        {
            stream.WriteInt32(EntityId);
            Item.WriteTo(stream);
            stream.WriteInt32(X);
            stream.WriteInt32(Y);
            stream.WriteInt32(Z);
            stream.WriteUInt8(Rotation);
            stream.WriteUInt8(Pitch);
            stream.WriteUInt8(Roll);
        }
    }

    public struct CollectItemPacket : IPacket
    {
        public int ItemId;
        public int PlayerId;

        public int Id { get { return 0x16; } }

        public void ReadPacket(MinecraftStream stream)
        {
            ItemId = stream.ReadInt32();
            PlayerId = stream.ReadInt32();
        }

        public void WritePacket(MinecraftStream stream)
        {
            stream.WriteInt32(ItemId);
            stream.WriteInt32(PlayerId);
        }
    }

    public struct SpawnObjectOrVehiclePacket : IPacket
    {
        public int EntityId;
        public byte Type;
        public int X, Y, Z;
        public int Metadata;
        public short? SpeedX, SpeedY, SpeedZ;

        public int Id { get { return 0x17; } }

        public void ReadPacket(MinecraftStream stream)
        {
            EntityId = stream.ReadInt32();
            Type = stream.ReadUInt8();
            X = stream.ReadInt32();
            Y = stream.ReadInt32();
            Z = stream.ReadInt32();
            Metadata = stream.ReadInt32();
            if (Metadata != 0)
            {
                SpeedX = stream.ReadInt16();
                SpeedY = stream.ReadInt16();
                SpeedZ = stream.ReadInt16();
            }
        }

        public void WritePacket(MinecraftStream stream)
        {
            stream.WriteInt32(EntityId);
            stream.WriteUInt8(Type);
            stream.WriteInt32(X);
            stream.WriteInt32(Y);
            stream.WriteInt32(Z);
            stream.WriteInt32(Metadata);
            if (Metadata != 0)
            {
                stream.WriteInt16(SpeedX.Value);
                stream.WriteInt16(SpeedY.Value);
                stream.WriteInt16(SpeedZ.Value);
            }
        }
    }

    public struct SpawnMobPacket : IPacket
    {
        public int EntityId;
        public byte Type;
        public int X, Y, Z;
        public byte Yaw, Pitch, HeadYaw;
        public short VelocityX, VelocityY, VelocityZ;
        public MetadataDictionary Metadata;

        public int Id { get { return 0x18; } }

        public void ReadPacket(MinecraftStream stream)
        {
            EntityId = stream.ReadInt32();
            Type = stream.ReadUInt8();
            X = stream.ReadInt32();
            Y = stream.ReadInt32();
            Z = stream.ReadInt32();
            Yaw = stream.ReadUInt8();
            Pitch = stream.ReadUInt8();
            VelocityX = stream.ReadInt16();
            VelocityY = stream.ReadInt16();
            VelocityZ = stream.ReadInt16();
            Metadata = MetadataDictionary.FromStream(stream);
        }

        public void WritePacket(MinecraftStream stream)
        {
            stream.WriteInt32(EntityId);
            stream.WriteUInt8(Type);
            stream.WriteInt32(X);
            stream.WriteInt32(Y);
            stream.WriteInt32(Z);
            stream.WriteUInt8(Yaw);
            stream.WriteUInt8(Pitch);
            stream.WriteInt16(VelocityX);
            stream.WriteInt16(VelocityY);
            stream.WriteInt16(VelocityZ);
            Metadata.WriteTo(stream);
        }
    }

    public struct SpawnPaintingPacket : IPacket
    {
        public int EntityId;
        public string Title;
        public int X, Y, Z;
        public int Direction;

        public int Id { get { return 0x19; } }

        public void ReadPacket(MinecraftStream stream)
        {
            EntityId = stream.ReadInt32();
            Title = stream.ReadString();
            X = stream.ReadInt32();
            Y = stream.ReadInt32();
            Z = stream.ReadInt32();
            Direction = stream.ReadInt32();
        }

        public void WritePacket(MinecraftStream stream)
        {
            stream.WriteInt32(EntityId);
            stream.WriteString(Title);
            stream.WriteInt32(X);
            stream.WriteInt32(Y);
            stream.WriteInt32(Z);
            stream.WriteInt32(Direction);
        }
    }

    public struct SpawnExperienceOrbPacket : IPacket
    {
        public int EntityId;
        public int X, Y, Z;
        public short Count;

        public int Id { get { return 0x1A; } }

        public void ReadPacket(MinecraftStream stream)
        {
            EntityId = stream.ReadInt32();
            X = stream.ReadInt32();
            Y = stream.ReadInt32();
            Z = stream.ReadInt32();
            Count = stream.ReadInt16();
        }

        public void WritePacket(MinecraftStream stream)
        {
            stream.WriteInt32(EntityId);
            stream.WriteInt32(X);
            stream.WriteInt32(Y);
            stream.WriteInt32(Z);
            stream.WriteInt16(Count);
        }
    }

    public struct EntityVelocityPacket : IPacket
    {
        public int EntityId;
        public short VelocityX, VelocityY, VelocityZ;

        public int Id { get { return 0x1C; } }

        public void ReadPacket(MinecraftStream stream)
        {
            EntityId = stream.ReadInt32();
            VelocityX = stream.ReadInt16();
            VelocityY = stream.ReadInt16();
            VelocityZ = stream.ReadInt16();
        }

        public void WritePacket(MinecraftStream stream)
        {
            stream.WriteInt32(EntityId);
            stream.WriteInt32(VelocityX);
            stream.WriteInt32(VelocityY);
            stream.WriteInt32(VelocityZ);
        }
    }

    public struct DestroyEntity : IPacket
    {
        public int[] EntityIds;

        public int Id { get { return 0x1D; } }

        public void ReadPacket(MinecraftStream stream)
        {
            var length = stream.ReadUInt8();
            EntityIds = stream.ReadInt32Array(length);
        }

        public void WritePacket(MinecraftStream stream)
        {
            stream.WriteUInt8((byte)EntityIds.Length);
            stream.WriteInt32Array(EntityIds);
        }
    }

    public struct EntityPacket : IPacket
    {
        public int EntityId;

        public int Id { get { return 0x1E; } }

        public void ReadPacket(MinecraftStream stream)
        {
            EntityId = stream.ReadInt32();
        }

        public void WritePacket(MinecraftStream stream)
        {
            stream.WriteInt32(EntityId);
        }
    }

    public struct EntityRelativeMovePacket : IPacket
    {
        public int EntityId;
        public sbyte DeltaX, DeltaY, DeltaZ;

        public int Id { get { return 0x1F; } }

        public void ReadPacket(MinecraftStream stream)
        {
            EntityId = stream.ReadInt32();
            DeltaX = stream.ReadInt8();
            DeltaY = stream.ReadInt8();
            DeltaZ = stream.ReadInt8();
        }

        public void WritePacket(MinecraftStream stream)
        {
            stream.WriteInt32(EntityId);
            stream.WriteInt8(DeltaX);
            stream.WriteInt8(DeltaY);
            stream.WriteInt8(DeltaZ);
        }
    }

    public struct EntityLookPacket : IPacket
    {
        public int EntityId;
        public byte Yaw, Pitch;

        public int Id { get { return 0x20; } }

        public void ReadPacket(MinecraftStream stream)
        {
            EntityId = stream.ReadInt32();
            Yaw = stream.ReadUInt8();
            Pitch = stream.ReadUInt8();
        }

        public void WritePacket(MinecraftStream stream)
        {
            stream.WriteInt32(EntityId);
            stream.WriteUInt8(Yaw);
            stream.WriteUInt8(Pitch);
        }
    }

    public struct EntityLookAndRelativeMovePacket : IPacket
    {
        public int EntityId;
        public sbyte DeltaX, DeltaY, DeltaZ;
        public byte Yaw, Pitch;

        public int Id { get { return 0x21; } }

        public void ReadPacket(MinecraftStream stream)
        {
            EntityId = stream.ReadInt32();
            DeltaX = stream.ReadInt8();
            DeltaY = stream.ReadInt8();
            DeltaZ = stream.ReadInt8();
            Yaw = stream.ReadUInt8();
            Pitch = stream.ReadUInt8();
        }

        public void WritePacket(MinecraftStream stream)
        {
            stream.WriteInt32(EntityId);
            stream.WriteInt8(DeltaX);
            stream.WriteInt8(DeltaY);
            stream.WriteInt8(DeltaZ);
            stream.WriteUInt8(Yaw);
            stream.WriteUInt8(Pitch);
        }
    }

    public struct EntityTeleportPacket : IPacket
    {
        public int EntityId;
        public int X, Y, Z;
        public byte Yaw, Pitch;

        public int Id { get { return 0x22; } }

        public void ReadPacket(MinecraftStream stream)
        {
            EntityId = stream.ReadInt32();
            X = stream.ReadInt32();
            Y = stream.ReadInt32();
            Z = stream.ReadInt32();
            Yaw = stream.ReadUInt8();
            Pitch = stream.ReadUInt8();
        }

        public void WritePacket(MinecraftStream stream)
        {
            stream.WriteInt32(EntityId);
            stream.WriteInt32(X);
            stream.WriteInt32(Y);
            stream.WriteInt32(Z);
            stream.WriteUInt8(Yaw);
            stream.WriteUInt8(Pitch);
        }
    }

    public struct EntityHeadLook : IPacket
    {
        public int EntityId;
        public byte HeadYaw;

        public int Id { get { return 0x23; } }

        public void ReadPacket(MinecraftStream stream)
        {
            EntityId = stream.ReadInt32();
            HeadYaw = stream.ReadUInt8();
        }

        public void WritePacket(MinecraftStream stream)
        {
            stream.WriteInt32(EntityId);
            stream.WriteUInt8(HeadYaw);
        }
    }

    public struct EntityStatusPacket : IPacket
    {
        public int EntityId;
        public byte Status;

        public int Id { get { return 0x26; } }

        public void ReadPacket(MinecraftStream stream)
        {
            EntityId = stream.ReadInt32();
            Status = stream.ReadUInt8();
        }

        public void WritePacket(MinecraftStream stream)
        {
            stream.WriteInt32(EntityId);
            stream.WriteUInt8(Status);
        }
    }

    public struct AttachEntityPacket : IPacket
    {
        public int EntityId, VehicleId;

        public int Id { get { return 0x27; } }

        public void ReadPacket(MinecraftStream stream)
        {
            EntityId = stream.ReadInt32();
            VehicleId = stream.ReadInt32();
        }

        public void WritePacket(MinecraftStream stream)
        {
            stream.WriteInt32(EntityId);
            stream.WriteInt32(VehicleId);
        }
    }

    public struct EntityMetadataPacket : IPacket
    {
        public int EntityId;
        public MetadataDictionary Metadata;

        public int Id { get { return 0x28; } }

        public void ReadPacket(MinecraftStream stream)
        {
            EntityId = stream.ReadInt32();
            Metadata = MetadataDictionary.FromStream(stream);
        }

        public void WritePacket(MinecraftStream stream)
        {
            stream.WriteInt32(EntityId);
            Metadata.WriteTo(stream);
        }
    }

    public struct EntityEffectPacket : IPacket
    {
        public int EntityId;
        public byte EffectId;
        public byte Amplifier;
        public short Duration;

        public int Id { get { return 0x29; } }

        public void ReadPacket(MinecraftStream stream)
        {
            EntityId = stream.ReadInt32();
            EffectId = stream.ReadUInt8();
            Amplifier = stream.ReadUInt8();
            Duration = stream.ReadInt16();
        }

        public void WritePacket(MinecraftStream stream)
        {
            stream.WriteInt32(EntityId);
            stream.WriteUInt8(EffectId);
            stream.WriteUInt8(Amplifier);
            stream.WriteInt16(Duration);
        }
    }

    public struct RemoveEntityEffect : IPacket
    {
        public int EntityId;
        public byte EffectId;

        public int Id { get { return 0x2A; } }

        public void ReadPacket(MinecraftStream stream)
        {
            EntityId = stream.ReadInt32();
            EffectId = stream.ReadUInt8();
        }

        public void WritePacket(MinecraftStream stream)
        {
            stream.WriteInt32(EntityId);
            stream.WriteUInt8(EffectId);
        }
    }

    public struct SetExperiencePacket : IPacket
    {
        public float ExperienceBar;
        public short Level;
        public short TotalExperience;

        public int Id { get { return 0x2B; } }

        public void ReadPacket(MinecraftStream stream)
        {
            ExperienceBar = stream.ReadSingle();
            Level = stream.ReadInt16();
            TotalExperience = stream.ReadInt16();
        }

        public void WritePacket(MinecraftStream stream)
        {
            stream.WriteSingle(ExperienceBar);
            stream.WriteInt16(Level);
            stream.WriteInt16(TotalExperience);
        }
    }
}
