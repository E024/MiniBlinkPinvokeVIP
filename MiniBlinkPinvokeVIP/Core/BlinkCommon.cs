using System;
using System.Collections.Generic;
using System.Runtime.InteropServices;
using System.Text;

namespace MiniBlinkPinvokeVIP.Core
{
    public static class BlinkCommon
    {  /// <summary>
       /// cookie 保存路径
       /// </summary>
        public static string CookiePath
        {
            get
            {
                return Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData) + "\\MiniBlink\\";
            }
        } /// <summary>
          /// 资源缓存路径
          /// </summary>
        public static string CachePath
        {
            get
            {
                return Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData) + "\\MiniBlink\\Cache\\";
            }
        }
        /// <summary>
        /// 设置抗锯齿渲染。必须设置为"1"
        /// </summary>
        public static IntPtr BlinkAntiAlias
        {
            get
            {
                return Common.Utf8StringToIntptr("1");
            }
        }
        /// <summary>
        /// 最小字体 默认 12
        /// </summary>
        public static IntPtr BlinkMinimumFontSize
        {
            get
            {
                return Common.Utf8StringToIntptr("12");
            }
        }

        /// <summary>
        /// 最小逻辑字体 默认 12
        /// </summary>
        public static IntPtr BlinkMinimumLogicalFontSize
        {
            get
            {
                return Common.Utf8StringToIntptr("12");
            }
        }
        /// <summary>
        /// LocalStorage 保存路径
        /// </summary>
        internal static string LocalStoragePath
        {
            get
            {
                return Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData) + "\\MiniBlink\\LocalStorage\\";
            }
        }

        /// <summary>
        /// 默认字体大小 默认 12
        /// </summary>
        public static IntPtr BlinkDefaultFixedFontSize
        {
            get
            {
                return Common.Utf8StringToIntptr("12");
            }
        }
        public static IntPtr JavascriptMIME
        {
            get
            {
                return Marshal.StringToCoTaskMemAnsi("application/javascript");
            }
        }
        public static IntPtr CssMIME
        {
            get
            {
                return Marshal.StringToCoTaskMemAnsi("text/css");
            }
        }
        /// <summary>
        /// 浏览器 UserAgent
        /// </summary>
        internal static string BlinkUserAgent
        {
            get
            {
                return "Mozilla/5.0 (Windows NT 6.1) AppleWebKit/537.36 (KHTML, like Gecko) MiniBlink/1.0 Chrome/70.0.3538.102 Safari/537.36";
            }
        }
    }
}
