﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using LibNbt;
using System.IO;
using System.IO.Compression;

namespace SMProxy
{
    public struct Slot
    {
        public Slot(short id)
        {
            Id = id;
            Count = 1;
            Metadata = 0;
            Nbt = null;
        }

        public Slot(short id, sbyte count) : this(id)
        {
            Count = count;
        }

        public Slot(short id, sbyte count, short metadata) : this(id, count)
        {
            Metadata = metadata;
        }

        public Slot(short id, sbyte count, short metadata, NbtFile nbt) : this(id, count, metadata)
        {
            Nbt = nbt;
        }

        public static Slot FromStream(MinecraftStream stream)
        {
            var slot = new Slot();
            slot.Id = stream.ReadInt16();
            if (slot.Empty)
                return slot;
            slot.Count = stream.ReadInt8();
            slot.Metadata = stream.ReadInt16();
            var length = stream.ReadInt16();
            if (length == -1)
                return slot;
            slot.Nbt = new NbtFile(stream, NbtCompression.GZip, null);
            return slot;
        }

        public void WriteTo(MinecraftStream stream)
        {
            stream.WriteInt16(Id);
            if (Empty)
                return;
            stream.WriteInt8(Count);
            stream.WriteInt16(Metadata);
            if (Nbt == null)
                return;
            Nbt.SaveToStream(stream, NbtCompression.GZip);
        }

        public bool Empty
        {
            get { return Id == -1; }
        }

        public short Id;
        public sbyte Count;
        public short Metadata;
        public NbtFile Nbt;
    }
}