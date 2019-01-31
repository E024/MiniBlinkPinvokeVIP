using System;
using System.Collections.Generic;
using System.Runtime.InteropServices;
using System.Text;

namespace TZ.BrowserMain.BrowserCore
{
    [StructLayout(LayoutKind.Sequential)]
    public struct POINT
    {
        public int x;
        public int y;
        public POINT(Int32 x, Int32 y)
        {
            this.x = x;
            this.y = y;
        }
    }
}
