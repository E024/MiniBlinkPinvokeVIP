using System;
using System.Collections.Generic;
using System.Reflection;
using System.Runtime.InteropServices;
using System.Text;

namespace TZ.BrowserMain.BrowserCore
{
    public class BlinkPInvoke
    {
        const string BlinkBrowserdll = "mb.dll";

        public static int mainThreadId;
        /// <summary>
        /// 判断当前是否是主线程
        /// </summary>
        public static bool IsMainThread
        {
            get { return System.Threading.Thread.CurrentThread.ManagedThreadId == mainThreadId; }
        }
        private static Dictionary<string, Assembly> _ResourceAssemblys = new Dictionary<string, Assembly>();
        public static Dictionary<string, Assembly> ResourceAssemblys
        {
            get
            {
                return _ResourceAssemblys;
            }
        }
        /// <summary>
        /// 初始化
        /// </summary>
        /// <param name="settings"></param>
        [DllImport(BlinkBrowserdll, CallingConvention = CallingConvention.StdCall)]
        public static extern void mbInit(IntPtr settings);
        public static void mbInitWrap(mbSettings settings)
        {
            int nSizeOfSettings = Marshal.SizeOf(settings);
            IntPtr intPtr = Marshal.AllocHGlobal(nSizeOfSettings);
            Marshal.StructureToPtr(settings, intPtr, true);
            mbInit(intPtr);
            Marshal.DestroyStructure(intPtr, typeof(mbSettings));
        }
        /// <param name="settings"></param>
        [DllImport(BlinkBrowserdll, CallingConvention = CallingConvention.StdCall)]
        public static extern void mbUninit(IntPtr settings);
        [DllImport(BlinkBrowserdll, CallingConvention = CallingConvention.StdCall)]
        public static extern void mbDestroyWebView(IntPtr webView);
        /// <summary>
        /// 创建Windows窗体
        /// </summary>
        /// <returns></returns>
        [DllImport(BlinkBrowserdll, CallingConvention = CallingConvention.StdCall)]
        public static extern IntPtr mbCreateWebView();
        /// <summary>
        /// hook 请求还没发生时，设置数据。
        /// </summary>
        /// <param name="job"></param>
        /// <param name="buf"></param>
        /// <param name="len"></param>

        [DllImport(BlinkBrowserdll, CallingConvention = CallingConvention.StdCall)]
        public static extern void mbNetSetData(IntPtr job, byte[] buf, int len);
        /// <summary>
        /// hook 请求
        /// </summary>
        /// <param name="job"></param>

        [DllImport(BlinkBrowserdll, CallingConvention = CallingConvention.StdCall)]
        public static extern void mbNetHookRequest(IntPtr job);
        /// <summary>
        /// 是否新开窗口
        /// </summary>
        /// <param name="webView"></param>
        /// <param name="Enable"></param>
        [DllImport(BlinkBrowserdll, CallingConvention = CallingConvention.StdCall)]
        public static extern void mbSetNavigationToNewWindowEnable(IntPtr webView, [MarshalAs(UnmanagedType.I1)] bool Enable);
        /// <summary>
        /// 可以关闭渲染 无头模式
        /// </summary>
        /// <param name="webView"></param>
        /// <param name="Enable"></param>
        [DllImport(BlinkBrowserdll, CallingConvention = CallingConvention.StdCall)]
        public static extern void mbSetHeadlessEnabled(IntPtr webView, [MarshalAs(UnmanagedType.I1)] bool Enable);
        /// <summary>
        /// 设置页面控件的偏移量，一般用作控件时，需要制定位置，否则会出现事件响应位置有问题。
        /// </summary>
        /// <param name="webView"></param>
        /// <param name="x"></param>
        /// <param name="y"></param>
        [DllImport(BlinkBrowserdll, CallingConvention = CallingConvention.StdCall)]
        public static extern void mbSetHandleOffset(IntPtr webView, int x, int y);
        [DllImport(BlinkBrowserdll, CallingConvention = CallingConvention.StdCall)]
        public static extern IntPtr mbGetHostHWND(IntPtr webView);
        /// <summary>
        /// 设置句柄
        /// </summary>
        /// <param name="webView"></param>
        /// <param name="wnd"></param>
        [DllImport(BlinkBrowserdll, CallingConvention = CallingConvention.StdCall)]
        public static extern void mbSetHandle(IntPtr webView, IntPtr wnd);
        /// <summary>
        /// 是否检查CSP，也就是跨域安全检查
        /// </summary>
        /// <param name="webView"></param>
        /// <param name="Enable"></param>
        [DllImport(BlinkBrowserdll, CallingConvention = CallingConvention.StdCall)]
        public static extern void mbSetCspCheckEnable(IntPtr webView, [MarshalAs(UnmanagedType.I1)] bool Enable);

