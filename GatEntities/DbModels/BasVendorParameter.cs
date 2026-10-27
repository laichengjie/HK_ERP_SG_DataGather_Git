using System;
using System.Collections.Generic;
using System.Linq;
using SqlSugar;
namespace GatEntities.DbModels
{
    /// <summary>
    /// 供应商参数（按 ReportPath 区分报表，供应商浏览时按 ReportPath 加载）
    ///</summary>
    [SugarTable("Bas_VendorParameter")]
    public class BasVendorParameter
    {
        /// <summary>
        ///  
        ///</summary>
         [SugarColumn(ColumnName="ID" ,IsPrimaryKey = true ,IsIdentity = true  )]
         public int Id { get; set; }
        /// <summary>
        ///  参数代码
        ///</summary>
         [SugarColumn(ColumnName="ParameterCode"    )]
         public string ParameterCode { get; set; }
        /// <summary>
        ///  参数名称
        ///</summary>
         [SugarColumn(ColumnName="ParameterName"    )]
         public string ParameterName { get; set; }
        /// <summary>
        ///  关联报表
        ///</summary>
         [SugarColumn(ColumnName="ReportPath"    )]
         public byte? ReportPath { get; set; }
    }
}
