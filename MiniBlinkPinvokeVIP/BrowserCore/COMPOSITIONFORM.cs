using System;
using System.Collections.Generic;
using System.Drawing;
using System.Runtime.InteropServices;
using System.Text;

namespace TZ.BrowserMain.BrowserCore
{
    [StructLayout(LayoutKind.Sequential)]
    public struct COMPOSITIONFORM
    {
        public int dwStyle;
        public POINT ptCurrentPos;
        public RECT rcArea;
    }
    [StructLayout(LayoutKind.Sequential)]
    public struct COMPOSITIONFORMw
    {
        public int dwStyle;
        public Point ptCurrentPos;
        public RECT rcArea;
    }
}