        /// <summary>
        /// 是否启用内存缓存
        /// </summary>
        /// <param name="webView"></param>
        /// <param name="Enable"></param>
        [DllImport(BlinkBrowserdll, CallingConvention = CallingConvention.StdCall)]
        public static extern void mbSetMemoryCacheEnable(IntPtr webView, [MarshalAs(UnmanagedType.I1)] bool Enable);
        /// <summary>
        /// 是否可关闭拖拽到其他进程,True 禁止拖拽，false 启用拖拽
        /// </summary>
        /// <param name="webView"></param>
        /// <param name="Enable"></param>
        [DllImport(BlinkBrowserdll, CallingConvention = CallingConvention.StdCall)]
        public static extern void mbSetDragDropEnable(IntPtr webView, [MarshalAs(UnmanagedType.I1)] bool Enable);

        /// <summary>
        /// 启用或禁用Cookie
        /// </summary>
        /// <param name="webView"></param>
        /// <param name="enable"></param>
        [DllImport(BlinkBrowserdll, CallingConvention = CallingConvention.StdCall)]
        public static extern void mbSetCookieEnabled(IntPtr webView, [MarshalAs(UnmanagedType.I1)] bool enable);
        /// <summary>
        /// 启用或禁用 NPAPI插件,启用的话，在软件运行目录增加 plugins 目录
        /// </summary>
        /// <param name="webView"></param>
        /// <param name="enable"></param>
        [DllImport(BlinkBrowserdll, CallingConvention = CallingConvention.StdCall)]
        public static extern void mbSetNpapiPluginsEnabled(IntPtr webView, [MarshalAs(UnmanagedType.I1)] bool enable);
        /// <summary>
        /// 设置Cookie路径，不包含最终文件名
        /// </summary>
        /// <param name="webView"></param>
        /// <param name="path"></param>

        [DllImport(BlinkBrowserdll, CallingConvention = CallingConvention.StdCall)]
        public static extern void mbSetCookieJarPath(IntPtr webView, [MarshalAs(UnmanagedType.LPWStr)] [In] string path);
        /// <summary>
        /// 设置Cookie路径，包含最终文件名
        /// </summary>
        /// <param name="webView"></param>
        /// <param name="path"></param>
        [DllImport(BlinkBrowserdll, CallingConvention = CallingConvention.StdCall)]
        public static extern void mbSetCookieJarFullPath(IntPtr webView, [MarshalAs(UnmanagedType.LPWStr)] [In] string path);
        /// <summary>
        /// 设置 LocalStorage 路径
        /// </summary>
        /// <param name="webView"></param>
        /// <param name="path"></param>
        [DllImport(BlinkBrowserdll, CallingConvention = CallingConvention.StdCall)]
        public static extern void mbSetLocalStorageFullPath(IntPtr webView, [MarshalAs(UnmanagedType.LPWStr)] [In] string path);
        /// <summary>
        /// 设置浏览器User-Agent
        /// </summary>
        /// <param name="handle"></param>
        /// <param name="userAgent"></param>
        [DllImport(BlinkBrowserdll, CallingConvention = CallingConvention.StdCall)]
        public static extern void mbSetUserAgent(IntPtr handle, IntPtr userAgent);

        /// <summary>
        /// 单独设置资源回收间隔
        /// </summary>
        /// <param name="webView"></param>
        /// <param name="delayMs"></param>
        [DllImport(BlinkBrowserdll, CallingConvention = CallingConvention.StdCall)]
        public static extern void mbSetResourceGc(IntPtr webView, int intervalSec);
        /// <summary>
        /// 改变大小
        /// </summary>
        /// <param name="webView"></param>
        /// <param name="w"></param>
        /// <param name="h"></param>

        [DllImport(BlinkBrowserdll, CallingConvention = CallingConvention.StdCall)]
        public static extern void mbResize(IntPtr webView, int w, int h);
        /// <summary>
        /// 开始加载或跳转链接时发生
        /// </summary>
        /// <param name="webView"></param>
        /// <param name="callback"></param>
        /// <param name="param"></param>

