using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using System.Transactions;
using Gat.Utils;

namespace Gat.DAL
{
    /// <summary>
    /// 事务扩展，SubmitChanges，ExcuteCommand、ExecuteQuery
    /// </summary>
    public class TransactionExtension
    {
        /// <summary>
        /// 执行SQL操作
        /// </summary>
        /// <param name="funcs"></param>
        public static HandleArgs Excute(params Func<HandleArgs>[] funcs)
        {
            //使用ReadCommitted隔离级别，保持与Sql Server的默认隔离级别一致
            return Excute(IsolationLevel.ReadCommitted, null, funcs);
        }


        /// <summary>
        /// 执行SQL操作
        /// </summary>
        /// <param name="actions"></param>
        public static HandleArgs Excute(IsolationLevel level, params Func<HandleArgs>[] funcs)
        {
            return Excute(level, null, funcs);

        }


        /// <summary>
        /// 执行SQL操作
        /// </summary>
        /// <param name="funcs"></param>
        public static HandleArgs Excute(int timeOut, params Func<HandleArgs>[] funcs)
        {
            return Excute(IsolationLevel.ReadCommitted, timeOut, funcs);

        }


        /// <summary>
        /// 执行SQL操作
        /// </summary>
        /// <param name="funcs"></param>
        public static HandleArgs Excute(IsolationLevel level, int? timeOut, params Func<HandleArgs>[] funcs)
        {
            HandleArgs HANDLE = new HandleArgs();
            if (funcs == null || funcs.Length == 0) return HANDLE;

            TransactionOptions options = new TransactionOptions();
            options.IsolationLevel = level; //默认为Serializable,这里根据参数来进行调整

            if (timeOut.HasValue)
                options.Timeout = new TimeSpan(0, 0, timeOut.Value); //默认60秒

            using (TransactionScope tran = new TransactionScope(TransactionScopeOption.Required, options))
            {
                try
                {
                    foreach (Func<HandleArgs> func in funcs)
                    {
                        HANDLE = func();
                        if (HANDLE.Status == HandleStatus.Fail)
                        {
                            throw new Exception(HANDLE.Message);
                        }
                    }

                    tran.Complete(); //通知事务管理器它可以提交事务
                    return HANDLE;
                }
                catch(Exception ex)
                {
                    HANDLE.Message = ex.Message;
                    HANDLE.Exception = ex;
                    return HANDLE;
                }
            }

        }
    }
}
