using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Text;
using System.Windows.Forms;
using System.Text.RegularExpressions;
using System.Runtime.InteropServices;
using System.Runtime.Remoting.Messaging;
using System.Reflection;
using System.IO;
using System.Diagnostics;
using System.Threading;
using MiniBlinkPinvokeVIP.Core;
using MiniBlinkPinvokeVIP.Model;
using MiniBlinkPinvokeVIP.Forms;
using Newtonsoft.Json.Linq;

namespace TZ.BrowserMain.BrowserCore
{

    public partial class BlinkBrowser : UserControl, IMessageFilter
    {
        public BlinkBrowser(Guid Id)
        {
            ID = Id;
            InitializeComponent();
            CheckForIllegalCrossThreadCalls = false;
            _isOffset = false;
            NewBrowser();
            Application.AddMessageFilter(this);
            ThreadPool.SetMinThreads(10, 10);
            ThreadPool.SetMaxThreads(100, 100);


        }
        /// <summary>
        /// Blink唯一标识ID
        /// </summary>
        public Guid ID { get; set; }

        protected IntPtr m_hImc;//输入法句柄



        private readonly object locker = new object();
        public IntPtr BlinkHandle = IntPtr.Zero;//句柄
        string url = string.Empty;
        public IntPtr MainFrameId;

        /// <summary>
        /// 框架ID 集合，方便执行JS
        /// </summary>
        public Dictionary<string, IntPtr> FrameIdList = new Dictionary<string, IntPtr>();

        private bool _isOffset;//是否离屏模式

        IntPtr bits = IntPtr.Zero;
        public object GlobalObjectJs = null;

        private bool _isActivity = true;

        private string _drawIntervalActivity = "3";//默认帧率
        private string _drawIntervalRest = "30000";//后台线程时帧率
        private string _strdrawMinInterval = "drawMinInterval";//设置帧率，默认值是3，值越大帧率越低

        public delegate void OnResponseBegin(string url, IntPtr job);

        public delegate void OnResponseEnd(string url, IntPtr buf, int len);
        /// <summary>
        /// 请求结束时
        /// </summary>
        public event OnResponseEnd OnResponseEndCall;

        private string download_url;//需要下载的url


        private bool _isCreateCored = false;


        mbURLChangedCallback _mbURLChangedCallback;
        mbTitleChangedCallback _mbTitleChangeCallback;
        mbNavigationCallback _mbNavigationCallback;
        mbPaintUpdatedCallback _mbPaintUpdatedCallback;
        mbDocumentReadyCallback _mbDocumentReadyCallback;
        mbLoadingFinishCallback _mbLoadingFinishCallback;
        mbDownloadCallback _mbDownloadFileCallback;
        mbCreateViewCallback _mbCreateViewCallback;
        mbLoadUrlBeginCallback _mbLoadUrlBeginCallback;
        mbLoadUrlEndCallback _mbLoadUrlEndCallback;
        mbJsQueryCallback _mbJsQueryCallback;
        mbConfirmBoxCallback _OnConfirmBoxCall;
        mbAlertBoxCallback _OnAlertBoxCall;
        mbNetGetFaviconCallback _OnNetGetFaviconCallback;
        mbConsoleCallback _OnConsoleCallback;
        mbPromptBoxCallback _OnPromptBoxCallback;


        public void CreateCore()
        {
            if (this.DesignMode)
            {
                return;
            }

            if (_isCreateCored)
            {
                return;
            }

            if (BlinkPInvoke.IsMainThread)
            {
                mbCreateCore();
            }
            else
            {
                if (this.IsHandleCreated)
                {
                    this.Invoke(new MethodInvoker(() =>
                    {
                        mbCreateCore();
                    }));
                }
            }
        }

        /// <summary>
        /// 初始化 MB
        /// </summary>
        private void mbCreateCore()
        {

            this._isCreateCored = true;
            //BlinkPInvoke.mbInit(IntPtr.Zero);
            //var ins = ;
            var ins = BlinkInit.Instance;


            BlinkHandle = BlinkPInvoke.mbCreateWebView();
            //#if DEBUG
            //            ShowDevTools();
            //#endif
            //只有开启才会触发 wkeOnCreateView
            BlinkPInvoke.mbSetNavigationToNewWindowEnable(BlinkHandle, true);

            if (!_isOffset)
            {
                BlinkPInvoke.mbSetHandle(BlinkHandle, this.Handle);
                //BlinkPInvoke.mbSetHandleOffset(BlinkHandle, Location.X, Location.Y);
            }
            BlinkPInvoke.mbSetCspCheckEnable(BlinkHandle, false);//跨域安全检查
            BlinkPInvoke.mbSetMemoryCacheEnable(BlinkHandle, true);//内存缓存
            BlinkPInvoke.mbSetResourceGc(BlinkHandle, ResourceGcIntervalSec);//设置缓存回收间隔
            BlinkPInvoke.mbSetDragDropEnable(BlinkHandle, false);//开启拖动




            BlinkPInvoke.mbSetCookieEnabled(BlinkHandle, true);
            BlinkPInvoke.mbSetNpapiPluginsEnabled(BlinkHandle, true);
            //if (string.IsNullOrEmpty(_cookiePath))
            //{
            //    _cookiePath = System.AppDomain.CurrentDomain.BaseDirectory + "\\cookie\\";
            //}
            if (!Directory.Exists(BlinkCommon.CookiePath))
            {
                Directory.CreateDirectory(BlinkCommon.CookiePath);
            }
            if (!Directory.Exists(BlinkCommon.LocalStoragePath))
            {
                Directory.CreateDirectory(BlinkCommon.LocalStoragePath);
            }
            if (!Directory.Exists(BlinkCommon.CachePath))
            {
                Directory.CreateDirectory(BlinkCommon.CachePath);
            }
            //BlinkPInvoke.mbSetCookieJarPath(BlinkHandle, _cookiePath);
            BlinkPInvoke.mbSetCookieJarFullPath(BlinkHandle, BlinkCommon.CookiePath + "cookies.dat");
            BlinkPInvoke.mbSetLocalStorageFullPath(BlinkHandle, BlinkCommon.LocalStoragePath);

            BlinkPInvoke.mbResize(BlinkHandle, Width, Height);
            BlinkPInvoke.mbSetUserAgent(BlinkHandle, Common.Utf8StringToIntptr(BlinkCommon.BlinkUserAgent));
            BlinkPInvoke.mbSetDebugConfig(BlinkHandle, Common.Utf8StringToIntptr("antiAlias"), BlinkCommon.BlinkAntiAlias);
            BlinkPInvoke.mbSetDebugConfig(BlinkHandle, Common.Utf8StringToIntptr("minimumFontSize"), BlinkCommon.BlinkMinimumFontSize);
            BlinkPInvoke.mbSetDebugConfig(BlinkHandle, Common.Utf8StringToIntptr("minimumLogicalFontSize"), BlinkCommon.BlinkMinimumLogicalFontSize);
            BlinkPInvoke.mbSetDebugConfig(BlinkHandle, Common.Utf8StringToIntptr("defaultFixedFontSize"), BlinkCommon.BlinkDefaultFixedFontSize);
            //BlinkPInvoke.mbSetDebugConfig(BlinkHandle, Common.Utf8StringToIntptr("defaultFontSize"), Common.Utf8StringToIntptr("13"));

            //设置声音
            //mbPInvoke.mbSetMediaVolume(handle, 20);

            _mbNavigationCallback = OnwkeNavigationCallback;
            BlinkPInvoke.mbOnNavigation(BlinkHandle, _mbNavigationCallback, IntPtr.Zero);
            ins.AddAction(new ActionModel { ID = ID, ObjData = _mbNavigationCallback });


            _mbTitleChangeCallback = OnTitleChangedCallback;
            BlinkPInvoke.mbOnTitleChanged(BlinkHandle, _mbTitleChangeCallback, IntPtr.Zero);
            ins.AddAction(new ActionModel { ID = ID, ObjData = _mbTitleChangeCallback });


            _mbDocumentReadyCallback = OnwkeDocumentReadyCallback;
            BlinkPInvoke.mbOnDocumentReady(BlinkHandle, _mbDocumentReadyCallback, IntPtr.Zero);
            ins.AddAction(new ActionModel { ID = ID, ObjData = _mbDocumentReadyCallback });

            //设置url改变时
            _mbURLChangedCallback = OnUrlChangedCallback;
            BlinkPInvoke.mbOnURLChanged(BlinkHandle, _mbURLChangedCallback, IntPtr.Zero);
            ins.AddAction(new ActionModel { ObjData = _mbURLChangedCallback, ID = ID });

            _mbPaintUpdatedCallback = OnWkePaintUpdatedCallback;
            BlinkPInvoke.mbOnPaintUpdated(BlinkHandle, _mbPaintUpdatedCallback, IntPtr.Zero);
            ins.AddAction(new ActionModel { ID = ID, ObjData = _mbPaintUpdatedCallback });

            _mbLoadingFinishCallback = OnwkeLoadingFinishCallback;
            BlinkPInvoke.mbOnLoadingFinish(BlinkHandle, _mbLoadingFinishCallback, IntPtr.Zero);
            ins.AddAction(new ActionModel { ID = ID, ObjData = _mbLoadingFinishCallback });

            _mbDownloadFileCallback = OnwkeDownloadFileCallback;
            BlinkPInvoke.mbOnDownload(BlinkHandle, _mbDownloadFileCallback, IntPtr.Zero);
            ins.AddAction(new ActionModel { ObjData = _mbDownloadFileCallback, ID = ID });

            _mbCreateViewCallback = OnwkeCreateViewCallback;
            BlinkPInvoke.mbOnCreateView(BlinkHandle, _mbCreateViewCallback, IntPtr.Zero);
            ins.AddAction(new ActionModel { ID = ID, ObjData = _mbCreateViewCallback });

            _mbLoadUrlBeginCallback = OnwkeLoadUrlBeginCallback;
            BlinkPInvoke.mbOnLoadUrlBegin(BlinkHandle, _mbLoadUrlBeginCallback, IntPtr.Zero);
            ins.AddAction(new ActionModel { ObjData = _mbLoadUrlBeginCallback, ID = ID });

            _mbJsQueryCallback = OnJsQueryCallback;
            BlinkPInvoke.mbOnJsQuery(BlinkHandle, _mbJsQueryCallback, IntPtr.Zero);
            ins.AddAction(new ActionModel { ID = ID, ObjData = _mbJsQueryCallback });

            _mbLoadUrlEndCallback = OnwkeLoadUrlEndCallback;
            BlinkPInvoke.mbOnLoadUrlEnd(BlinkHandle, _mbLoadUrlEndCallback, IntPtr.Zero);
            ins.AddAction(new ActionModel { ObjData = _mbLoadUrlEndCallback, ID = ID });

            _OnConfirmBoxCall = OnConfirmBoxCallback;
            BlinkPInvoke.mbOnConfirmBox(BlinkHandle, _OnConfirmBoxCall, IntPtr.Zero);
            ins.AddAction(new ActionModel { ID = ID, ObjData = _OnConfirmBoxCall });

            _OnAlertBoxCall = OnAlertCallback;
            BlinkPInvoke.mbOnAlertBox(BlinkHandle, _OnAlertBoxCall, IntPtr.Zero);
            ins.AddAction(new ActionModel { ObjData = _OnAlertBoxCall, ID = ID });

            _OnConsoleCallback = OnConsoleCallback;
            BlinkPInvoke.mbOnConsole(BlinkHandle, _OnConsoleCallback, IntPtr.Zero);
            ins.AddAction(new ActionModel { ObjData = _OnConsoleCallback, ID = ID });

            _OnPromptBoxCallback = OnPromptBoxCallback;
            BlinkPInvoke.mbOnPromptBox(BlinkHandle, _OnPromptBoxCallback, IntPtr.Zero);
            ins.AddAction(new ActionModel { ObjData = _OnPromptBoxCallback, ID = ID });


            if (url != string.Empty)
            {
                LoadUrl(url);
            }
        }