        [DllImport(BlinkBrowserdll, CallingConvention = CallingConvention.StdCall)]
        public static extern void mbOnNavigation(IntPtr webView, mbNavigationCallback callback, IntPtr param);
        /// <summary>
        /// 创建WebView时的回调
        /// </summary>
        /// <param name="webView"></param>
        /// <param name="callback"></param>
        /// <param name="param"></param>
        [DllImport(BlinkBrowserdll, CallingConvention = CallingConvention.StdCall)]
        public static extern void mbOnCreateView(IntPtr webView, mbCreateViewCallback callback, IntPtr param);
        /// <summary>
        /// 文档加载完成时回调(包含iframe)
        /// </summary>
        /// <param name="webView"></param>
        /// <param name="callback"></param>
        /// <param name="param"></param>
        [DllImport(BlinkBrowserdll, CallingConvention = CallingConvention.StdCall)]
        public static extern void mbOnDocumentReady(IntPtr webView, mbDocumentReadyCallback callback, IntPtr param);
        /// <summary>
        /// WebView 需要重新绘制时回调
        /// </summary>
        /// <param name="webView"></param>
        /// <param name="callback"></param>
        /// <param name="callbackParam"></param>
        [DllImport(BlinkBrowserdll, CallingConvention = CallingConvention.StdCall)]
        public static extern void mbOnPaintUpdated(IntPtr webView, mbPaintUpdatedCallback callback, IntPtr callbackParam);
        /// <summary>
        /// 加载地址时回调
        /// </summary>
        /// <param name="webView"></param>
        /// <param name="callback"></param>
        /// <param name="callbackParam"></param>
        [DllImport(BlinkBrowserdll, CallingConvention = CallingConvention.StdCall)]
        public static extern void mbOnLoadUrlBegin(IntPtr webView, mbLoadUrlBeginCallback callback, IntPtr callbackParam);
        /// <summary>
        /// URL加载完成后回调
        /// </summary>
        /// <param name="webView"></param>
        /// <param name="callback"></param>
        /// <param name="callbackParam"></param>
        [DllImport(BlinkBrowserdll, CallingConvention = CallingConvention.StdCall)]
        public static extern void mbOnLoadUrlEnd(IntPtr webView, mbLoadUrlEndCallback callback, IntPtr callbackParam);
        /// <summary>
        /// 页面Title属性发生改变时回调
        /// </summary>
        /// <param name="webView"></param>
        /// <param name="callback"></param>
        /// <param name="callbackParam"></param>
        [DllImport(BlinkBrowserdll, CallingConvention = CallingConvention.StdCall)]
        public static extern void mbOnTitleChanged(IntPtr webView, mbTitleChangedCallback callback, IntPtr callbackParam);
        /// <summary>
        /// 页面URL发生改变时回调
        /// </summary>
        /// <param name="webView"></param>
        /// <param name="callback"></param>
        /// <param name="callbackParam"></param>
        [DllImport(BlinkBrowserdll, CallingConvention = CallingConvention.StdCall)]
        public static extern void mbOnURLChanged(IntPtr webView, mbURLChangedCallback callback, IntPtr callbackParam);
        /// <summary>
        /// 页面资源加载完成时回调，会晚于DocumentReady回调
        /// </summary>
        /// <param name="webView"></param>
        /// <param name="callback"></param>
        /// <param name="param"></param>
        [DllImport(BlinkBrowserdll, CallingConvention = CallingConvention.StdCall)]
        public static extern void mbOnLoadingFinish(IntPtr webView, mbLoadingFinishCallback callback, IntPtr param);
        /// <summary>
        /// 页面后退
        /// </summary>
        /// <param name="webView"></param>
        [DllImport(BlinkBrowserdll, CallingConvention = CallingConvention.StdCall)]
        public static extern void mbGoBack(IntPtr webView);
        /// <summary>
        /// 页面前进
        /// </summary>
        /// <param name="webView"></param>
        [DllImport(BlinkBrowserdll, CallingConvention = CallingConvention.StdCall)]
        public static extern void mbGoForward(IntPtr webView);
        /// <summary>
        /// 强制停止加载页面
        /// </summary>
        /// <param name="webView"></param>
        [DllImport(BlinkBrowserdll, CallingConvention = CallingConvention.StdCall)]
        public static extern void mbStopLoading(IntPtr webView);
        /// <summary>
        /// 重新加载页面（刷新）
        /// </summary>
        /// <param name="webView"></param>
        [DllImport(BlinkBrowserdll, CallingConvention = CallingConvention.StdCall)]
        public static extern void mbReload(IntPtr webView);
        /// <summary>
        /// 执行Cookie 操作命令
        /// </summary>
        /// <param name="webView"></param>
        /// <param name="command"></param>
        [DllImport(BlinkBrowserdll, CallingConvention = CallingConvention.StdCall)]
        public static extern void mbPerformCookieCommand(IntPtr webView, mbCookieCommand command);
        /// <summary>
        /// 全选
        /// </summary>
        /// <param name="webView"></param>
        [DllImport(BlinkBrowserdll, CallingConvention = CallingConvention.StdCall)]
        public static extern void mbEditorSelectAll(IntPtr webView);
        /// <summary>
        /// 复制
        /// </summary>
        /// <param name="webView"></param>
        [DllImport(BlinkBrowserdll, CallingConvention = CallingConvention.StdCall)]
        public static extern void mbEditorCopy(IntPtr webView);
        /// <summary>
        /// 剪切
        /// </summary>
        /// <param name="webView"></param>
        [DllImport(BlinkBrowserdll, CallingConvention = CallingConvention.StdCall)]
        public static extern void mbEditorCut(IntPtr webView);
        /// <summary>
        /// 粘贴
        /// </summary>
        /// <param name="webView"></param>
        [DllImport(BlinkBrowserdll, CallingConvention = CallingConvention.StdCall)]
        public static extern void mbEditorPaste(IntPtr webView);
        /// <summary>
        /// 删除
        /// </summary>
        /// <param name="webView"></param>
        [DllImport(BlinkBrowserdll, CallingConvention = CallingConvention.StdCall)]
        public static extern void mbEditorDelete(IntPtr webView);
        /// <summary>
        /// 发送鼠标事件
        /// </summary>
        /// <param name="webView"></param>
        /// <param name="message"></param>
        /// <param name="x"></param>
        /// <param name="y"></param>
        /// <param name="flags"></param>
        /// <returns></returns>
        [return: MarshalAs(UnmanagedType.I1)]
        [DllImport(BlinkBrowserdll, CallingConvention = CallingConvention.StdCall)]
        public static extern bool mbFireMouseEvent(IntPtr webView, uint message, int x, int y, uint flags);
        /// <summary>
        /// 触发右键事件
        /// </summary>
        /// <param name="webView"></param>
        /// <param name="x"></param>
        /// <param name="y"></param>
        /// <param name="flags"></param>
        /// <returns></returns>
        [return: MarshalAs(UnmanagedType.I1)]
        [DllImport(BlinkBrowserdll, CallingConvention = CallingConvention.StdCall)]
        public static extern bool mbFireContextMenuEvent(IntPtr webView, int x, int y, uint flags);
        /// <summary>
        /// 触发鼠标滚轮事件
        /// </summary>
        /// <param name="webView"></param>
        /// <param name="x"></param>
        /// <param name="y"></param>
        /// <param name="delta"></param>
        /// <param name="flags"></param>
        /// <returns></returns>
        [return: MarshalAs(UnmanagedType.I1)]
        [DllImport(BlinkBrowserdll, CallingConvention = CallingConvention.StdCall)]
        public static extern bool mbFireMouseWheelEvent(IntPtr webView, int x, int y, int delta, uint flags);
        /// <summary>
        /// 触发按键抬起事件
        /// </summary>
        /// <param name="webView"></param>
        /// <param name="virtualKeyCode"></param>
        /// <param name="flags"></param>
        /// <param name="systemKey"></param>
        /// <returns></returns>
        [DllImport(BlinkBrowserdll, CallingConvention = CallingConvention.StdCall)]
        [return: MarshalAs(UnmanagedType.I1)]
        public static extern bool mbFireKeyUpEvent(IntPtr webView, uint virtualKeyCode, uint flags, [MarshalAs(UnmanagedType.I1)] bool systemKey);
        /// <summary>
        /// 触发按键按下事件
        /// </summary>
        /// <param name="webView"></param>
        /// <param name="virtualKeyCode"></param>
        /// <param name="flags"></param>
        /// <param name="systemKey"></param>
        /// <returns></returns>
        [return: MarshalAs(UnmanagedType.I1)]
        [DllImport(BlinkBrowserdll, CallingConvention = CallingConvention.StdCall)]
        public static extern bool mbFireKeyDownEvent(IntPtr webView, uint virtualKeyCode, uint flags, [MarshalAs(UnmanagedType.I1)] bool systemKey);
        /// <summary>
        /// 触发按键释放事件
        /// </summary>
        /// <param name="webView"></param>
        /// <param name="charCode"></param>
        /// <param name="flags"></param>
        /// <param name="systemKey"></param>
        /// <returns></returns>
        [DllImport(BlinkBrowserdll, CallingConvention = CallingConvention.StdCall)]
        [return: MarshalAs(UnmanagedType.I1)]
        public static extern bool mbFireKeyPressEvent(IntPtr webView, uint charCode, uint flags, [MarshalAs(UnmanagedType.I1)] bool systemKey);
        /// <summary>
        /// 发送WINDOWS消息
        /// </summary>
        /// <param name="webView"></param>
        /// <param name="hWnd"></param>
        /// <param name="message"></param>
        /// <param name="wParam"></param>
        /// <param name="lParam"></param>
        /// <param name="result"></param>
        /// <returns></returns>
        [DllImport(BlinkBrowserdll, CallingConvention = CallingConvention.StdCall)]
        [return: MarshalAs(UnmanagedType.I1)]
        public static extern bool mbFireWindowsMessage(IntPtr webView, IntPtr hWnd, uint message, IntPtr wParam, IntPtr lParam, IntPtr result);
        /// <summary>
        /// 使WebView选中
        /// </summary>
        /// <param name="webView"></param>
        [DllImport(BlinkBrowserdll, CallingConvention = CallingConvention.StdCall)]
        public static extern void mbSetFocus(IntPtr webView);
        /// <summary>
        /// 使WebView失去焦点。
        /// </summary>
        /// <param name="webView"></param>
        [DllImport(BlinkBrowserdll, CallingConvention = CallingConvention.StdCall)]
        public static extern void mbKillFocus(IntPtr webView);
        /// <summary>
        /// 显示窗体
        /// </summary>
        /// <param name="webView"></param>
        /// <param name="show"></param>
        [DllImport(BlinkBrowserdll, CallingConvention = CallingConvention.StdCall)]
        public static extern void mbShowWindow(IntPtr webView, [MarshalAs(UnmanagedType.I1)] bool show);
        /// <summary>
        /// 加载URL
        /// </summary>
        /// <param name="webView"></param>
        /// <param name="url"></param>
        [DllImport(BlinkBrowserdll, CallingConvention = CallingConvention.StdCall)]
        public static extern void mbLoadURL(IntPtr webView, IntPtr url);
        /// <summary>
        /// 设置HTML
        /// </summary>
        /// <param name="webView"></param>
        /// <param name="html"></param>
        /// <param name="url"></param>
        /// <param name="baseUrl"></param>
        [DllImport(BlinkBrowserdll, CallingConvention = CallingConvention.StdCall)]
        public static extern void mbLoadHtmlWithBaseUrl(IntPtr webView, IntPtr html, IntPtr url, IntPtr baseUrl);
        /// <summary>
        /// 获取锁定的WebView DC
        /// </summary>
        /// <param name="webView"></param>
        /// <returns></returns>
        [DllImport(BlinkBrowserdll, CallingConvention = CallingConvention.StdCall)]
        public static extern IntPtr mbGetLockedViewDC(IntPtr webView);
        /// <summary>
        /// 解锁 WebView DC
        /// </summary>
        /// <param name="webView"></param>
        [DllImport(BlinkBrowserdll, CallingConvention = CallingConvention.StdCall)]
        public static extern void mbUnlockViewDC(IntPtr webView);
        /// <summary>
        /// 无效
        /// </summary>
        /// <param name="webView"></param>
        [DllImport(BlinkBrowserdll, CallingConvention = CallingConvention.StdCall)]
        public static extern void mbWake(IntPtr webView);

