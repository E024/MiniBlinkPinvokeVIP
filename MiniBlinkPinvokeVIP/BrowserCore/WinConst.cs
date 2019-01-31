﻿using System;
using System.Collections.Generic;
using System.Text;

namespace TZ.BrowserMain.BrowserCore
{
    public enum WinConst : int
    {
        GWL_EXSTYLE = -20,
        GWL_WNDPROC = -4,
        WS_EX_LAYERED = 524288,
        WM_PAINT = 15,
        WM_ERASEBKGND = 20,
        WM_SIZE = 5,
        WM_KEYDOWN = 256,
        WM_KEYUP = 257,
        WM_CHAR = 258,
        WM_LBUTTONDOWN = 513,
        WM_LBUTTONUP = 514,
        WM_MBUTTONDOWN = 519,
        WM_RBUTTONDOWN = 516,
        WM_LBUTTONDBLCLK = 515,
        WM_MBUTTONDBLCLK = 521,
        WM_RBUTTONDBLCLK = 518,
        WM_MBUTTONUP = 520,
        WM_RBUTTONUP = 517,
        WM_MOUSEMOVE = 512,
        WM_CONTEXTMENU = 123,
        WM_MOUSEWHEEL = 522,
        WM_SETFOCUS = 7,
        WM_KILLFOCUS = 8,
        //WM_IME_STARTCOMPOSITION = 269,
        WM_NCHITTEST = 132,
        WM_GETMINMAXINFO = 36,
        WM_DESTROY = 2,
        WM_SETCURSOR = 32,
        MK_CONTROL = 8,
        MK_SHIFT = 4,
        MK_LBUTTON = 1,
        MK_MBUTTON = 16,
        MK_RBUTTON = 2,
        KF_REPEAT = 16384,
        KF_EXTENDED = 256,
        SRCCOPY = 13369376,
        CAPTUREBLT = 1073741824,
        CFS_POINT = 2,
        CFS_FORCE_POSITION = 32,
        OBJ_BITMAP = 7,
        AC_SRC_OVER = 0,
        AC_SRC_ALPHA = 1,
        ULW_ALPHA = 2,
        WM_INPUTLANGCHANGE = 81,
        WM_NCDESTROY = 130,
        WM_SETCURSOR_ASYN = 0x5327,
        WM_IME_STARTCOMPOSITION_ASYN = 0x5326,
        WM_IME_STARTCOMPOSITION = 0x10d,
        WM_IME_ENDCOMPOSITION = 0x10e,
        WM_IME_SETCONTEXT = 0x0281,

    }

}