        private void NewBrowser()
        {
            m_hImc = WinAPI.ImmGetContext(this.Handle);

            if (!_isOffset)
            {
                SetStyle(ControlStyles.OptimizedDoubleBuffer |
                    ControlStyles.DoubleBuffer |
                    ControlStyles.AllPaintingInWmPaint |
                    ControlStyles.ResizeRedraw |
                    ControlStyles.UserPaint, true);
                UpdateStyles();
            }
        }

        #region 各种重写
        //protected override void OnMouseEnter(EventArgs e)
        //{
        //    base.OnMouseEnter(e);
        //    if (this.Visible)
        //    {

        //        this.Focus();
        //    }
        //}

        protected override void OnMouseWheel(MouseEventArgs e)
        {

            base.OnMouseWheel(e);
            if (BlinkHandle != IntPtr.Zero)
            {
                uint flags = GetMouseFlags(e);
                BlinkPInvoke.mbFireMouseWheelEvent(BlinkHandle, e.X, e.Y, e.Delta, flags);
            }
        }

        private static uint GetMouseFlags(MouseEventArgs e)
        {
            uint flags = 0;
            if (e.Button == MouseButtons.Left)
            {
                flags = flags | (uint)mbMouseFlags.MB_LBUTTON;
            }
            if (e.Button == MouseButtons.Middle)
            {
                flags = flags | (uint)mbMouseFlags.MB_MBUTTON;
            }
            if (e.Button == MouseButtons.Right)
            {
                flags = flags | (uint)mbMouseFlags.MB_RBUTTON;
            }
            if (Control.ModifierKeys == Keys.Control)
            {
                flags = flags | (uint)mbMouseFlags.MB_CONTROL;
            }
            if (Control.ModifierKeys == Keys.Shift)
            {
                flags = flags | (uint)mbMouseFlags.MB_SHIFT;
            }
            return flags;
        }

        protected override void OnPreviewKeyDown(PreviewKeyDownEventArgs e)
        {
            base.OnPreviewKeyDown(e);
            switch (e.KeyCode)
            {
                case Keys.Down:
                case Keys.Up:
                case Keys.Left:
                case Keys.Right:
                case Keys.Tab:
                    e.IsInputKey = true;
                    break;
            }
        }

        protected override void OnLostFocus(EventArgs e)
        {
            base.OnLostFocus(e);
            if (BlinkHandle != IntPtr.Zero)
            {
                BlinkPInvoke.mbKillFocus(BlinkHandle);
            }
        }

        protected override void OnGotFocus(EventArgs e)
        {
            base.OnGotFocus(e);
            if (BlinkHandle != IntPtr.Zero)
            {
                BlinkPInvoke.mbSetFocus(BlinkHandle);
            }
        }

        protected override void OnKeyUp(KeyEventArgs e)
        {
            base.OnKeyUp(e);
            if (BlinkHandle != IntPtr.Zero)
            {
                BlinkPInvoke.mbFireKeyUpEvent(BlinkHandle, (uint)e.KeyValue, 0, false);
            }
        }

        protected override void OnKeyPress(KeyPressEventArgs e)
        {
            base.OnKeyPress(e);
            if (BlinkHandle != IntPtr.Zero)
            {
                e.Handled = true;
                BlinkPInvoke.mbFireKeyPressEvent(BlinkHandle, (uint)e.KeyChar, 0, false);
            }
        }
        /// <summary>
        /// 快捷键
        /// </summary>
        /// <param name="msg"></param>
        /// <param name="keyData"></param>
        /// <returns></returns>
        protected override bool ProcessCmdKey(ref Message msg, Keys keyData)
        {
            switch (msg.Msg)
            {
                case 0x100:
                case 0x104:
                    switch (keyData)
                    {
                        case Keys.Control | Keys.S: //Ctrl + S pressed
                            if (!url.StartsWith("tz://"))
                            {
                                SaveAs();
                            }
                            break;
                            //case Keys.Control | Keys.F: //Ctrl + F pressed
                            //    if (!baseSearchPanel.Visible)
                            //    {
                            //        baseSearchPanel.Visible = true;
                            //        baseSearchPanel.BackColor = Color.White;
                            //    }
                            //    break;
                    }
                    break;
                default:
                    break;
            }
            return base.ProcessCmdKey(ref msg, keyData);
        }

        private void SaveAs()
        {
            //mbGetMHTMLCallback callback = delegate (IntPtr webView, IntPtr param, IntPtr mthml)
            //{
            //    BlinkJSAPI.mainForm.Invoke(new EventHandler(delegate
            //    {
            //        SaveFileDialog saveFileDialog = new SaveFileDialog();
            //        saveFileDialog.Title = RS.MainProgramName + " 网页另存为";
            //        saveFileDialog.Filter = "mhtml文件|*.mhtml";
            //        saveFileDialog.FileName = _title;
            //        if (saveFileDialog.ShowDialog() == DialogResult.OK)
            //        {
            //            if (!string.IsNullOrEmpty(saveFileDialog.FileName))
            //            {
            //                File.WriteAllText(saveFileDialog.FileName, Common.Utf8IntptrToString(mthml));
            //            }
            //        }
            //    }));

            //};
            //BlinkPInvoke.mbUtilSerializeToMHTML(BlinkHandle, callback, IntPtr.Zero);
        }

        protected override void OnMouseDown(MouseEventArgs e)
        {

            base.OnMouseDown(e);
            uint msg = 0;
            if (e.Button == MouseButtons.Left)
            {
                msg = (uint)wkeMouseMessage.WKE_MSG_LBUTTONDOWN;
            }
            else if (e.Button == MouseButtons.Middle)
            {
                msg = (uint)wkeMouseMessage.WKE_MSG_MBUTTONDOWN;
            }
            else if (e.Button == MouseButtons.Right)
            {
                msg = (uint)wkeMouseMessage.WKE_MSG_RBUTTONDOWN;
            }
            uint flags = GetMouseFlags(e);
            if (BlinkHandle != IntPtr.Zero)
            {
                BlinkPInvoke.mbFireMouseEvent(BlinkHandle, msg, e.X, e.Y, flags);
            }
        }
        protected override void OnMouseUp(MouseEventArgs e)
        {
            base.OnMouseUp(e);
            uint msg = 0;
            if (e.Button == MouseButtons.Left)
            {
                msg = (uint)mbMouseMsg.MB_MSG_LBUTTONUP;
            }
            else if (e.Button == MouseButtons.Middle)
            {
                msg = (uint)mbMouseMsg.MB_MSG_MBUTTONUP;
            }
            else if (e.Button == MouseButtons.Right)
            {
                msg = (uint)mbMouseMsg.MB_MSG_RBUTTONUP;
            }
            uint flags = GetMouseFlags(e);
            if (BlinkHandle != IntPtr.Zero)
            {
                BlinkPInvoke.mbFireMouseEvent(BlinkHandle, msg, e.X, e.Y, flags);
            }
        }
        protected override void OnMouseMove(MouseEventArgs e)
        {
            base.OnMouseMove(e);
            if (BlinkHandle != IntPtr.Zero)
            {
                uint flags = GetMouseFlags(e);
                BlinkPInvoke.mbFireMouseEvent(BlinkHandle, 0x200, e.X, e.Y, flags);
            }
        }
        protected override void OnKeyDown(KeyEventArgs e)
        {
            base.OnKeyDown(e);
            if (BlinkHandle != IntPtr.Zero)
            {
                BlinkPInvoke.mbFireKeyDownEvent(BlinkHandle, (uint)e.KeyValue, 0, false);
            }
            //#if !DEBUG
            if (e.KeyCode == Keys.F12)
            {
                ShowDevTools();
            }
            //if (e.KeyCode == Keys.F5)
            //{
            //    Reload();
            //}
            //#endif
        }

        protected override void OnSizeChanged(EventArgs e)
        {
            base.OnSizeChanged(e);
            if (BlinkHandle != IntPtr.Zero && Width > 0 && Height > 0)
            {
                BlinkPInvoke.mbResize(BlinkHandle, Width, Height);
                Invalidate();
            }
        }