        [DllImport(BlinkBrowserdll, CallingConvention = CallingConvention.StdCall)]
        public static extern double mbJsToDouble(IntPtr es, Int64 v);

        [DllImport(BlinkBrowserdll, CallingConvention = CallingConvention.StdCall)]
        [return: MarshalAs(UnmanagedType.I1)]
        public static extern bool mbJsToBoolean(IntPtr es, Int64 v);

        [DllImport(BlinkBrowserdll, CallingConvention = CallingConvention.StdCall)]
        public static extern IntPtr mbJsToString(IntPtr es, Int64 v);

        [DllImport(BlinkBrowserdll, CallingConvention = CallingConvention.StdCall)]
        public static extern void mbOnJsQuery(IntPtr webView, mbJsQueryCallback callback, IntPtr param);

        [DllImport(BlinkBrowserdll, CallingConvention = CallingConvention.StdCall)]
        public static extern void mbResponseQuery(IntPtr webView, Int64 queryId, int customMsg, IntPtr response);
        /// <summary>
        /// 
        /// </summary>
        /// <param name="webView"></param>
        /// <param name="frameId"></param>
        /// <param name="script"></param>
        /// <param name="isInClosure">参数：isInClosure表示是否在外层包个function() {}形式的闭包。注意：如果需要返回值，在isInClosure为true时，需要写return，为false则不用</param>
        /// <param name="callback"></param>
        /// <param name="param"></param>
        /// <param name="unuse"></param>
        [DllImport(BlinkBrowserdll, CallingConvention = CallingConvention.StdCall)]
        public static extern void mbRunJs(IntPtr webView, IntPtr frameId, IntPtr script, [MarshalAs(UnmanagedType.I1)] bool isInClosure, mbRunJsCallback callback, IntPtr param, IntPtr unuse);


