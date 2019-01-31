using System;
using System.Collections.Generic;
using System.Runtime.InteropServices;
using System.Text;

namespace TZ.BrowserMain.BrowserCore
{
    [StructLayout(LayoutKind.Explicit, Size = 4)]
    public struct BLENDFUNCTION
    {
        [FieldOffset(0)]
        public byte BlendOp;
        [FieldOffset(1)]
        public byte BlendFlags;
        [FieldOffset(2)]
        public byte SourceConstantAlpha;
        [FieldOffset(3)]
        public byte AlphaFormat;
    }
}