        protected override void OnCreateControl()
        {
            base.OnCreateControl();
            if (!DesignMode)
            {
                CreateCore();
            }
        }

        protected override void SetVisibleCore(bool value)
        {
            if (_isOffset)
            {
                base.SetVisibleCore(false);
            }
            else
            {
                base.SetVisibleCore(value);
            }
        }

        protected override void OnPaint(PaintEventArgs e)
        {
            if (DesignMode)
            {
                e.Graphics.DrawString("TZMiniBlinkBrowser", Font, Brushes.Red, (PointF)new Point());
                e.Graphics.DrawRectangle(Pens.Black, new Rectangle(0, 0, Width, Height));
            }
            else
            {
                if (BlinkHandle != IntPtr.Zero)
                {
                    IntPtr hSrcDC = BlinkPInvoke.mbGetLockedViewDC(BlinkHandle);
                    //Point point = FindForm().PointToClient(PointToScreen(new Point(e.ClipRectangle.X, e.ClipRectangle.Y)));
                    IntPtr hdc = e.Graphics.GetHdc();
                    WinAPI.BitBlt(hdc, e.ClipRectangle.X, e.ClipRectangle.Y, e.ClipRectangle.Width, e.ClipRectangle.Height, hSrcDC, e.ClipRectangle.X, e.ClipRectangle.Y, 0xcc0020);
                    e.Graphics.ReleaseHdc(hdc);
                    BlinkPInvoke.mbUnlockViewDC(BlinkHandle);
                }
            }
            base.OnPaint(e);
        }
        ushort HIWORD(uint val)
        {
            return (ushort)((((uint)(val)) >> 16) & 0xffff);
        }
        protected override void WndProc(ref Message m)
        {
            if (BlinkHandle == IntPtr.Zero)
            {
                base.WndProc(ref m);
                return;
            }
            //if (13 != m.Msg && 14 != m.Msg && 15 != m.Msg && 20 != m.Msg
            //     && 32 != m.Msg && 512 != m.Msg && 132 != m.Msg && 0x2A3 != m.Msg && 0x282 != m.Msg && 0x281 != m.Msg)
            //    Console.WriteLine("WndProc: {0:X000}", m.Msg);
            uint flags = 0;
            IntPtr result = IntPtr.Zero;
            uint virtualKeyCode = (uint)m.WParam;

            switch (m.Msg)
            {
                case (int)WinConst.WM_SETCURSOR:
                    {
                        if (BlinkPInvoke.mbFireWindowsMessage(BlinkHandle, m.HWnd, (uint)WinConst.WM_SETCURSOR, IntPtr.Zero, IntPtr.Zero, result))
                            m.Result = result;

                        return;
                    }
                case (int)WinConst.WM_IME_STARTCOMPOSITION:
                    {
                        if (BlinkPInvoke.mbFireWindowsMessage(BlinkHandle, m.HWnd, (uint)WinConst.WM_IME_STARTCOMPOSITION, m.WParam, m.LParam, result))
                            m.Result = result;
                        break;
                    }

                //case (int)WinConst.WM_IME_COMPOSITION:
                //    m.Result = IntPtr.Zero;
                //    break;

                case (int)WinConst.WM_IME_SETCONTEXT:
                    if (m.WParam.ToInt32() == 1)
                    {
                        WinAPI.ImmAssociateContext(this.Handle, m_hImc);
                    }
                    break;
                    //case (int)WinConst.WM_KEYDOWN:

                    //    if (0 != (HIWORD((uint)m.LParam) & (uint)WinConst.KF_REPEAT))
                    //        flags |= (uint)WinConst.KF_REPEAT;
                    //    if (0 != (HIWORD((uint)m.LParam) & (uint)WinConst.KF_EXTENDED))
                    //        flags |= (uint)WinConst.KF_EXTENDED;

                    //    BlinkPInvoke.mbFireKeyDownEvent(BlinkHandle, virtualKeyCode, flags, false);
                    //    m.Result = IntPtr.Zero;
                    //    return;
                    //break;
                    //case (int)WinConst.WM_KEYUP:
                    //    if (0 != (HIWORD((uint)m.LParam) & (uint)WinConst.KF_REPEAT))
                    //        flags |= (uint)WinConst.KF_REPEAT;
                    //    if (0 != (HIWORD((uint)m.LParam) & (uint)WinConst.KF_EXTENDED))
                    //        flags |= (uint)WinConst.KF_EXTENDED;

                    //    BlinkPInvoke.mbFireKeyUpEvent(BlinkHandle, virtualKeyCode, flags, false);
                    //    m.Result = IntPtr.Zero;
                    //    return;
                    //break;
                    //case (int)WinConst.WM_CHAR:
                    //    if (0 != (HIWORD((uint)m.LParam) & (uint)WinConst.KF_REPEAT))
                    //        flags |= (uint)WinConst.KF_REPEAT;
                    //    if (0 != (HIWORD((uint)m.LParam) & (uint)WinConst.KF_EXTENDED))
                    //        flags |= (uint)WinConst.KF_EXTENDED;

                    //    BlinkPInvoke.mbFireKeyPressEvent(BlinkHandle, virtualKeyCode, flags, false);
                    //    m.Result = IntPtr.Zero;
                    //    return;
                    break;

            }
            base.WndProc(ref m);
        }
        protected override void OnVisibleChanged(EventArgs e)
        {
            base.OnVisibleChanged(e);
            if (BlinkHandle != IntPtr.Zero && !_isOffset)
            {
                if (this.Visible)
                {
                    this.Focus();
                    if (this.IsDisposed || this.Disposing || !this.IsHandleCreated) return;
                    _isActivity = true;
                    this.MbInvoke(new MethodInvoker(() =>
                    {
                        //IntPtr intPtr = Marshal.StringToHGlobalAnsi("3"); ;
                        BlinkPInvoke.mbSetDebugConfig(BlinkHandle, _strdrawMinInterval, _drawIntervalActivity);
                        //Marshal.FreeHGlobal(intPtr);
                    }));
                }
                else
                {
                    //if (this.IsDisposed || this.Disposing || !this.IsHandleCreated) return;
                    //_isActivity = false;
                    //this.MbInvoke(new MethodInvoker(() =>
                    //{
                    //    //Debug.WriteLine(DateTime.Now + " 命中 OnVisibleChanged _drawIntervalRest:" + _drawIntervalRest);

                    //    BlinkPInvoke.mbSetDebugConfig(BlinkHandle, _strdrawMinInterval, _drawIntervalRest);
                    //}));
                }
            }
        }
        #endregion

        #region 各种回调
        void OnwkeLoadUrlEndCallback(IntPtr webView, IntPtr param, string url, IntPtr job, IntPtr buf, int len)
        {
            if (OnResponseEndCall != null)
            {
                OnResponseEndCall(url, buf, len);
            }

        }
        void OnWkePaintUpdatedCallback(IntPtr webView, IntPtr param, IntPtr hdc, int x, int y, int cx, int cy)
        {
            if (this.Visible)
            {
                _isActivity = true;

                Invalidate(new Rectangle(x, y, cx, cy), false);
            }
            else
            {
                if (_isActivity)
                {
                    if (this.IsDisposed || BlinkHandle == IntPtr.Zero)
                        return;

                    _isActivity = false;
                    this.MbInvoke(new MethodInvoker(() =>
                    {
                        //Console.WriteLine(DateTime.Now + " 命中 OnWkePaintUpdatedCallback _drawIntervalRest:" + _drawIntervalRest);
                        BlinkPInvoke.mbSetDebugConfig(BlinkHandle, _strdrawMinInterval, _drawIntervalRest);
                    }));
                }
            }
        }


        bool OnwkeLoadUrlBeginCallback(IntPtr webView, IntPtr param, string url, IntPtr job)
        {
            //Console.WriteLine($"{DateTime.Now} mbGetCookieOnBlinkThread:{Common.Utf8IntptrToString(cc)}");
            if (url.StartsWith("miniblinkvip://"))
            {
                string[] paramList = new string[0];
                if (url.Contains("?"))
                {
                    paramList = url.Split('?')[1].Split('&');
                    url = url.Split('?')[0];
                }
                Regex regex = new Regex(@"^miniblinkvip://", RegexOptions.IgnoreCase | RegexOptions.Singleline);
                string str = regex.Replace(url, "");
                str = str.Replace('/', '.');
                System.Reflection.Assembly Assemblys = BlinkPInvoke.ResourceAssemblys["MiniBlinkPinvokeVIP"];
                if (Assemblys != null)
                {
                    using (Stream sm = Assemblys.GetManifestResourceStream("MiniBlinkPinvokeVIP." + str))
                    {
                        if (sm != null)
                        {
                            var bytes = Common.StreamToBytes(sm);
                            if (url.EndsWith(".css"))
                            {
                                BlinkPInvoke.mbNetSetMIMEType(job, BlinkCommon.CssMIME);
                            }
                            else if (url.EndsWith(".png"))
                            {
                                BlinkPInvoke.mbNetSetMIMEType(job, Marshal.StringToCoTaskMemAnsi("image/png"));
                            }
                            else if (url.EndsWith(".gif"))
                            {
                                BlinkPInvoke.mbNetSetMIMEType(job, Marshal.StringToCoTaskMemAnsi("image/gif"));
                            }
                            else if (url.EndsWith(".jpg") || url.EndsWith(".jpeg"))
                            {
                                BlinkPInvoke.mbNetSetMIMEType(job, Marshal.StringToCoTaskMemAnsi("image/jpg"));
                            }
                            else if (url.EndsWith(".js"))
                            {
                                BlinkPInvoke.mbNetSetMIMEType(job, BlinkCommon.JavascriptMIME);
                            }
                            else if (url.EndsWith(".svg"))
                            {
                                BlinkPInvoke.mbNetSetMIMEType(job, Marshal.StringToCoTaskMemAnsi("image/svg+xml"));
                            }
                            else if (url.EndsWith(".woff"))
                            {
                                BlinkPInvoke.mbNetSetMIMEType(job, Marshal.StringToCoTaskMemAnsi("application/x-font-woff"));
                            }
                            else if (url.EndsWith(".woff2"))
                            {
                                BlinkPInvoke.mbNetSetMIMEType(job, Marshal.StringToCoTaskMemAnsi("application/x-font-woff"));
                            }
                            else if (url.EndsWith(".ttf"))
                            {
                                BlinkPInvoke.mbNetSetMIMEType(job, Marshal.StringToCoTaskMemAnsi("application/x-font-truetype"));
                            }
                            else if (url.EndsWith(".svg"))
                            {
                                BlinkPInvoke.mbNetSetMIMEType(job, Marshal.StringToCoTaskMemAnsi("image/svg+xml"));
                            }
                            else if (url.EndsWith(".otf"))
                            {
                                BlinkPInvoke.mbNetSetMIMEType(job, Marshal.StringToCoTaskMemAnsi("application/x-font-opentype"));
                            }
                            else if (url.EndsWith(".eot"))
                            {
                                BlinkPInvoke.mbNetSetMIMEType(job, Marshal.StringToCoTaskMemAnsi("application/vnd.ms-fontobject"));
                            }
                            else
                            {
                                BlinkPInvoke.mbNetSetMIMEType(job, Marshal.StringToCoTaskMemAnsi("text/html"));
                            }
                            BlinkPInvoke.mbNetSetData(job, bytes, bytes.Length);
                            //页面加载完成事件
                            _isLoadingCompleted = true;
                            _isLoadingSucceeded = true;
                            if (DocumentReady != null)
                            {
                                DocumentReady(this, new DocumentReadyEventArgs { FrameId = IntPtr.Zero });
                            }
                        }
                        else
                        {
                            ResNotFond(url, job);
                        }
                    }
                }
                else
                {
                    return false;//不处理请求，交给 blink 处理
                }
            }
            else
            {
                //如果需要 OnwkeLoadUrlEndCallback 回调，需要取消注释下面的 hook
                //   BlinkPInvoke.mbNetHookRequest(job);

            }
            return false;//不处理请求，交给 blink 处理
        }

