using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Gat.DAL
{
    /// <summary>
    /// 执行数据库操作返回的模型
    /// </summary>
    public partial class ReturnModule
    {
        //private System.Nullable<System.DateTime> _Column1;
        private string _Message;

        private string _Code;

        public ReturnModule()
        {
        }

        [global::System.Data.Linq.Mapping.ColumnAttribute(Name = "", Storage = "_Message", DbType = "Varchar(1000)")]
        public string Message
        {
            get
            {
                return this._Message;
            }
            set
            {
                if ((this._Message != value))
                {
                    this._Message = value;
                }
            }
        }

        [global::System.Data.Linq.Mapping.ColumnAttribute(Name = "", Storage = "_Code", DbType = "Varchar(3)")]
        public string Code
        {
            get
            {
                return this._Code;
            }
            set
            {
                if ((this._Code != value))
                {
                    this._Code = value;
                }
            }
        }
    }
}
