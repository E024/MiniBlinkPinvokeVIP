using System;
using System.Collections.Generic;
using System.Runtime.InteropServices;
using System.Text;

namespace TZ.BrowserMain.BrowserCore
{
    [UnmanagedFunctionPointer(CallingConvention.StdCall)]
    public delegate void mbRunJsCallback(IntPtr webView, IntPtr param, IntPtr es, Int64 v);

    [UnmanagedFunctionPointer(CallingConvention.StdCall, CharSet = CharSet.Auto)]
    public delegate void mbJsQueryCallback(IntPtr webView, IntPtr param, IntPtr es, Int64 queryId, int customMsg, IntPtr request);

    [UnmanagedFunctionPointer(CallingConvention.StdCall)]
    public delegate void mbTitleChangedCallback(IntPtr webView, IntPtr param, IntPtr title);

    [UnmanagedFunctionPointer(CallingConvention.StdCall)]
    public delegate void mbURLChangedCallback(IntPtr webView, IntPtr param, IntPtr url, bool canGoBack, bool canGoForward);

    [UnmanagedFunctionPointer(CallingConvention.StdCall)]
    public delegate void mbURLChangedCallback2(IntPtr webView, IntPtr param, IntPtr frameId, IntPtr url);

    [UnmanagedFunctionPointer(CallingConvention.StdCall)]
    public delegate void mbPaintUpdatedCallback(IntPtr webView, IntPtr param, IntPtr hdc, int x, int y, int cx, int cy);

    [UnmanagedFunctionPointer(CallingConvention.StdCall)]
    public delegate void mbPaintBitUpdatedCallback(IntPtr webView, IntPtr param, IntPtr buffer, mbRect r, int width, int height);

    [UnmanagedFunctionPointer(CallingConvention.StdCall)]
    public delegate void mbAlertBoxCallback(IntPtr webView, IntPtr param, IntPtr msg);

    [UnmanagedFunctionPointer(CallingConvention.StdCall)]
    public delegate bool mbConfirmBoxCallback(IntPtr webView, IntPtr param, IntPtr msg);

    [UnmanagedFunctionPointer(CallingConvention.StdCall)]
    public delegate IntPtr mbPromptBoxCallback(IntPtr webView, IntPtr param, IntPtr msg, IntPtr defaultResult);

    [UnmanagedFunctionPointer(CallingConvention.StdCall)]
    public delegate bool mbNavigationCallback(IntPtr webView, IntPtr param, mbNavigationType navigationType, IntPtr url);

    [UnmanagedFunctionPointer(CallingConvention.StdCall)]
    public delegate IntPtr mbCreateViewCallback(IntPtr webView, IntPtr param, mbNavigationType navigationType, IntPtr url, IntPtr windowFeatures);

    [UnmanagedFunctionPointer(CallingConvention.StdCall)]
    public delegate void mbDocumentReadyCallback(IntPtr webView, IntPtr param, IntPtr frameId);

    [UnmanagedFunctionPointer(CallingConvention.StdCall)]
    public delegate void mbOnShowDevtoolsCallback(IntPtr webView, IntPtr param);

    [UnmanagedFunctionPointer(CallingConvention.StdCall)]
    public delegate void mbLoadingFinishCallback(IntPtr webView, IntPtr param, IntPtr frameId, IntPtr url, mbLoadingResult result, IntPtr failedReason);

    [return: MarshalAs(UnmanagedType.I1)]
    [UnmanagedFunctionPointer(CallingConvention.StdCall)]
    public delegate bool mbDownloadCallback(IntPtr webView, IntPtr param, IntPtr frameId, IntPtr url, IntPtr downloadJob);



    [UnmanagedFunctionPointer(CallingConvention.StdCall)]
    public delegate void mbConsoleCallback(IntPtr webView, IntPtr param, mbConsoleLevel level, IntPtr message, IntPtr sourceName, int sourceLine, IntPtr stackTrace);

