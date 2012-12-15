using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;

namespace SMProxy
{
    class Program
    {
        static void Main(string[] args)
        {
            
        }

        private static void CreatePacketArray()
        {
            // For generating the PacketReader array dynamically.
            string array = "";
            var types = Assembly.GetExecutingAssembly().GetTypes().Where(
                t => typeof(IPacket).IsAssignableFrom(t) && !t.IsInterface && !t.IsAbstract);
            for (int i = 0; i < 256; i++)
            {
                var type = types.FirstOrDefault(t => ((IPacket)Activator.CreateInstance(t)).Id == i);
                if (type == null)
                    array += "null,";
                else
                    array += "typeof(" + type.Name + "),";
                array += " // 0x" + i.ToString("X2") + Environment.NewLine;
            }
        }
    }
}
