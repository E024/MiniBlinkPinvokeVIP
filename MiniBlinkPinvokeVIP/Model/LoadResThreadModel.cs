using System;
using System.Collections.Generic;
using System.Text;

namespace MiniBlinkPinvokeVIP.Model
{
    public class LoadResThreadModel
    {
        public string Url { get; set; }
        public string Cookie { get; set; }
        public int Type { get; set; }
        public object MB { get; set; }
        public IntPtr NetJob { get; set; }
    }
}