        public static void mbSetDebugConfig(IntPtr handler, string debugString, string param)
        {
            mbSetDebugConfig(handler, Marshal.StringToCoTaskMemAnsi(debugString), Marshal.StringToCoTaskMemAnsi(param));
        }
        /// <summary>
        /// 调试接口
        /// </summary>
        /// <param name="name"></param>
        /// <param name="fn"></param>
        /// <param name="param"></param>
        [DllImport(BlinkBrowserdll, CallingConvention = CallingConvention.StdCall)]
        public static extern void mbSetDebugConfig(IntPtr handler, IntPtr debugString, IntPtr param);
        /// <summary>
        /// 是否可前进
        /// </summary>
        /// <param name="webView"></param>
        /// <param name="callback"></param>
        /// <param name="param"></param>
        [DllImport(BlinkBrowserdll, CallingConvention = CallingConvention.StdCall)]
        public static extern void mbCanGoForward(IntPtr webView, mbCanGoBackForwardCallback callback, IntPtr param);
        /// <summary>
        /// 是否可后退
        /// </summary>
        /// <param name="webView"></param>
        /// <param name="callback"></param>
        /// <param name="param"></param>
        [DllImport(BlinkBrowserdll, CallingConvention = CallingConvention.StdCall)]
        public static extern void mbCanGoBack(IntPtr webView, mbCanGoBackForwardCallback callback, IntPtr param);