    [UnmanagedFunctionPointer(CallingConvention.StdCall)]
    public delegate void mbOnCallUiThread(IntPtr webView, IntPtr paramOnInThread);

    [UnmanagedFunctionPointer(CallingConvention.StdCall)]
    public delegate void mbCallUiThread(IntPtr webView, mbOnCallUiThread func, IntPtr param);

    //mbNet--------------------------------------------------------------------------------------
    [UnmanagedFunctionPointer(CallingConvention.StdCall)]
    public delegate bool mbLoadUrlBeginCallback(IntPtr webView, IntPtr param, [MarshalAs(UnmanagedType.LPStr)] string url, IntPtr job);
    [UnmanagedFunctionPointer(CallingConvention.StdCall)]
    public delegate void mbLoadUrlEndCallback(IntPtr webView, IntPtr param, [MarshalAs(UnmanagedType.LPStr)] string url, IntPtr job, IntPtr buf, int len);
    [UnmanagedFunctionPointer(CallingConvention.StdCall)]
    public delegate void mbCanGoBackForwardCallback(IntPtr webView, IntPtr param, MbAsynRequestState state, bool b);
    [UnmanagedFunctionPointer(CallingConvention.StdCall)]
    public delegate void mbGetCookieCallback(IntPtr webView, IntPtr param, MbAsynRequestState state, IntPtr cookie);
    [UnmanagedFunctionPointer(CallingConvention.StdCall)]
    public delegate void mbGetMHTMLCallback(IntPtr webView, IntPtr param, IntPtr mhtml);

    [UnmanagedFunctionPointer(CallingConvention.StdCall)]
    public delegate void mbOnUrlRequestWillRedirectCallback(IntPtr webView, IntPtr param, IntPtr oldRequest, IntPtr request, IntPtr redirectResponse);
    [UnmanagedFunctionPointer(CallingConvention.StdCall)]
    public delegate void mbOnUrlRequestDidReceiveResponseCallback(IntPtr webView, IntPtr param, IntPtr request, IntPtr response);
    [UnmanagedFunctionPointer(CallingConvention.StdCall)]
    public delegate void mbOnUrlRequestDidReceiveDataCallback(IntPtr webView, IntPtr param, IntPtr request, IntPtr data, int dataLength);
    [UnmanagedFunctionPointer(CallingConvention.StdCall)]
    public delegate void mbOnUrlRequestDidFailCallback(IntPtr webView, IntPtr param, IntPtr request, IntPtr error);
    [UnmanagedFunctionPointer(CallingConvention.StdCall)]
    public delegate void mbOnUrlRequestDidFinishLoadingCallback(IntPtr webView, IntPtr param, IntPtr request, double finishTime);
    [UnmanagedFunctionPointer(CallingConvention.StdCall)]
    public delegate void mbNetGetFaviconCallback(IntPtr webView, IntPtr param, IntPtr url, IntPtr buf);
    [UnmanagedFunctionPointer(CallingConvention.StdCall)]
    public delegate void mbOnBlinkThreadInitCallback(IntPtr param);
    [UnmanagedFunctionPointer(CallingConvention.StdCall)]
    public delegate void mbPrintPdfDataCallback(IntPtr webview, IntPtr param, mbPdfDatas datas);
    [UnmanagedFunctionPointer(CallingConvention.StdCall)]
    public delegate void mbNetJobDataRecvCallback(IntPtr ptr, IntPtr job, IntPtr data, int length);
    [UnmanagedFunctionPointer(CallingConvention.StdCall)]
    public delegate void mbNetJobDataFinishCallback(IntPtr ptr, IntPtr job, mbLoadingResult result);
    [UnmanagedFunctionPointer(CallingConvention.StdCall)]
    public delegate void mbPopupDialogSaveNameCallback(IntPtr ptr, IntPtr filePath);
}