        IntPtr OnwkeCreateViewCallback(IntPtr webView, IntPtr param, mbNavigationType navigationType, IntPtr url, IntPtr windowFeatures)
        {
            if (NewWindow != null)
            {
                NewWindowEventArgs ee = new NewWindowEventArgs();
                ee.MiniBlinkHandle = webView;
                ee.NavigationType = navigationType;
                ee.Url = Common.Utf8IntptrToString(url);
                ee.WindowFeatures = windowFeatures;
                NewWindow(this, ee);
                return IntPtr.Zero;
            }
            else
            {
                LoadUrl(Common.Utf8IntptrToString(url));
                return webView;
            }
        }

        void OnAlertCallback(IntPtr webView, IntPtr param, IntPtr msg)
        {
            if (AlertBox != null)
            {
                AlertBoxEventArgs alertBoxEventArgs = new AlertBoxEventArgs
                {
                    Handled = true,
                    Message = Common.Utf8IntptrToString(msg)
                };
                AlertBox(this, alertBoxEventArgs);
            }
            else
            {
                bool isEnd = false;
                Thread thread = new Thread(new ParameterizedThreadStart((ps) =>
                {
                    isEnd = false;
                    var model = ps as ConfirmThreadModel;
                    this.Invoke(new EventHandler(delegate
                    {
                        isEnd = false;
                        var confirm = new frmAlert(model.Url, model.Msg);
                        confirm.ShowDialog();
                        isEnd = true;
                    }));

                }))
                {
                    IsBackground = true
                };
                thread.Start(
                    new ConfirmThreadModel
                    {
                        Msg = Common.Utf8IntptrToString(msg),
                        Url = url
                    }
                   );
                isEnd = false;
                do
                {
                    Thread.Sleep(25);
                    Application.DoEvents();

                } while (!isEnd);
            }
        }

        IntPtr OnPromptBoxCallback(IntPtr webView, IntPtr param, IntPtr msg, IntPtr defaultResult)
        {
            bool IsEnd = false;
            bool isOk = false;
            string userMsg = string.Empty;
            this.Invoke((EventHandler)delegate
            {
                var f = new frmPromptBox(Common.Utf8IntptrToString(msg), Common.Utf8IntptrToString(defaultResult));
                f.UserInputEvent += (_msg, _isOk) =>
                {
                    userMsg = _msg;
                    isOk = _isOk;
                };
                f.ShowDialog();
                IsEnd = true;
            });
            do
            {
                Thread.Sleep(25);
                Application.DoEvents();
            } while (!IsEnd);
            var pi = BlinkPInvoke.mbCreateString(Common.Utf8StringToIntptr(userMsg), Encoding.UTF8.GetByteCount(userMsg));
            System.Windows.Forms.Timer delTimer = new System.Windows.Forms.Timer();
            delTimer.Tick += (a, b) =>
            {
                try
                {
                    BlinkPInvoke.mbDeleteString(pi);
                }
                catch (Exception)
                {
                }
                delTimer.Enabled = false;
                GC.Collect();
                delTimer.Dispose();
            };
            delTimer.Interval = 3000;
            delTimer.Enabled = true;
            return pi;
        }


        bool OnConfirmBoxCallback(IntPtr webView, IntPtr param, IntPtr msg)
        {
            bool isOk = false;
            if (ConfirmBox != null)
            {
                return ConfirmBox(Common.Utf8IntptrToString(msg));
            }
            else
            {
                bool isEnd = false;
                Thread thread = new Thread(new ParameterizedThreadStart((ps) =>
                {
                    isEnd = false;
                    var model = ps as ConfirmThreadModel;
                    this.Invoke(new EventHandler(delegate
                      {
                          isEnd = false;
                          var confirm = new frmConfirm(model.Url, model.Msg);
                          confirm.TopMost = true;
                          var res = confirm.ShowDialog();
                          isOk = res == DialogResult.OK;
                          isEnd = true;
                      }));

                }))
                {
                    IsBackground = true
                };
                thread.Start(
                    new ConfirmThreadModel
                    {
                        Msg = Common.Utf8IntptrToString(msg),
                        Url = url
                    }
                   );
                isEnd = false;
                do
                {
                    Thread.Sleep(25);
                    Application.DoEvents();

                } while (!isEnd);
            }
            return isOk;
        }
        bool OnwkeDownloadFileCallback(IntPtr webView, IntPtr param, IntPtr frameId, IntPtr url, IntPtr downloadJob)
        {
            //return false;
            string strURL = Common.Utf8IntptrToString(url);
            if (!string.IsNullOrEmpty(strURL))
            {
                //var uri = new Uri(strURL);
                //if (uri.AbsolutePath.EndsWith(".woff2"))
                //{
                //    Console.WriteLine(DateTime.Now + " URL:" + strURL + " 不自己处理，丢给 blink");
                //    return false;//不自己处理，丢给 blink
                //}
                //else if (strURL.EndsWith("/ImportDeclaration"))
                //{
                //    return false;//金二核注清单导入失败返回
                //}
            }
            else
            {
                Console.WriteLine(DateTime.Now + " URL:" + strURL + " 不自己处理，丢给 blink");
                return false;//不自己处理，丢给 blink
            }
            if (Download != null)
            {
                DownloadEventArgs ee = new DownloadEventArgs();
                ee.Url = Common.Utf8IntptrToString(url);
                Download(this, ee);
            }
            download_url = Common.Utf8IntptrToString(url);

            #region 调用内置下载管理
            BlinkPInvoke.mbPopupDownloadMgr(webView, url, downloadJob);

            #endregion

            //页面加载完成事件
            if (DocumentReady != null)
            {
                DocumentReady(this, new DocumentReadyEventArgs { FrameId = frameId });
            }
            return true;//自己处理，不需要 blink 处理(理论应该返回 true 但如果返回true ，程序会异常退出 先放这，可能是 blink 接口问题。)
        }

        void OnwkeLoadingFinishCallback(IntPtr webView, IntPtr param, IntPtr frameId, IntPtr urlPtr, mbLoadingResult result, IntPtr failedReason)
        {

            string urlStr = Common.Utf8IntptrToString(urlPtr);
            if (urlStr == "about:blank")
            {
                return;
            }
            if (result == mbLoadingResult.MB_LOADING_SUCCEEDED)
            {
                if (!_isLoadingFailed)
                {
                    _OnNetGetFaviconCallback = OnNetGetFaviconCallback;
                    BlinkPInvoke.mbOnNetGetFavicon(BlinkHandle, _OnNetGetFaviconCallback, IntPtr.Zero);
                    BlinkInit.Instance.AddAction(new ActionModel { ObjData = _OnNetGetFaviconCallback, ID = ID });
                }
                _isLoadingSucceeded = true;
                _isLoadingFailed = false;
                if (urlStr != "about:blank" && !IsMainFrame(frameId))
                {

                    if (!FrameIdList.ContainsKey(urlStr))
                    {
                        FrameIdList.Add(urlStr, frameId);
                    }
                    else
                    {
                        FrameIdList[urlStr] = frameId;
                    }
                }
                url = urlStr;
            }
            else if (result == mbLoadingResult.MB_LOADING_FAILED)
            {
                _isLoadingFailed = true;
                var urlTemp = Common.Utf8IntptrToString(urlPtr);
                if (!string.IsNullOrEmpty(urlTemp))
                {
                    url = urlTemp;
                }
                string errMsg = Common.Utf8IntptrToString(failedReason);
                if (errMsg.Equals("error reason: 7, Couldn't connect to server", StringComparison.CurrentCultureIgnoreCase))
                {
                    //Url = $"tz://nonetwork.html?u={url}&e={errMsg}";
                    HTML = NoNetwork(url, errMsg, 7);
                }
                else if (errMsg.Equals("error reason: 6, Couldn't resolve host name", StringComparison.CurrentCultureIgnoreCase))
                {
                    HTML = NoNetwork(url, errMsg, 6);
                }
                //else if (errMsg.StartsWith("error reason: 1"))
                //{

                //}
                else
                {
                    //error reason: 1, Unsupported protocol
                    if (urlStr.StartsWith("singlewindow"))//自定义协议。
                    {

                    }
                    else
                    {
                        HTML = "<h1>" + Common.Utf8IntptrToString(failedReason) + "</h1>";
                    }
                }
            }
            else if (result == mbLoadingResult.MB_LOADING_CANCELED)
            {
                if (!string.IsNullOrEmpty(download_url))
                {
                    DownLoad(webView, param, urlPtr);
                }
            }
            _isLoadingCompleted = true;

            if (LoadingFinish != null && IsMainFrame(frameId))
            {
                LoadingFinishEventArgs ee = new LoadingFinishEventArgs();
                ee.FailedReason = Common.Utf8IntptrToString(failedReason);
                ee.LoadingResult = (mbLoadingResult)result;
                ee.Url = Common.Utf8IntptrToString(urlPtr);
                ee.frameId = frameId;

                LoadingFinish(this, ee);
            }
        }

