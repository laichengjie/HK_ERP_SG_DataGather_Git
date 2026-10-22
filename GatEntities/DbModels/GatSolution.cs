using System;
using System.Collections.Generic;
using System.Linq;
using SqlSugar;
namespace GatEntities.DbModels
{
    /// <summary>
    /// 
    ///</summary>
    [SugarTable("Gat_Solution")]
    public class GatSolution
    {
        /// <summary>
        ///  
        ///</summary>
         [SugarColumn(ColumnName="ID" ,IsPrimaryKey = true ,IsIdentity = true  )]
         public int Id { get; set; }
        /// <summary>
        ///  
        ///</summary>
         [SugarColumn(ColumnName="Title"    )]
         public string Title { get; set; }
        /// <summary>
        ///  
        ///</summary>
         [SugarColumn(ColumnName="PastVersionVendorID"    )]
         public string PastVersionVendorID { get; set; }
        /// <summary>
        ///  
        ///</summary>
         [SugarColumn(ColumnName="CardID"    )]
         public string CardID { get; set; }
        /// <summary>
        ///  
        /// 默认值: ((0))
        ///</summary>
         [SugarColumn(ColumnName="VendorID"    )]
         public int? VendorID { get; set; }
        /// <summary>
        ///  
        ///</summary>
         [SugarColumn(ColumnName="CompanyID"    )]
         public string CompanyID { get; set; }
        /// <summary>
        ///  
        ///</summary>
         [SugarColumn(ColumnName="GatType"    )]
         public byte GatType { get; set; }
        /// <summary>
        ///  
        ///</summary>
         [SugarColumn(ColumnName="Directory"    )]
         public string Directory { get; set; }
        /// <summary>
        ///  
        ///</summary>
         [SugarColumn(ColumnName="ExecType"    )]
         public byte ExecType { get; set; }
        /// <summary>
        ///  
        /// 默认值: ((1))
        ///</summary>
         [SugarColumn(ColumnName="ExecFrequency"    )]
         public int ExecFrequency { get; set; }
        /// <summary>
        ///  
        ///</summary>
         [SugarColumn(ColumnName="DailyType"    )]
         public byte DailyType { get; set; }
        /// <summary>
        ///  
        ///</summary>
         [SugarColumn(ColumnName="OnceStartTime"    )]
         public DateTime? OnceStartTime { get; set; }
        /// <summary>
        ///  
        ///</summary>
         [SugarColumn(ColumnName="RepeatType"    )]
         public byte? RepeatType { get; set; }
        /// <summary>
        ///  
        ///</summary>
         [SugarColumn(ColumnName="RepeatInterval"    )]
         public int? RepeatInterval { get; set; }
        /// <summary>
        ///  
        ///</summary>
         [SugarColumn(ColumnName="RepeatStartTime"    )]
         public DateTime? RepeatStartTime { get; set; }
        /// <summary>
        ///  
        ///</summary>
         [SugarColumn(ColumnName="ManualGatStartTime"    )]
         public DateTime ManualGatStartTime { get; set; }
        /// <summary>
        ///  
        ///</summary>
         [SugarColumn(ColumnName="ManualGatEndTime"    )]
         public DateTime ManualGatEndTime { get; set; }
        /// <summary>
        ///  
        ///</summary>
         [SugarColumn(ColumnName="ManualInterval"    )]
         public int ManualInterval { get; set; }
        /// <summary>
        ///  
        /// 默认值: ((1))
        ///</summary>
         [SugarColumn(ColumnName="Enabled"    )]
         public bool Enabled { get; set; }
        /// <summary>
        ///  
        ///</summary>
         [SugarColumn(ColumnName="Remark"    )]
         public string Remark { get; set; }
        /// <summary>
        ///  
        /// 默认值: ((0))
        ///</summary>
         [SugarColumn(ColumnName="SpecifyStartTime"    )]
         public bool? SpecifyStartTime { get; set; }
        /// <summary>
        ///  
        ///</summary>
         [SugarColumn(ColumnName="ProcName"    )]
         public string ProcName { get; set; }
        /// <summary>
        ///  是否自动提报
        /// 默认值: ((1))
        ///</summary>
         [SugarColumn(ColumnName="IsAuto"    )]
         public bool? IsAuto { get; set; }
        /// <summary>
        ///  是否后台任务提报
        /// 默认值: ((1))
        ///</summary>
        [SugarColumn(ColumnName = "IsBackground")]
        public bool? IsBackground { get; set; }
        /// <summary>
        ///  Cron表达式
        ///</summary>
        [SugarColumn(ColumnName="CronExpression"    )]
         public string CronExpression { get; set; }

        /// <summary>
        ///  appKey过期时间
        ///</summary>
        [SugarColumn(ColumnName = "ExpiryDate")]
        public string ExpiryDate { get; set; }

        /// <summary>
        ///  appKey
        ///</summary>
        [SugarColumn(ColumnName = "AppKey")]
        public string AppKey { get; set; }

        /// <summary>
        /// 销售金额取数 0 结算金额  1 ActualAmount实际结算金额
        ///</summary>
        [SugarColumn(ColumnName = "SalesAmountType")]
        public int SalesAmountType { get; set; }
    }
}
