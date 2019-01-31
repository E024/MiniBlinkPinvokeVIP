using System;
using System.Collections.Generic;
using System.Runtime.InteropServices;
using System.Text;

namespace TZ.BrowserMain.BrowserCore
{
    [StructLayout(LayoutKind.Sequential)]
    public struct BITMAP
    {
        public int bmType;
        public int bmWidth;
        public int bmHeight;
        public int bmWidthBytes;
        public short bmPlanes;
        public short bmBitsPixel;
        public int bmBits;
    }
}