        /// <summary>
        /// 根据URL加载自定义HTML
        /// </summary>
        /// <param name="webView"></param>
        /// <param name="html"></param>
        /// <param name="baseUrl"></param>
        [DllImport(BlinkBrowserdll, CallingConvention = CallingConvention.StdCall)]
        public static extern void mbLoadHtmlWithBaseUrl(IntPtr webView, IntPtr html, IntPtr baseUrl);

        /// <summary>
        /// 设置页面缩放比例
        /// </summary>
        /// <param name="webView"></param>
        /// <param name="factor"></param>
        [DllImport(BlinkBrowserdll, CallingConvention = CallingConvention.StdCall)]
        public static extern void mbSetZoomFactor(IntPtr webView, float factor);

        /// <summary>
        /// 获取cookie
        /// </summary>
        /// <param name="webView"></param>
        /// <param name="callback"></param>
        /// <param name="param"></param>
        [DllImport(BlinkBrowserdll, CallingConvention = CallingConvention.StdCall)]
        public static extern void mbGetCookie(IntPtr webView, mbGetCookieCallback callback, IntPtr param);

        /// <summary>
        /// 设置cookie
        /// </summary>
        /// <param name="webView"></param>
        /// <param name="url"></param>
        /// <param name="cookie"></param>
        [DllImport(BlinkBrowserdll, CallingConvention = CallingConvention.StdCall)]
        public static extern void mbSetCookie(IntPtr webView, IntPtr url, IntPtr cookie);

        /// <summary>
        /// 获取主框架
        /// </summary>
        /// <param name="webView"></param>
        /// <returns></returns>
        [DllImport(BlinkBrowserdll, CallingConvention = CallingConvention.StdCall)]
        public static extern IntPtr mbWebFrameGetMainFrame(IntPtr webView);
        /// <summary>
        /// 判断是不是主框架
        /// </summary>
        /// <param name="webView"></param>
        /// <param name="frameId"></param>
        /// <returns></returns>
        [return: MarshalAs(UnmanagedType.I1)]
        [DllImport(BlinkBrowserdll, CallingConvention = CallingConvention.StdCall)]
        public static extern bool mbIsMainFrame(IntPtr webView, IntPtr frameId);

        /// <summary>
        /// 将页面序列化成 mhtml
        /// </summary>
        /// <param name="webView"></param>
        /// <param name="calback"></param>
        /// <param name="param"></param>
        [DllImport(BlinkBrowserdll, CallingConvention = CallingConvention.StdCall)]
        public static extern void mbUtilSerializeToMHTML(IntPtr webView, mbGetMHTMLCallback calback, IntPtr param);

        [DllImport(BlinkBrowserdll, CallingConvention = CallingConvention.StdCall)]
        public static extern void mbOnDownload(IntPtr webView, mbDownloadCallback calback, IntPtr param);


        /// <summary>
        /// 修改响应URL
        /// </summary>
        /// <param name="jobPtr"></param>
        /// <param name="url"></param>
        [DllImport(BlinkBrowserdll, CallingConvention = CallingConvention.StdCall)]
        public static extern void mbNetChangeRequestUrl(IntPtr jobPtr, IntPtr url);

        /// <summary>
        /// 创建一个自定义请求
        /// </summary>
        /// <param name="url"></param>
        /// <param name="method"></param>
        /// <param name="mime"></param>
        /// <returns></returns>
        [DllImport(BlinkBrowserdll, CallingConvention = CallingConvention.StdCall)]
        public static extern IntPtr mbNetCreateWebUrlRequest(IntPtr url, IntPtr method, IntPtr mime);

