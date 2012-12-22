using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace SMProxy.Events
{
    public class UnrecognizedArgumentEventArgs : EventArgs
    {
        public string[] Args { get; set; }
        public int Index { get; set; }
        public string Argument { get; set; }
        public bool Handled { get; set; }
    }
}
