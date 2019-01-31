using System;
using System.Collections.Generic;
using System.Drawing;
using System.Runtime.InteropServices;
using System.Text;

namespace TZ.BrowserMain.BrowserCore
{
    [StructLayout(LayoutKind.Sequential)]
    public struct wkeConsoleMessage
    {
        public wkeMessageSource source;
        public wkeMessageType type;
        public wkeMessageLevel level;
        public IntPtr message;
        public IntPtr url;
        public uint lineNumber;
    }
    [StructLayout(LayoutKind.Sequential)]
    public struct mbNewViewInfo
    {
        public mbNavigationType navigationType;
        public IntPtr url;
        public IntPtr target;
        public int x;
        public int y;
        public int width;
        public int height;
        [MarshalAs(UnmanagedType.I1)]
        public bool menuBarVisible;
        [MarshalAs(UnmanagedType.I1)]
        public bool statusBarVisible;
        [MarshalAs(UnmanagedType.I1)]
        public bool toolBarVisible;
        [MarshalAs(UnmanagedType.I1)]
        public bool locationBarVisible;
        [MarshalAs(UnmanagedType.I1)]
        public bool scrollbarsVisible;
        [MarshalAs(UnmanagedType.I1)]
        public bool resizable;
        [MarshalAs(UnmanagedType.I1)]
        public bool fullscreen;
    }
    [StructLayout(LayoutKind.Sequential)]
    public struct wkeDocumentReadyInfo
    {
        public IntPtr url;

        public IntPtr frameJSState;

        public IntPtr mainFrameJSState;
    }

    [StructLayout(LayoutKind.Sequential)]
    public struct mbProxy
    {
        public mbProxyType type;
        [MarshalAs(UnmanagedType.ByValTStr, SizeConst = 100)]
        public string hostname;
        public ushort port;
        [MarshalAs(UnmanagedType.ByValTStr, SizeConst = 50)]
        public string username;
        [MarshalAs(UnmanagedType.ByValTStr, SizeConst = 50)]
        public string password;
    }
    [StructLayout(LayoutKind.Sequential)]
    public struct mbSettings
    {
        public mbProxy proxy;
        public mbSettingMask mask;
        public IntPtr blinkThreadInitCallback;
        public IntPtr blinkThreadInitCallbackParam;

    }

    [StructLayout(LayoutKind.Sequential)]
    public struct mbWindowFeatures
    {
        public int x;
        public int y;
        public int width;
        public int height;
        [MarshalAs(UnmanagedType.I1)]
        public bool menuBarVisible;
        [MarshalAs(UnmanagedType.I1)]
        public bool statusBarVisible;
        [MarshalAs(UnmanagedType.I1)]
        public bool toolBarVisible;
        [MarshalAs(UnmanagedType.I1)]
        public bool locationBarVisible;
        [MarshalAs(UnmanagedType.I1)]
        public bool scrollbarsVisible;
        [MarshalAs(UnmanagedType.I1)]
        public bool resizable;
        [MarshalAs(UnmanagedType.I1)]
        public bool fullscreen;
    }


    [StructLayout(LayoutKind.Sequential)]
    public struct mbRect
    {
        public int x;
        public int y;
        public int w;
        public int h;
        public Rectangle ToRectangle()
        {
            return new Rectangle(this.x, this.y, this.w, this.h);
        }
    }

    [StructLayout(LayoutKind.Sequential)]
    public struct mbUrlRequestCallbacks
    {
        public mbOnUrlRequestWillRedirectCallback willRedirectCallback;
        public mbOnUrlRequestDidReceiveResponseCallback didReceiveResponseCallback;
        public mbOnUrlRequestDidReceiveDataCallback didReceiveDataCallback;
        public mbOnUrlRequestDidFailCallback didFailCallback;
        public mbOnUrlRequestDidFinishLoadingCallback didFinishLoadingCallback;

        public IntPtr ToIntPtr()
        {
            int size = Marshal.SizeOf(this);
            IntPtr buffer = Marshal.AllocHGlobal(size);

            Marshal.StructureToPtr(this, buffer, false);
            return buffer;
        }
    }
    [StructLayout(LayoutKind.Sequential)]
    public struct mbMemBuf
    {
        public int size;
        public IntPtr data;
        public Int32 length;
    }
    [StructLayout(LayoutKind.Sequential)]
    public struct mbPrintSettings
    {
        public int structSize;
        public int dpi;
        public int width;
        public int height;
        public int marginTop;
        public int marginBottom;
        public int marginLeft;
        public int marginRight;
        [MarshalAs(UnmanagedType.I1)]
        public bool isPrintPageHeadAndFooter;
        [MarshalAs(UnmanagedType.I1)]
        public bool isPrintBackgroud;
    }
    [StructLayout(LayoutKind.Sequential)]
    public struct mbPdfDatas
    {
        public int count;
        public IntPtr sizes;
        public IntPtr datas;
    }
    [StructLayout(LayoutKind.Sequential)]
    public struct mbPdfDatas_SizeArray
    {
        [MarshalAs(UnmanagedType.ByValArray, SizeConst = 100)]
        public int[] Size;
    }
    [StructLayout(LayoutKind.Sequential)]
    public struct mbPdfDatas_DataArray
    {
        [MarshalAs(UnmanagedType.ByValArray, SizeConst = 100)]
        public IntPtr[] Data;
    }
    [StructLayout(LayoutKind.Sequential)]
    public struct mbNetJobDataBind
    {
        public IntPtr ptr;
        public mbNetJobDataRecvCallback recvCallback;
        public mbNetJobDataFinishCallback finishCallback;
    }
    [StructLayout(LayoutKind.Sequential)]
    public struct mbPopupDialogAndDownloadBind
    {
        public IntPtr ptr;
        mbNetJobDataRecvCallback recvCallback;
        mbNetJobDataFinishCallback finishCallback;
        mbPopupDialogSaveNameCallback saveNameCallback;
    }
}