        /// <summary>
        /// 添加 HEADER到HTTP请求当中
        /// </summary>
        /// <param name="request"></param>
        /// <param name="name"></param>
        /// <param name="value"></param>
        [DllImport(BlinkBrowserdll, CallingConvention = CallingConvention.StdCall)]
        public static extern void mbNetAddHTTPHeaderFieldToUrlRequest(IntPtr request, IntPtr name, IntPtr value);

        /// <summary>
        /// 开始一个网络请求
        /// </summary>
        /// <param name="webView"></param>
        /// <param name="request"></param>
        /// <param name="param"></param>
        /// <param name="callbacks"></param>
        /// <returns></returns>
        [DllImport(BlinkBrowserdll, CallingConvention = CallingConvention.StdCall)]
        public static extern int mbNetStartUrlRequest(IntPtr webView, IntPtr request, IntPtr param, IntPtr callbacks);

        /// <summary>
        /// 获取网络请求状态
        /// </summary>
        /// <param name="response"></param>
        /// <returns></returns>
        [DllImport(BlinkBrowserdll, CallingConvention = CallingConvention.StdCall)]
        public static extern int mbNetGetHttpStatusCode(IntPtr response);
        /// <summary>
        /// 获取网络异常内容长度
        /// </summary>
        /// <param name="response"></param>
        /// <returns></returns>
        [DllImport(BlinkBrowserdll, CallingConvention = CallingConvention.StdCall)]
        public static extern long mbNetGetExpectedContentLength(IntPtr response);

        /// <summary>
        /// 获取相应URL
        /// </summary>
        /// <param name="response"></param>
        /// <returns></returns>
        [DllImport(BlinkBrowserdll, CallingConvention = CallingConvention.StdCall)]
        public static extern IntPtr mbNetGetResponseUrl(IntPtr response);

        /// <summary>
        /// 
        /// </summary>
        /// <param name="requestId"></param>
        [DllImport(BlinkBrowserdll, CallingConvention = CallingConvention.StdCall)]
        public static extern void mbNetCancelWebUrlRequest(int requestId);

        [DllImport(BlinkBrowserdll, CallingConvention = CallingConvention.StdCall)]
        public static extern void mbOnAlertBox(IntPtr webView, mbAlertBoxCallback callback, IntPtr param);

        [DllImport(BlinkBrowserdll, CallingConvention = CallingConvention.StdCall)]
        public static extern void mbOnConfirmBox(IntPtr webView, mbConfirmBoxCallback callback, IntPtr param);
        [DllImport(BlinkBrowserdll, CallingConvention = CallingConvention.StdCall)]
        public static extern void mbOnPromptBox(IntPtr webView, mbPromptBoxCallback callback, IntPtr param);
        /// <summary>
        /// 设置当前请求的 mime
        /// </summary>
        /// <param name="jobPtr"></param>
        /// <param name="type"></param>
        [DllImport(BlinkBrowserdll, CallingConvention = CallingConvention.StdCall)]
        public static extern void mbNetSetMIMEType(IntPtr jobPtr, IntPtr type);
        /// <summary>
        /// 获取当前请求的mime类型
        /// </summary>
        /// <param name="jobPtr"></param>
        /// <param name="mime"></param>
        /// <returns></returns>
        [DllImport(BlinkBrowserdll, CallingConvention = CallingConvention.StdCall)]
        public static extern IntPtr mbNetGetMIMEType(IntPtr jobPtr, IntPtr mime);
        /// <summary>
        /// 等页面 finish 时回调获取网页 favicon
        /// </summary>
        /// <param name="webView"></param>
        /// <param name="callback"></param>
        /// <param name="param"></param>
        [DllImport(BlinkBrowserdll, CallingConvention = CallingConvention.StdCall)]
        public static extern void mbOnNetGetFavicon(IntPtr webView, mbNetGetFaviconCallback callback, IntPtr param);
        /// <summary>
        /// 后台触发打印功能
        /// </summary>
        /// <param name="webView"></param>
        /// <param name="frameId"></param>
        /// <param name="printParams"></param>
        [DllImport(BlinkBrowserdll, CallingConvention = CallingConvention.StdCall)]
        public static extern void mbUtilPrint(IntPtr webView, IntPtr frameId, mbPrintSettings printParams);

        /// <summary>
        /// 网页有控制台消息时回调
        /// </summary>
        /// <param name="webView">窗体</param>
        /// <param name="callback">回调</param>
        /// <param name="param">自定义参数</param>
        [DllImport(BlinkBrowserdll, CallingConvention = CallingConvention.StdCall)]
        public static extern void mbOnConsole(IntPtr webView, mbConsoleCallback callback, IntPtr param);

