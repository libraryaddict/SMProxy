using System;
using System.IO;
using LibNbt;

namespace SMProxy.Metadata
{
    public class MetadataSlot : MetadataEntry
    {
        public override byte Identifier { get { return 5; } }
        public override string FriendlyName { get { return "slot"; } }

        public Slot Value;

        public MetadataSlot(byte index) : base(index)
        {
        }

        public MetadataSlot(byte index, Slot value) : base(index)
        {
            Value = value;
        }

        public override bool TryReadEntry(byte[] buffer, ref int offset)
        {
            if (buffer.Length - offset < 6)
                return false;
            offset++;
            short id, metadata;
            byte count;
            if (!DataUtility.TryReadInt16(buffer, ref offset, out id))
                return false;
            if (id == -1)
                return true;
            if (!DataUtility.TryReadByte(buffer, ref offset, out count))
                return false;
            if (!DataUtility.TryReadInt16(buffer, ref offset, out metadata))
                return false;
            Value = new Slot(id, (sbyte)count, metadata);
            return true;
        }

        public override void FromStream(MinecraftStream stream)
        {
            // TODO: Confirm this works properly for metadata slots
            Value = Slot.FromStream(stream);
        }

        public override byte[] Encode()
        {
            byte[] data = new byte[]
            {
                GetKey(),
                0, 0,
                (byte)Value.Count,
                0, 0,
                0, Value.Nbt != null && Value.Nbt.RootTag != null ? (byte)1 : (byte)0
            };
            Array.Copy(DataUtility.CreateInt16(Value.Id), 0, data, 1, 2);
            Array.Copy(DataUtility.CreateInt16(Value.Metadata), 0, data, 4, 2);
            if (Value.Nbt != null && Value.Nbt.RootTag != null)
            {
                MemoryStream ms = new MemoryStream();
                Value.Nbt.SaveToStream(ms, NbtCompression.None);
                var nbt = ms.GetBuffer();
                Array.Resize(ref data, data.Length + nbt.Length);
                Array.Copy(nbt, 0, data, 8, nbt.Length);
            }
            return data;
        }
    }
}
