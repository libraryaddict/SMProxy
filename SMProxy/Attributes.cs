using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace SMProxy
{
    public class LogDisplayAttribute : Attribute
    {
        public LogDisplayType DisplayType { get; set; }

        public LogDisplayAttribute(LogDisplayType displayType)
        {
            DisplayType = displayType;
        }
    }

    public enum LogDisplayType
    {
        Binary,
        Hexadecimal
    }
}
