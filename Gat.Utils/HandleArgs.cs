using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Text;

namespace Gat.Utils
{
    /// <summary>
    /// 执行结果
    /// </summary>
    public partial class HandleArgs
    {
        #region 公开属性

        /// <summary>
        /// 执行状态
        /// </summary>
        public HandleStatus Status { get; set; }

        /// <summary>
        /// 捕捉到的的异常
        /// </summary>
        public Exception Exception { get; set; }

        /// <summary>
        /// 消息，该值也可能来自Exception的Message属性
        /// </summary>
        public string Message { get; set; }

        #endregion

        public HandleArgs()
        {
            this.Status = HandleStatus.Completed;
        }
    }

    /// <summary>
    /// 执行
    /// </summary>
    public enum HandleStatus
    {
        /// <summary>
        /// 处理中
        /// </summary>
        Processing,
        /// <summary>
        /// 失败
        /// </summary>
        Fail,
        /// <summary>
        /// 完成
        /// </summary>
        Completed
    }
}