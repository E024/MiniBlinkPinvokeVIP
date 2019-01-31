using MiniBlinkPinvokeVIP.Model;
using System;
using System.Collections.Generic;
using System.Text;

namespace TZ.BrowserMain.BrowserCore
{

    public class AlertBoxEventArgs : EventArgs
    {
        public bool Handled { get; set; }
        public string Message { get; set; }
    }
    public class ConfirmBoxEventArgs : EventArgs
    {
        public string Message { get; set; }
    }

    public class ConsoleMessageEventArgs : EventArgs
    {
        public mbConsoleLevel ConsoleLevel { get; set; }
        public bool Handled { get; set; }
        public string Message { get; set; }
        public int SourceLine { get; set; }
        public string SourceName { get; set; }
        public string StackTrace { get; set; }
    }

    public class DocumentReadyEventArgs : EventArgs
    {
        public IntPtr FrameId { get; set; }
    }

    public class DownloadEventArgs : EventArgs
    {
        public bool Handled { get; set; }
        public string Url { get; set; }
    }
    /// <summary>
    /// 下载完成事件
    /// </summary>
    public class DownloadFilshEvent : EventArgs
    {
        public DownloadModel DownloadInfo { get; set; }
        public bool IsError { get; set; }
        public string ErrorInfo { get; set; }
    }
    public class LoadingFinishEventArgs : EventArgs
    {
        public string FailedReason { get; set; }
        public mbLoadingResult LoadingResult { get; set; }
        public string Url { get; set; }
        public IntPtr frameId { get; set; }
    }

    public class NavigationEventArgs : EventArgs
    {
        public bool Continue { get; set; }
        public mbNavigationType NavigationType { get; set; }
        public string Url { get; set; }
    }

    public class NewWindowEventArgs : EventArgs
    {
        public IntPtr MiniBlinkHandle { get; set; }
        public mbNavigationType NavigationType { get; set; }
        public string Url { get; set; }
        public IntPtr WindowFeatures { get; set; }
    }

    public class PromptBoxEventArgs : EventArgs
    {
        public string DefaultResult { get; set; }
        public string Message { get; set; }
        public string Result { get; set; }
        public bool Yes { get; set; }
    }

    public class ReadDataEventArgs : EventArgs
    {
        public byte[] Data { get; set; }
        public string Url { get; set; }
    }
}
