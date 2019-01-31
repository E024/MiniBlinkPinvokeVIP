using System;
using System.Collections.Generic;
using System.Text;

namespace MiniBlinkPinvokeVIP.Model
{
    /// <summary>
    /// 文件下载实体
    /// </summary>
    public class DownloadModel
    {
        public string Name { get; set; }
        public string Path { get; set; }
        public string URL { get; set; }
        /// <summary>
        /// 任务ID，删除时要用
        /// </summary>
        public Guid ID { get; set; } = Guid.NewGuid();
        /// <summary>
        /// 完成后打开文件，或打开文件夹
        /// </summary>
        public bool FilshOpenFile { get; set; } = false;
    }
}