        void OnConsoleCallback(IntPtr webView, IntPtr param, mbConsoleLevel level, IntPtr message, IntPtr sourceName, int sourceLine, IntPtr stackTrace)
        {
            //string msg = $"TZ.BrowserMain Console level:{level.ToString()} message:{Common.Utf8IntptrToString(message)} sourceName:{Common.Utf8IntptrToString(sourceName)} sourceLine:{sourceLine} stackTrace:{Common.Utf8IntptrToString(stackTrace)}";
            //Console.WriteLine(msg);
            //Trace.WriteLine(msg);
        }

        private void DownLoad(IntPtr webView, IntPtr param, IntPtr url)
        {
            Guid _ID = Guid.NewGuid();

            return;
            //////return;
            //////先用head获取http头
            ////mbUrlRequestCallbacks callback = new mbUrlRequestCallbacks();
            ////List<byte> dataList = new List<byte>();
            //////下载失败
            ////callback.didFailCallback = new mbOnUrlRequestDidFailCallback((re_webView, re_param, re_request, re_error) =>
            ////{
            ////    Console.WriteLine("didFailCallback:" + Utf8IntptrToString(re_error));
            ////    if (DownloadFilsh != null)
            ////    {
            ////        DownloadFilsh(this, new DownloadFilshEvent
            ////        {
            ////            IsError = true,
            ////            ErrorInfo = Utf8IntptrToString(re_error)
            ////        });
            ////    }
            ////});
            //////下载完成
            ////callback.didFinishLoadingCallback = new mbOnUrlRequestDidFinishLoadingCallback((re_webView, re_param, re_request, re_finishTime) =>
            ////{
            ////    var pass = Utf8IntptrToString(re_param);
            ////    var para = JsonConvert.DeserializeObject<DownloadModel>(pass);
            ////    if (para != null)
            ////    {
            ////        if (dataList.Count != 0)
            ////        {
            ////            if (!Directory.Exists(para.Path))
            ////            {
            ////                Directory.CreateDirectory(para.Path);
            ////            }
            ////            if (File.Exists(para.Path + para.Name))
            ////            {
            ////                para.Name = DateTime.Now.Ticks.ToString() + "_" + para.Name;
            ////            }
            ////            File.WriteAllBytes(para.Path + para.Name, dataList.ToArray());
            ////        }
            ////        BlinkInit.Instance.RemoveAction(new ActionModel { ID = para.ID });
            ////        if (DownloadFilsh != null)
            ////        {
            ////            DownloadFilsh(this, new DownloadFilshEvent
            ////            {
            ////                IsError = false,
            ////                DownloadInfo = para
            ////            });
            ////        }
            ////    }
            ////    MessageBox.Show("文件下载完成。");
            ////});
            //////收到数据
            ////callback.didReceiveDataCallback = new mbOnUrlRequestDidReceiveDataCallback((re_webView, re_param, re_request, re_data, re_dataLength) =>
            ////{
            ////    Console.WriteLine("didReceiveDataCallback:" + re_dataLength);
            ////    byte[] managedArray2 = new byte[re_dataLength];
            ////    Marshal.Copy(re_data, managedArray2, 0, re_dataLength);
            ////    dataList.AddRange(managedArray2);
            ////});
            //////开始收到下载响应
            ////callback.didReceiveResponseCallback = new mbOnUrlRequestDidReceiveResponseCallback((re_webView, re_param, re_request, re_response) =>
            ////{
            ////    Console.WriteLine("didReceiveResponseCallback:" + Utf8IntptrToString(re_response));
            ////});
            //////下载被重定向
            ////callback.willRedirectCallback = new mbOnUrlRequestWillRedirectCallback((re_webView, re_param, re_oldRequest, re_request, re_redirectResponse) =>
            ////{
            ////    if (DownloadFilsh != null)
            ////    {
            ////        DownloadFilsh(this, new DownloadFilshEvent
            ////        {
            ////            IsError = true,
            ////            ErrorInfo = "下载被重定向"
            ////        });
            ////    }
            ////});
            //var _cc = GetCookie();
            //var formDownload = new Forms.frmNewDownload(Common.Utf8IntptrToString(url), GetCookiesFromFile);
            ////var formDownload = new Forms.frmFindDownload(Common.Utf8IntptrToString(url), GetCookiesFromFile);
            //DownloadModel downloadModel = null;
            //formDownload.GetURLDownloadInfo += (string path, string fileName, bool flag, bool openFile) =>
            //{
            //    if (flag)
            //    {
            //        downloadModel = new DownloadModel
            //        {
            //            ID = _ID,
            //            Name = fileName,
            //            Path = path,
            //            URL = Common.Utf8IntptrToString(url),
            //            FilshOpenFile = openFile
            //        };
            //    }
            //};
            //BlinkJSAPI.mainForm.Invoke(new EventHandler(delegate
            //{
            //    formDownload.ShowDialog();
            //}));
            //if (downloadModel != null)
            //{
            //    //////BlinkInit.Instance.AddAction(new ActionModel { ID = _ID, ObjData = callback });
            //    //////开始下载
            //    //////IntPtr donwloadrequest = BlinkPInvoke.mbNetCreateWebUrlRequest(url, Utf8StringToIntptr("GET"), Utf8StringToIntptr("application/octet-stream;charset=UTF-8"));
            //    //////设置COOKIE
            //    //////var cc = GetCookie();//  SWCommon.SWLoinInfo.SWCookie;
            //    //////foreach (var item in cc.Split(';'))
            //    //////{
            //    //////    BlinkPInvoke.mbNetAddHTTPHeaderFieldToUrlRequest(donwloadrequest, Utf8StringToIntptr("Cookie"), Utf8StringToIntptr(" " + item));
            //    //////}
            //    //////BlinkPInvoke.mbNetAddHTTPHeaderFieldToUrlRequest(donwloadrequest, Utf8StringToIntptr("User-Agent"), Utf8StringToIntptr(UserAgent));
            //    //////BlinkPInvoke.mbNetStartUrlRequest(webView, donwloadrequest, Utf8StringToIntptr(JsonConvert.SerializeObject(downloadModel)), callback.ToIntPtr());

            //    //后台多线程下载

            //    Thread thread = new Thread(new ParameterizedThreadStart((ps) =>
            //    {
            //        var dm = ps as DownloadModel;
            //        HttpItem httpItem = new HttpItem
            //        {
            //            Accept = "text/html,application/xhtml+xml,application/xml;q=0.9,image/webp,image/apng,*/*;q=0.8",
            //            UserAgent = BlinkCommon.BlinkUserAgent,
            //            Cookie = GetCookie(),
            //            Allowautoredirect = false,
            //            Referer = dm.URL,
            //            Method = "GET",
            //            ResultType = ResultType.Byte,
            //            URL = dm.URL,
            //        };
            //        if (httpItem.Cookie == string.Empty)
            //        {
            //            httpItem.Cookie = GetCookiesFromFile;
            //        }
            //        var res = new HttpHelper().GetHtml(httpItem);
            //        if (res.StatusCode != System.Net.HttpStatusCode.OK)
            //        {
            //            if (DownloadFilsh != null)
            //            {
            //                DownloadFilsh(this, new DownloadFilshEvent
            //                {
            //                    IsError = true,
            //                    ErrorInfo = "下载失败"
            //                });
            //            }
            //        }
            //        else
            //        {
            //            if (!Directory.Exists(dm.Path))
            //            {
            //                Directory.CreateDirectory(dm.Path);
            //            }
            //            if (File.Exists(dm.Path + dm.Name))
            //            {
            //                dm.Name = DateTime.Now.Ticks.ToString() + "_" + dm.Name;
            //            }
            //            //res.Header[""].
            //            File.WriteAllBytes(dm.Path + dm.Name, res.ResultByte);
            //            if (DownloadFilsh != null)
            //            {
            //                DownloadFilsh(this, new DownloadFilshEvent
            //                {
            //                    IsError = false,
            //                    ErrorInfo = "下载完成",
            //                    DownloadInfo = dm
            //                });
            //            }
            //        }
            //    }))
            //    { IsBackground = true };
            //    thread.Start(downloadModel);
            //}
        }

        void OnwkeDocumentReadyCallback(IntPtr webView, IntPtr param, IntPtr frameId)
        {
            BindJsFunc(frameId);
            if (BlinkPInvoke.mbIsMainFrame(BlinkHandle, frameId))
            {
                DocumentIsReady = true;
                MainFrameId = frameId;
                if (!this.Visible)
                {
                    //页面加载完成后再将页面的刷新帧率降低
                    if (this.IsDisposed || this.Disposing || !this.IsHandleCreated) return;
                    _isActivity = false;
                    this.MbInvoke(new MethodInvoker(() =>
                    {
                        //Debug.WriteLine(DateTime.Now + " 命中 OnVisibleChanged _drawIntervalRest:" + _drawIntervalRest);
                        BlinkPInvoke.mbSetDebugConfig(BlinkHandle, _strdrawMinInterval, _drawIntervalRest);
                    }));
                }
            }
            else
            {
            }
            if (DocumentReady != null)
            {
                DocumentReadyEventArgs ee = new DocumentReadyEventArgs();
                ee.FrameId = frameId;
                DocumentReady(this, ee);
            }

            //if (url != "about:blank" && !IsMainFrame(frameId))
            //{

            //    if (!FrameIdList.ContainsKey(url))
            //    {
            //        FrameIdList.Add(url, frameId);
            //    }
            //    else
            //    {
            //        FrameIdList[url] = frameId;
            //    }
            //}


        }