        /// <summary>
        /// 在 url hook begin 的时候调用可获取当前页面请求所需要的cookie
        /// </summary>
        /// <param name="webView"></param>
        /// <returns></returns>
        [DllImport(BlinkBrowserdll, CallingConvention = CallingConvention.StdCall)]
        public static extern IntPtr mbGetCookieOnBlinkThread(IntPtr webView);
        /// <summary>
        /// 创建字符串对象
        /// </summary>
        /// <param name="str"></param>
        /// <param name="length"></param>
        /// <returns></returns>
        [DllImport(BlinkBrowserdll, CallingConvention = CallingConvention.StdCall)]
        public static extern IntPtr mbCreateString(IntPtr str, int length);
        /// <summary>
        /// 删除 mbCreateString 创建的对象
        /// </summary>
        /// <param name="mbStringPtr"></param>
        [DllImport(BlinkBrowserdll, CallingConvention = CallingConvention.StdCall)]
        public static extern void mbDeleteString(IntPtr mbStringPtr);
        /// <summary>
        /// 暂停网络任务，一般用于自己接管网络，做后台耗时或其他操作。
        /// </summary>
        /// <param name="jobPtr"></param>
        [DllImport(BlinkBrowserdll, CallingConvention = CallingConvention.StdCall)]
        [return: MarshalAs(UnmanagedType.I1)]
        public static extern bool mbNetHoldJobToAsynCommit(IntPtr jobPtr);
        /// <summary>
        /// 继续网络任务
        /// </summary>
        /// <param name="jobPtr"></param>
        [DllImport(BlinkBrowserdll, CallingConvention = CallingConvention.StdCall)]
        public static extern void mbNetContinueJob(IntPtr jobPtr);
        /// <summary>
        /// 将页面转换成PDF
        /// </summary>
        /// <param name="webView">窗体</param>
        /// <param name="frameId">框架ID</param>
        /// <param name="settings">配置</param>
        /// <param name="callback">回调</param>
        /// <param name="param">自定义参数</param>
        [DllImport(BlinkBrowserdll, CallingConvention = CallingConvention.StdCall)]
        public static extern void mbUtilPrintToPdf(IntPtr webView, IntPtr frameId, mbPrintSettings settings, mbPrintPdfDataCallback callback, IntPtr param);
        /// <summary>
        /// 执行异步JS
        /// </summary>
        /// <param name="webView">窗体</param>
        /// <param name="frameId">框架ID</param>
        /// <param name="script">待执行脚本</param>
        /// <param name="isInClosure">参数：isInClosure表示是否在外层包个function() {}形式的闭包。注意：如果需要返回值，在isInClosure为true时，需要写return，为false则不用</param>
        /// <returns>JS结果</returns>
        [DllImport(BlinkBrowserdll, CallingConvention = CallingConvention.StdCall)]
        public static extern UInt32 mbRunJsSync(IntPtr webView, IntPtr frameId, IntPtr script, [MarshalAs(UnmanagedType.I1)] bool isInClosure);
        /// <summary>
        /// 设置代理
        /// </summary>
        /// <param name="webView"></param>
        /// <param name="proxy"></param>
        [DllImport(BlinkBrowserdll, CallingConvention = CallingConvention.StdCall)]
        public static extern void mbSetProxy(IntPtr webView, mbProxy proxy);

        /// <summary>
        /// 调出下载管理器
        /// </summary>
        /// <param name="webView"></param>
        /// <param name="url"></param>
        /// <param name="downloadJob"></param>
        /// <returns></returns>
        [DllImport(BlinkBrowserdll, CallingConvention = CallingConvention.StdCall)]
        [return: MarshalAs(UnmanagedType.I1)]
        public static extern bool mbPopupDownloadMgr(IntPtr webView, IntPtr url, IntPtr downloadJob);
        /// <summary>
        /// 下载回调
        /// </summary>
        /// <param name="webView"></param>
        /// <param name="param"></param>
        /// <param name="contentLength"></param>
        /// <param name="url"></param>
        /// <param name="mime"></param>
        /// <param name="disposition"></param>
        /// <param name="job"></param>
        /// <param name="dataBind">mbNetJobDataBind 类型</param>
        /// <param name="callbackBind">mbPopupDialogAndDownloadBind 类型</param>
        /// <returns></returns>
        [DllImport(BlinkBrowserdll, CallingConvention = CallingConvention.StdCall)]
        public static extern mbDownloadOpt mbPopupDialogAndDownload(IntPtr webView, IntPtr param, int contentLength, IntPtr url, IntPtr mime, IntPtr disposition, IntPtr job, IntPtr/*mbNetJobDataBind*/ dataBind, IntPtr /*mbPopupDialogAndDownloadBind*/ callbackBind);

    }
}
