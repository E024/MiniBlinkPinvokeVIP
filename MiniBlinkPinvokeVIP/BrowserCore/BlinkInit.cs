using System;
using System.Collections.Generic;
using System.Text;
using System.Threading;
using System.Linq;
using System.IO;
using MiniBlinkPinvokeVIP.Model;
using MiniBlinkPinvokeVIP.Core;
using System.Windows.Forms;
using System.Runtime.InteropServices;

namespace TZ.BrowserMain.BrowserCore
{
    public class BlinkInit
    {
        #region 属性
        // 定义一个标识确保线程同步
        private static readonly object locker = new object();

        // 定义一个静态变量来保存类的实例
        private static BlinkInit uniqueInstance;

        private Thread thread;//处理mb消息的线程

        private bool _isStop = false;//标识线程是否结束

        private int ThreadId = 0;

        //private ConcurrentQueue<Action> ListQueue = new ConcurrentQueue<Action>();
        #endregion

        #region 单例模式的构造函数
        // 定义私有构造函数，使外界不能创建该类实例
        private BlinkInit()
        {

            //if (!File.Exists("mb.dll"))
            //{
            //    MessageBox.Show("缺少必要文件 mb.dll ，请将 mb.dll 放置软件运行目录后再试。", "错误", MessageBoxButtons.OK, MessageBoxIcon.Error);
            //    Environment.Exit(-1);
            //}
            //if (!File.Exists("node.dll"))
            //{
            //    MessageBox.Show("缺少必要文件 node.dll ，请将 node.dll 放置软件运行目录后再试。", "错误", MessageBoxButtons.OK, MessageBoxIcon.Error);
            //    Environment.Exit(-1);
            //}
            BlinkPInvoke.mbInitWrap(new mbSettings
            {
                mask = mbSettingMask.MB_SETTING_PAINTCALLBACK_IN_OTHER_THREAD,
                blinkThreadInitCallback = IntPtr.Zero,
                blinkThreadInitCallbackParam = IntPtr.Zero
            });

            list_Action = new List<ActionModel> { };
        }
        #endregion

        public mbProxyType GetProxyTypeByName(string name)
        {
            switch (name)
            {
                case "不使用代理":
                    return mbProxyType.MB_PROXY_NONE;
                case "HTTP":
                    return mbProxyType.MB_PROXY_HTTP;
                case "SOCKET5":
                    return mbProxyType.MB_PROXY_SOCKS5;
                case "SOCKS4":
                    return mbProxyType.MB_PROXY_SOCKS4;
                case "SOCKS4A":
                    return mbProxyType.MB_PROXY_SOCKS4A;
                case "SOCKS5HOSTNAME":
                    return mbProxyType.MB_PROXY_SOCKS5HOSTNAME;
                default:
                    return mbProxyType.MB_PROXY_NONE;
            }
        }

        #region 全局访问点
        /// <summary>
        /// 定义公有方法提供一个全局访问点,同时你也可以定义公有属性来提供全局访问点
        /// </summary>
        /// <returns></returns>
        public static BlinkInit Instance
        {
            get
            { // 当第一个线程运行到这里时，此时会对locker对象 "加锁"，
              // 当第二个线程运行该方法时，首先检测到locker对象为"加锁"状态，该线程就会挂起等待第一个线程解锁
              // lock语句运行完之后（即线程运行完之后）会对该对象"解锁"
              // 双重锁定只需要一句判断就可以了
                if (uniqueInstance == null)
                {
                    lock (locker)
                    {
                        // 如果类的实例不存在则创建，否则直接返回
                        if (uniqueInstance == null)
                        {
                            uniqueInstance = new BlinkInit();
                            ////加载缓存
                            //var files = Directory.GetFiles(BlinkCommon.CachePath);
                            //if (files != null && files.Count() != 0)
                            //{

                            //}

                            //ThreadPool.QueueUserWorkItem(new WaitCallback((a) =>
                            //{
                            //    try
                            //    {
                            //        var files = Directory.GetFiles(BlinkCommon.CachePath);
                            //        if (files != null && files.Count() != 0)
                            //        {
                            //            foreach (var item in files)
                            //            {
                            //                if (File.Exists(item))
                            //                {
                            //                    FileInfo fi = new FileInfo(item);
                            //                    BlinkCommon.SetBlinkResCache(fi.Name, File.ReadAllBytes(fi.FullName));
                            //                }
                            //            }
                            //        }
                            //    }
                            //    catch (Exception)
                            //    {
                            //    }
                            //}), null);
                        }
                    }
                }

                return uniqueInstance;
            }


        }
        #endregion

        public void Stop()
        {
            _isStop = true;
        }

        ~BlinkInit()
        {
            _isStop = true;
        }

        /// <summary>
        /// 为了防止委托的页面关闭时内存被回收
        /// </summary>
        private static List<ActionModel> list_Action;

        /// <summary>
        /// 将委托添加到静态变量中，防止内存被回收
        /// </summary>
        /// <param name="action"></param>
        public void AddAction(ActionModel action)
        {
            lock (locker)
            {
                list_Action.Add(action);
            }
        }

        /// <summary>
        /// 将委托异常，防止内存无限增大
        /// </summary>
        /// <param name="action"></param>
        public void RemoveAction(ActionModel action)
        {
            lock (locker)
            {
                if (action == null)
                {
                    return;
                }
                //删除保存的事件
                var list = from a in list_Action where a.ID == action.ID select a;
                if (list != null)
                {
                    list.ToList().ForEach(s =>
                    {
                        list_Action.Remove(s);
                    });
                }
                GC.Collect();
            }
        }
    }
}