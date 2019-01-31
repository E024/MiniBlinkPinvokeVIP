using System;
using System.Collections.Generic;
using System.Text;

namespace TZ.BrowserMain.BrowserCore
{
    public class JSFunctin : Attribute
    {
        private int _customMsg;
        /// <summary>
        /// 消息ID，不能重复
        /// </summary>
        public int CustomMsg
        {
            get { return _customMsg; }
        }
        /// <summary>
        /// 
        /// </summary>
        /// <param name="customMsg">消息ID，不能重复</param>
        /// <param name="isAsy">是否异步</param>
        public JSFunctin(int customMsg)
        {
            _customMsg = customMsg;
        }
    }
}
