using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace SMProxy
{
    /// <summary>
    /// A big-endian stream for reading/writing Minecraft data types.
    /// </summary>
    public partial class MinecraftStream
    {
        public byte ReadUInt8()
        {
            return (byte)base.ReadByte();
        }

        public void WriteUInt8(byte value)
        {
            Write(new[] { value }, 0, 1);
        }

        public sbyte ReadInt8()
        {
            return (sbyte)ReadUInt8();
        }

        public void WriteInt8(sbyte value)
        {
            WriteUInt8((byte)value);
        }

        public ushort ReadUInt16()
        {
            return (ushort)(
                (ReadUInt8() << 8) |
                ReadUInt8());
        }

        public void WriteUInt16(ushort value)
        {
            Write(new[]
            {
                (byte)(value & 0xFF00 >> 8),
                (byte)(value & 0xFF)
            }, 0, 2);
        }

        public short ReadInt16()
        {
            return (short)ReadUInt16();
        }

        public void WriteInt16(short value)
        {
            WriteUInt16((ushort)value);
        }

        public uint ReadUInt32()
        {
            return (uint)(
                (ReadUInt8() << 24) |
                (ReadUInt8() << 16) |
                (ReadUInt8() << 8 ) |
                ReadUInt8());
        }

        public void WriteUInt32(uint value)
        {
            Write(new[]
            {
                (byte)(value & 0xFF000000 >> 24),
                (byte)(value & 0xFF0000 >> 16),
                (byte)(value & 0xFF00 >> 8),
                (byte)(value & 0xFF)
            }, 0, 4);
        }

        public int ReadInt32()
        {
            return (int)ReadUInt32();
        }

        public void WriteInt32(int value)
        {
            WriteUInt32((uint)value);
        }

        public ulong ReadUInt64()
        {
            return (ulong)(
                (ReadUInt8() << 56) |
                (ReadUInt8() << 48) |
                (ReadUInt8() << 40) |
                (ReadUInt8() << 32) |
                (ReadUInt8() << 24) |
                (ReadUInt8() << 16) |
                (ReadUInt8() << 8) |
                ReadUInt8());
        }

        public void WriteUInt64(ulong value)
        {
            Write(new[]
            {
                (byte)(value & 0xFF00000000000000 >> 56),
                (byte)(value & 0xFF000000000000 >> 48),
                (byte)(value & 0xFF0000000000 >> 40),
                (byte)(value & 0xFF00000000 >> 32),
                (byte)(value & 0xFF000000 >> 24),
                (byte)(value & 0xFF0000 >> 16),
                (byte)(value & 0xFF00 >> 8),
                (byte)(value & 0xFF)
            }, 0, 8);
        }

        public long ReadInt64()
        {
            return (long)ReadUInt64();
        }

        public void WriteInt64(long value)
        {
            WriteUInt64((ulong)value);
        }
    }
}
