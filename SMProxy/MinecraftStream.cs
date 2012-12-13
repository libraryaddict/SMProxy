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
        static MinecraftStream()
        {
            StringEncoding = Encoding.BigEndianUnicode;
        }

        public static Encoding StringEncoding;

        public byte ReadUInt8()
        {
            return (byte)base.ReadByte();
        }

        public void WriteUInt8(byte value)
        {
            WriteByte(value);
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

        public byte[] ReadUInt8Array(int length)
        {
            var buffer = new byte[length];
            Read(buffer, 0, length);
            return buffer;
        }

        public void WriteUInt8Array(byte[] value)
        {
            Write(value, 0, value.Length);
        }

        public sbyte[] ReadInt8Array(int length)
        {
            return (sbyte[])(Array)ReadUInt8Array(length);
        }

        public void WriteInt8Array(sbyte[] value)
        {
            Write((byte[])(Array)value, 0, value.Length);
        }

        public ushort[] ReadUInt16Array(int length)
        {
            var result = new ushort[length];
            for (int i = 0; i < length; i++)
                result[i] = ReadUInt16();
            return result;
        }

        public void WriteUInt16Array(ushort[] value)
        {
            for (int i = 0; i < value.Length; i++)
                WriteUInt16(value[i]);
        }

        public short[] ReadInt16Array(int length)
        {
            return (short[])(Array)ReadUInt16Array(length);
        }

        public void WriteInt16Array(short[] value)
        {
            WriteUInt16Array((ushort[])(Array)value);
        }

        public uint[] ReadUInt32Array(int length)
        {
            var result = new uint[length];
            for (int i = 0; i < length; i++)
                result[i] = ReadUInt32();
            return result;
        }

        public void WriteUInt32Array(uint[] value)
        {
            for (int i = 0; i < value.Length; i++)
                WriteUInt32(value[i]);
        }

        public int[] ReadInt32Array(int length)
        {
            return (int[])(Array)ReadUInt32Array(length);
        }

        public void WriteInt32Array(int[] value)
        {
            WriteUInt32Array((uint[])(Array)value);
        }

        public ulong[] ReadUInt64Array(int length)
        {
            var result = new ulong[length];
            for (int i = 0; i < length; i++)
                result[i] = ReadUInt64();
            return result;
        }

        public void WriteUInt64Array(ulong[] value)
        {
            for (int i = 0; i < value.Length; i++)
                WriteUInt64(value[i]);
        }

        public long[] ReadInt64Array(int length)
        {
            return (long[])(Array)ReadUInt64Array(length);
        }

        public void WriteInt64Array(long[] value)
        {
            WriteUInt64Array((ulong[])(Array)value);
        }

        public unsafe float ReadSingle()
        {
            int value = ReadInt32();
            return *(float*)&value;
        }

        public unsafe void WriteSingle(float value)
        {
            WriteInt32(*(int*)&value);
        }

        public unsafe double ReadDouble()
        {
            long value = ReadInt64();
            return *(double*)&value;
        }

        public unsafe void WriteDouble(double value)
        {
            WriteInt64(*(long*)&value);
        }

        public string ReadString()
        {
            ushort length = ReadUInt16();
            var data = ReadUInt8Array(length * 2);
            return StringEncoding.GetString(data);
        }

        public void WriteString(string value)
        {
            WriteUInt16((ushort)value.Length);
            WriteUInt8Array(StringEncoding.GetBytes(value));
        }
    }
}
