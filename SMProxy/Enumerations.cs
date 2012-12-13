using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace SMProxy
{
    [Flags]
    public enum GameMode : byte
    {
        Survival = 0,
        Creative = 1,
        Adventure = 2,
        Hardcore = 0x8 // Hardcore is the only flag, and may be seen in combination with the others
    }

    public enum Dimension : sbyte
    {
        Nether = -1,
        Overworld = 0,
        End = 1
    }

    public enum Difficulty : byte
    {
        Peaceful = 0,
        Easy = 1,
        Normal = 2,
        Hard = 3
    }
}