        void OnNetGetFaviconCallback(IntPtr webView, IntPtr param, IntPtr url, IntPtr buf)
        {
            if (url != IntPtr.Zero && buf != IntPtr.Zero)
            {
                var _url = Common.Utf8IntptrToString(url);
                var data = (mbMemBuf)Marshal.PtrToStructure(buf, typeof(mbMemBuf));
                if (data.length != 0)
                {
                    try
                    {
                        byte[] ys = new byte[data.length];
                        Marshal.Copy(data.data, ys, 0, data.length);
                        Image img = Common.byteToImage(ys);
                        if (FaviconCallback != null)
                        {
                            FaviconCallback(Common.Utf8IntptrToString(url), img);
                        }
                    }
                    catch (Exception)
                    {
                        //如果网站没有设置icon buf  是404页面而不是图片
                        //byte[] ys = new byte[data.length];
                        //Marshal.Copy(data.data, ys, 0, data.length);
                        //File.WriteAllBytes("err.icon",ys);
                    }
                }
            }


        }

        /// <summary>
        /// JS回调，当执行window.mbQuery时执行
        /// </summary>
        /// <param name="webView"></param>
        /// <param name="param"></param>
        /// <param name="es"></param>
        /// <param name="queryId"></param>
        /// <param name="customMsg"></param>
        /// <param name="request"></param>
        void OnJsQueryCallback(IntPtr webView, IntPtr param, IntPtr es, Int64 queryId, int customMsg, IntPtr request)
        {
            if (GlobalObjectJs == null)
            {
                GlobalObjectJs = this;
            }

            var att = GlobalObjectJs.GetType().GetMethods();
            //att.First(k=>k.GetCustomAttributes(typeof(JSFunctin), true)
            foreach (var item in att)
            {
                var xx = item.GetCustomAttributes(typeof(JSFunctin), true);
                if (xx != null && xx.Length != 0)
                {
                    if (((JSFunctin)xx[0]).CustomMsg == customMsg)
                    {
                        string requestParam = Common.Utf8IntptrToString(request);//参数
                        JObject job = new JObject();

                        var xp = item.GetParameters();
                        //var res = new object();
                        Func<object> func = null;
                        if (xp != null && xp.Length != 0)
                        {
                            if (xp.Length > 1)
                            {
                                job = JObject.Parse(requestParam);
                            }
                            #region 带参数的
                            object[] listParam = new object[xp.Length];
                            for (int i = 0; i < xp.Length; i++)
                            {
                                Type tType = xp[i].ParameterType;

                                if (tType == typeof(int))
                                {
                                    if (xp.Length > 1)
                                    {
                                        if (job[xp[i].Name] != null)
                                        {
                                            listParam[i] = job[xp[i].Name].Value<int>();
                                        }
                                    }
                                    else
                                    {
                                        listParam[i] = Convert.ChangeType(request.ToInt32(), tType);
                                    }
                                }
                                else if (tType == typeof(float))
                                {
                                    if (xp.Length > 1)
                                    {
                                        listParam[i] = job[xp[i].Name].Value<float>();
                                    }
                                    else
                                    {
                                        GCHandle gcHandle = GCHandle.FromIntPtr(request);
                                        listParam[i] = Convert.ChangeType((float)gcHandle.Target, tType);
                                        gcHandle.Free();
                                    }
                                }
                                else if (tType == typeof(bool))
                                {
                                    if (xp.Length > 1)
                                    {
                                        listParam[i] = job[xp[i].Name].Value<bool>();
                                    }
                                    else
                                    {
                                        GCHandle gcHandle = GCHandle.FromIntPtr(request);
                                        listParam[i] = Convert.ChangeType((bool)gcHandle.Target, tType);
                                    }
                                }
                                else
                                {
                                    if (xp.Length > 1)
                                    {
                                        if (job[xp[i].Name] != null)
                                        {
                                            listParam[i] = job[xp[i].Name].Value<string>();
                                        }
                                        else
                                        {
                                            listParam[i] = null;
                                        }
                                    }
                                    else
                                    {
                                        listParam[i] = Convert.ChangeType(Common.Utf8IntptrToString(request), tType);
                                    }
                                }
                            }
                            #endregion
                            try
                            {
                                func = new Func<object>(() =>
                                {
                                    object invokeResult = null;
                                    IAsyncResult iAsyncInvoke = this.BeginInvoke(new MethodInvoker(() =>
                                    {
                                        invokeResult = item.Invoke(GlobalObjectJs, listParam);
                                    }));
                                    Application.DoEvents();
                                    if (IsDisposed || !this.Parent.IsHandleCreated)
                                    {

                                    }
                                    else
                                    {
                                        this.EndInvoke(iAsyncInvoke);
                                    }
                                    return invokeResult;
                                });
                            }
                            catch (Exception)
                            {
                            }
                        }
                        else
                        {//不带参数
                            try
                            {
                                func = new Func<object>(() =>
                                {
                                    object invokeResult = null;
                                    IAsyncResult iAsyncInvoke = this.BeginInvoke(new MethodInvoker(() =>
                                    {
                                        invokeResult = item.Invoke(GlobalObjectJs, null);
                                    }));

                                    this.EndInvoke(iAsyncInvoke);
                                    return invokeResult;
                                });
                            }
                            catch (Exception)
                            {
                            }
                        }

                        IAsyncResult iAsyncResult = func.BeginInvoke(new AsyncCallback((result) =>
                        {
                            if (result != null)
                            {
                                try
                                {
                                    Func<object> func1 = (Func<object>)((AsyncResult)result).AsyncDelegate;
                                    if (func1 != null)
                                    {
                                        object res = func1.EndInvoke(result);

                                        if (res != null)
                                        {
                                            IntPtr intPtr = IntPtr.Zero;

                                            if (res.GetType() == typeof(bool) ||
                                                res.GetType() == typeof(int) ||
                                                res.GetType() == typeof(float) ||
                                                res.GetType() == typeof(double) ||
                                                res.GetType() == typeof(decimal)
                                                )
                                            {
                                                int nSizeOfPerson = Marshal.SizeOf(res);
                                                intPtr = Marshal.AllocHGlobal(nSizeOfPerson);
                                                Marshal.StructureToPtr(res, intPtr, true);
                                            }
                                            else if (res.GetType() == typeof(string))
                                            {
                                                intPtr = Common.Utf8StringToIntptr(res.ToString());
                                            }

                                            BlinkPInvoke.mbResponseQuery(webView, queryId, customMsg, intPtr);
                                            //Marshal.FreeHGlobal(intPtr);
                                            return;
                                        }
                                        else
                                        {
                                            //无返回值时也回调，返回false
                                            int nSizeOfPerson = Marshal.SizeOf(false);
                                            IntPtr intPtr = Marshal.AllocHGlobal(nSizeOfPerson);
                                            Marshal.StructureToPtr(false, intPtr, true);

                                            BlinkPInvoke.mbResponseQuery(webView, queryId, customMsg, intPtr);
                                            //Marshal.FreeHGlobal(intPtr);
                                            return;
                                        }
                                    }
                                }
                                catch (Exception)
                                {
                                }
                            }

                        }), null);
                        return;
                    }
                }
            }
        }


        void OnTitleChangedCallback(IntPtr webView, IntPtr param, IntPtr title)
        {
            _title = Common.Utf8IntptrToString(title);
            if (TitleChanged != null)
            {
                TitleChanged(this, new EventArgs());
            }
        }
        void OnAlterCallback(IntPtr webView, IntPtr param, IntPtr msg)
        {

        }

        bool OnwkeNavigationCallback(IntPtr webView, IntPtr param, mbNavigationType navigationType, IntPtr urlPtr)
        {

            download_url = null;
            url = Common.Utf8IntptrToString(urlPtr);


            if (Navigating != null)
            {
                NavigationEventArgs ee = new NavigationEventArgs();
                ee.Continue = true;
                ee.NavigationType = (mbNavigationType)navigationType;
                ee.Url = Common.Utf8IntptrToString(urlPtr);
                Navigating(this, ee);

                return ee.Continue;
            }
            return true;
            //Console.WriteLine($"{DateTime.Now} Navigation：{url}");

        }

        void OnUrlChangedCallback(IntPtr webView, IntPtr param, IntPtr url, bool canGoBack, bool canGoForward)
        {
            _canGoForward = canGoForward;
            _canGoBack = canGoBack;

            string nowURL = Common.Utf8IntptrToString(url);
            this.url = nowURL;
            if (UrlChanged != null)
            {
                UrlChanged(this, new EventArgs());
            }
        }
        #endregion
        /// <summary>
        /// 重新加载
        /// </summary>
        public void Reload()
        {
            if (BlinkHandle != IntPtr.Zero)
            {
                this.MbInvoke(new MethodInvoker(() =>
                {
                    BlinkPInvoke.mbReload(BlinkHandle);
                }));
            }
        }
        /// <summary>
        /// 强制重新加载
        /// </summary>
        public void ReloadNoCache()
        {
            if (BlinkHandle != IntPtr.Zero)
            {
                this.MbInvoke(new MethodInvoker(() =>
                {
                    ResourceGcIntervalSec = 0;
                    Thread.Sleep(300);
                    GC.Collect();
                    Application.DoEvents();
                    ResourceGcIntervalSec = 3600;
                    BlinkPInvoke.mbReload(BlinkHandle);
                }));
            }
        }
        //#if !DEBUG
        /// <summary>
        /// 显示调试工具
        /// </summary>
        public void ShowDevTools()
        {

            if (BlinkHandle != IntPtr.Zero)
            {
                string path = System.AppDomain.CurrentDomain.BaseDirectory;
                string dev = Path.Combine(path, "front_end\\inspector.html");
                if (File.Exists(dev))
                {
                    this.BeginInvoke(new MethodInvoker(() =>
                    {
                        string param = Uri.EscapeUriString(path + "\\front_end\\inspector.html").Replace("%20", " ").Replace("%5C", "\\");
                        //IntPtr intPtr =  Marshal.StringToCoTaskMemAnsi("showDevTools");
                        BlinkPInvoke.mbSetDebugConfig(BlinkHandle, "showDevTools", param);
                    }));
                }
            }

        }
        //#endif

