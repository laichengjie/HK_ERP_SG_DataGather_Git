using System;
using System.Collections.Generic;
using System.Linq;
using SqlSugar;
namespace GatEntities.DbModels
{
    /// <summary>
    /// 供应商参数值（一个供应商 + 一个参数代码 一条记录）
    ///</summary>
    [SugarTable("Bas_VendParameterValue")]
    public class BasVendParameterValue
    {
        /// <summary>
        ///  
        ///</summary>
         [SugarColumn(ColumnName="ID" ,IsPrimaryKey = true ,IsIdentity = true  )]
         public int Id { get; set; }
        /// <summary>
        ///  供应商ID
        ///</summary>
         [SugarColumn(ColumnName="VendorID"    )]
         public int VendorID { get; set; }
        /// <summary>
        ///  参数代码
        ///</summary>
         [SugarColumn(ColumnName="ParameterCode"    )]
         public string ParameterCode { get; set; }
        /// <summary>
        ///  参数值
        ///</summary>
         [SugarColumn(ColumnName="ParameterValue"    )]
         public string ParameterValue { get; set; }
        /// <summary>
        ///  修改时间
        ///</summary>
         [SugarColumn(ColumnName="ModifyDate"    )]
         public DateTime ModifyDate { get; set; }
    }
}
