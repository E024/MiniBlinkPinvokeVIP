using System;
using System.Collections.Generic;
using System.Text;

namespace TZ.BrowserMain.BrowserCore
{
    public enum mbProxyType
    {
        MB_PROXY_NONE,
        MB_PROXY_HTTP,
        MB_PROXY_SOCKS4,
        MB_PROXY_SOCKS4A,
        MB_PROXY_SOCKS5,
        MB_PROXY_SOCKS5HOSTNAME,
    }
    public enum mbSettingMask
    {
        MB_SETTING_PROXY = 1,
        MB_SETTING_PAINTCALLBACK_IN_OTHER_THREAD = 1 << 2,
    }
    public enum wkeMouseMessage : uint
    {
        WKE_MSG_MOUSEMOVE = 0x0200,
        WKE_MSG_LBUTTONDOWN = 0x0201,
        WKE_MSG_LBUTTONUP = 0x0202,
        WKE_MSG_LBUTTONDBLCLK = 0x0203,
        WKE_MSG_RBUTTONDOWN = 0x0204,
        WKE_MSG_RBUTTONUP = 0x0205,
        WKE_MSG_RBUTTONDBLCLK = 0x0206,
        WKE_MSG_MBUTTONDOWN = 0x0207,
        WKE_MSG_MBUTTONUP = 0x0208,
        WKE_MSG_MBUTTONDBLCLK = 0x0209,
        WKE_MSG_MOUSEWHEEL = 0x020A,
    }
    public enum mbMouseMsg : uint
    {
        MB_MSG_MOUSEMOVE = 0x0200,
        MB_MSG_LBUTTONDOWN = 0x0201,
        MB_MSG_LBUTTONUP = 0x0202,
        MB_MSG_LBUTTONDBLCLK = 0x0203,
        MB_MSG_RBUTTONDOWN = 0x0204,
        MB_MSG_RBUTTONUP = 0x0205,
        MB_MSG_RBUTTONDBLCLK = 0x0206,
        MB_MSG_MBUTTONDOWN = 0x0207,
        MB_MSG_MBUTTONUP = 0x0208,
        MB_MSG_MBUTTONDBLCLK = 0x0209,
        MB_MSG_MOUSEWHEEL = 0x020A,
    }
    public enum wkeMouseFlags
    {
        WKE_LBUTTON = 0x01,
        WKE_RBUTTON = 0x02,
        WKE_SHIFT = 0x04,
        WKE_CONTROL = 0x08,
        WKE_MBUTTON = 0x10,
    }

    public enum wkeJSType
    {
        JSTYPE_NUMBER,
        JSTYPE_STRING,
        JSTYPE_BOOLEAN,
        JSTYPE_OBJECT,
        JSTYPE_FUNCTION,
        JSTYPE_UNDEFINED
    }
    public enum wkeMessageSource
    {
        WKE_MESSAGE_SOURCE_HTML,
        WKE_MESSAGE_SOURCE_XML,
        WKE_MESSAGE_SOURCE_JS,
        WKE_MESSAGE_SOURCE_NETWORK,
        WKE_MESSAGE_SOURCE_CONSOLE_API,
        WKE_MESSAGE_SOURCE_OTHER
    }
    public enum wkeMessageType
    {
        WKE_MESSAGE_TYPE_LOG,
        WKE_MESSAGE_TYPE_DIR,
        WKE_MESSAGE_TYPE_DIR_XML,
        WKE_MESSAGE_TYPE_TRACE,
        WKE_MESSAGE_TYPE_START_GROUP,
        WKE_MESSAGE_TYPE_START_GROUP_COLLAPSED,
        WKE_MESSAGE_TYPE_END_GROUP,
        WKE_MESSAGE_TYPE_ASSERT
    }
    public enum wkeMessageLevel
    {
        WKE_MESSAGE_LEVEL_TIP,
        WKE_MESSAGE_LEVEL_LOG,
        WKE_MESSAGE_LEVEL_WARNING,
        WKE_MESSAGE_LEVEL_ERROR,
        WKE_MESSAGE_LEVEL_DEBUG
    }
    public enum mbConsoleLevel
    {
        mbLevelDebug = 4,
        mbLevelLog = 1,
        mbLevelInfo = 5,
        mbLevelWarning = 2,
        mbLevelError = 3,
        mbLevelRevokedError = 6,
        mbLevelLast = mbLevelInfo
    }
    public enum wkeNavigationAction
    {
        WKE_NAVIGATION_CONTINUE,
        WKE_NAVIGATION_ABORT,
        WKE_NAVIGATION_DOWNLOAD
    }
    public enum mbNavigationType
    {
        MB_NAVIGATION_TYPE_LINKCLICK,
        MB_NAVIGATION_TYPE_FORMSUBMITTE,
        MB_NAVIGATION_TYPE_BACKFORWARD,
        MB_NAVIGATION_TYPE_RELOAD,
        MB_NAVIGATION_TYPE_FORMRESUBMITT,
        MB_NAVIGATION_TYPE_OTHER
    }

    public enum WkeCursorInfo
    {
        WkeCursorInfoPointer = 0,
        WkeCursorInfoCross = 1,
        WkeCursorInfoHand = 2,
        WkeCursorInfoIBeam = 3,
        WkeCursorInfoWait = 4,
        WkeCursorInfoHelp = 5,
        WkeCursorInfoEastResize = 6,
        WkeCursorInfoNorthResize = 7,
        WkeCursorInfoNorthEastResize = 8,
        WkeCursorInfoNorthWestResize = 9,
        WkeCursorInfoSouthResize = 10,
        WkeCursorInfoSouthEastResize = 11,
        WkeCursorInfoSouthWestResize = 12,
        WkeCursorInfoWestResize = 13,
        WkeCursorInfoNorthSouthResize = 14,
        WkeCursorInfoEastWestResize = 15,
        WkeCursorInfoNorthEastSouthWestResize = 16,
        WkeCursorInfoNorthWestSouthEastResize = 17,
        WkeCursorInfoColumnResize = 18,
        WkeCursorInfoRowResize = 19,
    }
    public enum mbLoadingResult
    {
        MB_LOADING_SUCCEEDED,
        MB_LOADING_FAILED,
        MB_LOADING_CANCELED
    }



    public enum mbCookieCommand
    {
        mbCookieCommandClearAllCookies,
        mbCookieCommandClearSessionCookies,
        mbCookieCommandFlushCookiesToFile,
        mbCookieCommandReloadCookiesFromFile,
    }
    public enum mbKeyFlags
    {
        MB_EXTENDED = 0x0100,
        MB_REPEAT = 0x4000,
    }
    public enum mbMouseFlags
    {
        MB_LBUTTON = 0x01,
        MB_RBUTTON = 0x02,
        MB_SHIFT = 0x04,
        MB_CONTROL = 0x08,
        MB_MBUTTON = 0x10,
    }

    public enum MbAsynRequestState
    {
        kMbAsynRequestStateOk = 0,
        kMbAsynRequestStateFail = 1,
    }
    public enum mbDownloadOpt
    {
        kMbDownloadOptCancel,
        kMbDownloadOptCacheData,
    }
}