        private static void ResNotFond(string url, IntPtr job)
        {
            string data = $"<html><head>404没有找到资源<title>404没有找到资源</title></head><body>没有找到资源<br />{url}</body></html>";
            BlinkPInvoke.mbNetSetMIMEType(job, Marshal.StringToCoTaskMemAnsi("text/html"));
            BlinkPInvoke.mbNetSetData(job, Encoding.Default.GetBytes(data), Encoding.Default.GetBytes(data).Length);
        }

        /// <summary>
        /// 加载url或者文件
        /// </summary>
        /// <param name="url"></param>
        public void LoadUrl(string url)
        {
            if (string.IsNullOrEmpty(url))
            {
                return;
            }
            if (BlinkHandle != IntPtr.Zero)
            {
                this.MbInvoke(new MethodInvoker(() =>
                {
                    DocumentIsReady = false;
                    _isLoadingCompleted = false;
                    IntPtr jsPtr = Common.Utf8StringToIntptr(url);
                    BlinkPInvoke.mbLoadURL(BlinkHandle, jsPtr);
                    //Marshal.FreeCoTaskMem(jsPtr);
                }));
            }
        }
        /// <summary>
        /// 没网络或断网情况
        /// </summary>
        /// <param name="url"></param>
        /// <param name="errCode"></param>
        /// <returns></returns>
        string NoNetwork(string url, string errCode, int code = 7)
        {
            System.Reflection.Assembly Assemblys = BlinkPInvoke.ResourceAssemblys["TZ.BrowserMain"];

            using (Stream sm = Assemblys.GetManifestResourceStream($"TZ.BrowserMain.Res.nonetwork{code}.html"))
            {
                if (sm != null)
                {
                    StreamReader m_stream = new StreamReader(sm, Encoding.Default);
                    m_stream.BaseStream.Seek(0, SeekOrigin.Begin);
                    string strLine = m_stream.ReadToEnd();
                    m_stream.Close();

                    strLine = strLine.Replace("#title#", url);

                    strLine = strLine.Replace("#errCode#", errCode);
                    return strLine;
                }
            }
            return string.Empty;
        }

        public bool IsMainFrame(IntPtr frameId)
        {
            bool mainframeId = false;
            if (BlinkHandle != IntPtr.Zero)
            {
                this.MbInvoke(new MethodInvoker(() =>
                {
                    mainframeId = BlinkPInvoke.mbIsMainFrame(BlinkHandle, frameId);
                }));
            }
            return mainframeId;
        }
        #region 保证在主线程执行的方法
        /// <summary>
        /// 保证在主线程执行的方法
        /// </summary>
        /// <param name="method"></param>
        public void MbInvoke(Delegate method)
        {
            if (!IsEnable())
            {
                return;
            }
            Application.DoEvents();

            this.Invoke(method);
        }
        private bool IsEnable()
        {
            if (BlinkHandle == IntPtr.Zero || !this.IsHandleCreated || this.IsDisposed || this.Disposing)
            {
                return false;
            }
            else
            {
                return true;
            }
        }
        private void BindJsFunc(IntPtr frameId = default(IntPtr))
        {
            if (frameId == default(IntPtr))
            {
                frameId = MainFrameId;
            }
            if (GlobalObjectJs == null)
            {
                GlobalObjectJs = this;
            }
            StringBuilder stringBuilder = new StringBuilder();
            var att = GlobalObjectJs.GetType().GetMethods();

            foreach (var item in att)
            {
                var xx = item.GetCustomAttributes(typeof(JSFunctin), true);
                if (xx != null && xx.Length != 0)
                {
                    int customMsg = ((JSFunctin)xx[0]).CustomMsg;
                    MethodInfo methodInfo = item;
                    var xp = methodInfo.GetParameters();

                    //异步调用
                    #region 是异步调用时
                    if (xp != null && xp.Length != 0)
                    {//有参数
                        if (xp.Length > 1)
                        {//参数大于1个时
                            List<string> strParam = new List<string>();
                            List<string> strJson = new List<string>();
                            for (int i = 0; i < xp.Length; i++)
                            {
                                strParam.Add(xp[i].Name);
                                strJson.Add("\"" + xp[i].Name + "\":" + xp[i].Name);
                            }
                            stringBuilder.Append(string.Concat(new string[]
                                {
                                    "window.",
                                    methodInfo.Name,
                                    "=function("+string.Join( ",", strParam.ToArray())+"){var promise = new Promise(function(resolve, reject) {var json={"+string.Join(",", strJson.ToArray())+"}; window.mbQuery(",
                                    customMsg.ToString(),
                                    ", JSON.stringify(json), function(a,b){ resolve(b); }); }); return promise; } ; "
                                }));
                        }
                        else
                        {//一个参数时
                            stringBuilder.Append(string.Concat(new string[]
                                {
                                    "window.",
                                    methodInfo.Name,
                                    "=function(json){var promise = new Promise(function(resolve, reject) { window.mbQuery(",
                                    customMsg.ToString(),
                                    ", json, function(a,b){ resolve(b); }); }); return promise; } ; "
                                }));
                        }
                    }
                    else
                    {//无参数
                        stringBuilder.Append(string.Concat(new string[]
                                {
                                    "window.",
                                    methodInfo.Name,
                                    "=function(){var promise = new Promise(function(resolve, reject) { window.mbQuery(",
                                   customMsg.ToString(),
                                    ", null, function(a,b){ resolve(b); });}); return promise; } ; "
                                }));
                    }
                    #endregion
                }
            }

            if (stringBuilder.ToString().Trim() != string.Empty)
            {
                InvokeJSWVoid(stringBuilder.ToString().Trim(), frameId);
            }
        }
        /// <summary>
        /// 执行js，不返回结果
        /// </summary>
        /// <param name="js"></param>
        public void InvokeJSWVoid(string js)
        {
            this.MbInvoke(new MethodInvoker(() =>
            {
                Guid _ID = Guid.NewGuid();
                mbRunJsCallback callback = null;
                callback = new mbRunJsCallback((webView, param, es, v) =>
                {
                    var pass = Common.Utf8IntptrToString(param);
                    BlinkInit.Instance.RemoveAction(new ActionModel { ID = new Guid(pass) });
                });
                BlinkInit.Instance.AddAction(new ActionModel { ID = _ID, ObjData = callback });

                IntPtr jsPtr = Common.Utf8StringToIntptr(js);
                IntPtr para = Common.Utf8StringToIntptr(_ID.ToString());

                BlinkPInvoke.mbRunJs(BlinkHandle, MainFrameId, jsPtr, false, callback, para, IntPtr.Zero);
            }));
        }
        public void InvokeJSWVoid(string js, IntPtr frameId)
        {
            if (IsDisposed || !this.Parent.IsHandleCreated) return;

            this.MbInvoke(new MethodInvoker(() =>
        {
            Guid _ID = Guid.NewGuid();
            mbRunJsCallback callback = null;
            callback = new mbRunJsCallback((webView, param, es, v) =>
            {
                var pass = Common.Utf8IntptrToString(param);
                BlinkInit.Instance.RemoveAction(new ActionModel { ID = new Guid(pass) });
            });
            BlinkInit.Instance.AddAction(new ActionModel { ID = _ID, ObjData = callback });

            IntPtr jsPtr = Common.Utf8StringToIntptr(js);
            IntPtr para = Common.Utf8StringToIntptr(_ID.ToString());

            BlinkPInvoke.mbRunJs(BlinkHandle, frameId, jsPtr, false, callback, para, IntPtr.Zero);
        }));
        }
        public string Url
        {
            get { return url; }
            set
            {
                url = value;
                if (BlinkHandle != IntPtr.Zero)
                {
                    LoadUrl(url);// 加载url或者文件
                }
            }
        }
        /// <summary>
        /// 清除所有Cookie
        /// </summary>
        public void ClearAllCookie()
        {
            if (BlinkHandle != IntPtr.Zero)
            {
                this.Invoke((EventHandler)delegate
                {
                    BlinkPInvoke.mbPerformCookieCommand(BlinkHandle, mbCookieCommand.mbCookieCommandClearAllCookies);
                });
            }
        }
       

        /// <summary>
        /// 解析 cookies.dat文件得到Cookie,没有判断 path，只有 域的判断
        /// </summary>
        public string GetCookiesFromFile
        {
            get
            {
                StringBuilder sbCookie = new StringBuilder();
                if (File.Exists(CookiePath + "\\cookies.dat"))
                {

                    var uri = new Uri(Url);
                    var host = uri.Host;
                    FileStream fs = new FileStream(CookiePath + "\\cookies.dat", FileMode.Open, FileAccess.Read, FileShare.ReadWrite);
                    StreamReader sr = new StreamReader(fs, System.Text.Encoding.Default);
                    List<string> ls = new List<string>();
                    while (!sr.EndOfStream)
                    {
                        ls.Add(sr.ReadLine());
                    }

                    var allCookies = ls.ToArray();// File.ReadAllLines(CookiePath + "\\cookie.dat");
                    for (int i = 4; i < allCookies.Length; i++)
                    {
                        host = uri.Host;
                        var listCookie = allCookies[i].Split('\t');
                        if (listCookie != null && listCookie.Length != 0 && listCookie.Length == 7)
                        {
                            var _cookie = listCookie[0];

                        Lable:
                            if (_cookie == host)
                            {
                                sbCookie.AppendFormat("{0}={1};", listCookie[5], listCookie[6]);
                            }
                            //httponly
                            var httpOnly = "#HttpOnly_" + host;
                            if (_cookie == httpOnly)
                            {
                                sbCookie.AppendFormat("{0}={1};", listCookie[5], listCookie[6]);
                            }
                            if (host.IndexOf('.') == 0)// . 开头
                            {
                                host = host.Substring(host.IndexOf('.') + 1);//. 开头 去掉 .
                                goto Lable;
                            }
                            else
                            {
                                if (host.TrimStart('.').Split('.').Length > 2)
                                {
                                    host = host.Substring(host.IndexOf('.'));//带 . 
                                    goto Lable;
                                }
                            }
                        }

                    }
                }
                return sbCookie.ToString();
            }
        }

        /// <summary>
        /// 取 cookie
        /// </summary>
        /// <param name="name">默认全取，否则按照KEY返回</param>
        /// <returns></returns>
        public string GetCookie2(string name = "")
        {
            string cc = string.Empty;
            if (BlinkHandle != IntPtr.Zero)
            {
                bool isend = false;
                mbGetCookieCallback mbGetCookieCallback = new mbGetCookieCallback((a, b, c, d) =>
                {
                    if (c == MbAsynRequestState.kMbAsynRequestStateOk)
                    {
                        cc = Common.Utf8IntptrToString(d);
                    }
                    isend = true;
                });
                //BlinkJSAPI.mainForm.Invoke((EventHandler)delegate
                //{
                BlinkPInvoke.mbGetCookie(BlinkHandle, mbGetCookieCallback, IntPtr.Zero);
                //});
                do
                {
                    Thread.Sleep(200);
                    Application.DoEvents();
                } while (!isend);
                if (string.IsNullOrEmpty(cc))
                {
                    return string.Empty;
                }
                if (name == "")
                {
                    return cc;
                }
                else
                {
                    var ccList = cc.Split(';');
                    foreach (var item in ccList)
                    {
                        if (string.IsNullOrEmpty(item) || item.Trim().Length == 0)
                        {
                            continue;
                        }
                        var si = item.Split('=');
                        if (si[0].Trim() == name && si.Length == 2)
                        {
                            return si[1];
                        }
                    }
                }
            }
            return cc;
        }
        public string HTML
        {
            //get
            //{
            //    if (BlinkHandle != IntPtr.Zero)
            //    {
            //        //Action<IntPtr, IntPtr, IntPtr> ac =
            //        string _html = string.Empty;
            //        mbGetMHTMLCallback callback = delegate (IntPtr webView, IntPtr param, IntPtr mthml)
            //        {
            //            _html = Utf8IntptrToString(mthml);
            //        };
            //        BlinkPInvoke.mbUtilSerializeToMHTML(BlinkHandle, callback, IntPtr.Zero);
            //    }
            //    return string.Empty;
            //}
            set
            {
                if (BlinkHandle != IntPtr.Zero)
                {
                    if (string.IsNullOrEmpty(value))
                    {
                        value = "<html><head></head><body></body></html>";
                    }
                    if (string.IsNullOrEmpty(url))
                    {
                        url = "tz://error";
                    }
                    BlinkPInvoke.mbLoadHtmlWithBaseUrl(BlinkHandle, Common.Utf8StringToIntptr(value), Common.Utf8StringToIntptr(url));
                }
            }
        }


        private string _title;
        /// <summary>
        /// 浏览器标题
        /// </summary>
        public string Title
        {
            get { return _title; }
            set { _title = value; }
        }

        private bool _canGoBack;
        /// <summary>
        /// 是否可以后退
        /// </summary>
        public bool CanGoBack
        {
            get { return _canGoBack; }
        }
        public void GoBack()
        {
            if (CanGoBack && BlinkHandle != IntPtr.Zero)
            {
                this.MbInvoke(new MethodInvoker(() =>
                {
                    BlinkPInvoke.mbGoBack(BlinkHandle);
                }));
            }
        }
        public void GoForward()
        {
            if (CanGoForward && BlinkHandle != IntPtr.Zero)
            {
                this.MbInvoke(new MethodInvoker(() =>
                {
                    BlinkPInvoke.mbGoForward(BlinkHandle);
                }));
            }
        }
        /// <summary>
        /// 释放Blink
        /// </summary>
        public void ExitBlink()
        {
            if (BlinkHandle != IntPtr.Zero)
            {
                BlinkPInvoke.mbDestroyWebView(BlinkHandle);
                this.Dispose();
            }
        }
        private bool _canGoForward;
        /// <summary>
        /// 是否可以向前
        /// </summary>
        public bool CanGoForward
        {
            get { return _canGoForward; }
        }
        private bool _isLoadingFailed;
        /// <summary>
        /// 页面是否加载失败
        /// </summary>
        public bool IsLoadingFailed
        {
            get
            {
                return _isLoadingFailed;
            }
        }

        public bool DocumentIsReady { get; set; } = false;
        private bool _isLoadingSucceeded;
        /// <summary>
        /// 页面是否加载成功
        /// </summary>
        public bool IsLoadingSucceeded
        {
            get
            {
                return _isLoadingSucceeded;
            }
        }

        private bool _isLoadingCompleted;
        public bool IsLoadingCompleted
        {
            get
            {

                return _isLoadingCompleted;
            }
        }
        private int _ResourceGcIntervalSec = 3600;
        /// <summary>
        /// 资源回收间隔(秒)
        /// </summary>
        public int ResourceGcIntervalSec
        {
            get { return _ResourceGcIntervalSec; }
            set
            {
                _ResourceGcIntervalSec = value;
                BlinkPInvoke.mbSetResourceGc(BlinkHandle, value);//设置缓存回收间隔
            }
        }


        //private string _cookiePath = Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData) + "\\TZBlowser\\";
        /// <summary>
        /// Cookie保存路径
        /// </summary>
        public string CookiePath
        {
            get { return BlinkCommon.CookiePath; }
            //private set
            //{
            //    _cookiePath = value;
            //    if (BlinkHandle != IntPtr.Zero)
            //    {
            //        if (_cookiePath != value && !string.IsNullOrEmpty(value))
            //        {
            //            if (!Directory.Exists(_cookiePath))
            //            {
            //                Directory.CreateDirectory(_cookiePath);
            //            }
            //            this.MbInvoke(new MethodInvoker(() =>
            //            {
            //                BlinkPInvoke.mbSetCookieJarPath(BlinkHandle, _cookiePath);
            //            }));
            //        }
            //    }
            //}
        }
        //private string _userAgent = "Mozilla/5.0 (Windows NT 6.1) AppleWebKit/537.36 (KHTML, like Gecko)TZBROWSER/1.0 Chrome/70.0.3538.102 Safari/537.36";
        ///// <summary>
        ///// 获取或设置浏览器标识
        ///// </summary>
        //public string UserAgent
        //{
        //    get { return _userAgent; }
        //    set
        //    {
        //        _userAgent = value;
        //        if (value != string.Empty)
        //        {
        //            this.MbInvoke(new MethodInvoker(() =>
        //            {
        //                BlinkPInvoke.mbSetUserAgent(BlinkHandle, Common.Utf8StringToIntptr(value));
        //            }));
        //        }
        //    }
        //}

        /// <summary>
        /// 缩放比例
        /// </summary>
        public float ZoomFactor
        {
            private get
            {
                return 1;
            }
            set
            {
                BlinkPInvoke.mbSetZoomFactor(BlinkHandle, (float)value);
            }
        }

        #endregion
        #region 浏览器事件委托
        [Category("Browser")]
        public event EventHandler UrlChanged;
        [Category("Browser")]
        public event EventHandler<LoadingFinishEventArgs> LoadingFinish;

        [Category("Browser")]
        public event EventHandler<DocumentReadyEventArgs> DocumentReady;

        [Category("Browser")]
        public event EventHandler TitleChanged;
        [Category("Browser")]
        public event EventHandler<NavigationEventArgs> Navigating;
        [Category("Browser")]
        public event EventHandler<NewWindowEventArgs> NewWindow;
        [Category("Browser")]
        public event EventHandler<AlertBoxEventArgs> AlertBox;
        //[Category("Browser")]
        //public event EventHandler<PromptBoxEventArgs> PromptBox;

        public delegate bool OnConfirmBoxCall(string msg);
        [Category("Browser")]
        public event OnConfirmBoxCall ConfirmBox;

        [Category("Browser")]
        public delegate void OnGetFaviconCallback(string url, Image image);
        public event OnGetFaviconCallback FaviconCallback;

        //[Category("Browser")]
        //public event EventHandler<ConsoleMessageEventArgs> ConsoleMessage;
        [Category("Browser")]
        public event EventHandler<DownloadEventArgs> Download;
        [Category("Browser")]
        public event EventHandler<ReadDataEventArgs> ReadData;
        /// <summary>
        /// 下载完成
        /// </summary>
        [Category("Browser")]
        public event EventHandler<DownloadFilshEvent> DownloadFilsh;

        #endregion

   

        public bool PreFilterMessage(ref Message m)
        {
            IntPtr myPtr = WinAPI.GetForegroundWindow();
            StringBuilder sb = new StringBuilder(256);
            int length = WinAPI.GetClassName(myPtr, sb, sb.Capacity);
            if (sb.ToString() == "MtMbWebWindow")//打印窗体的className
            {
                int wp = m.WParam.ToInt32();
                if (wp == 13/*回车*/ || wp == 37/*左 */ || wp == 39/*右*/ || wp == 38/*上*/ || wp == 40/*下*/)
                {
                    WinAPI.SendMessage(m.HWnd, m.Msg, m.WParam.ToInt32(), m.LParam.ToInt32());
                }
                if (m.Msg == 258)//字符
                {
                    WinAPI.SendMessage(m.HWnd, m.Msg, m.WParam.ToInt32(), m.LParam.ToInt32());
                }
            }
            return false;
        }
      
    }
}
