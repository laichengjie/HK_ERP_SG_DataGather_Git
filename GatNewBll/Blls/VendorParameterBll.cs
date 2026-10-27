using System;
using System.Collections.Generic;
using System.Linq;
using Gat.Utils;
using GatEntities.DbModels;
using GatNewBll.Models;
using SqlSugar;

namespace GatNewBll.Blls
{
    /// <summary>
    /// 供应商参数维护（Bas_VendorParameter 参数表 + Bas_VendParameterValue 参数值表）
    /// </summary>
    public class VendorParameterBll : BaseBll
    {
        /// <summary>
        /// 按供应商的 ReportPath 加载参数列表，并带上该供应商已设置的参数值
        /// </summary>
        /// <param name="vendorId">供应商ID</param>
        /// <param name="reportPath">关联报表（Bas_Vendor.ReportPath）</param>
        /// <returns></returns>
        public List<VendorParameterModel> QueryVendorParameters(int vendorId, byte? reportPath)
        {
            string sql = @"
SELECT p.ParameterCode,
       p.ParameterName,
       v.ParameterValue,
       v.ModifyDate
FROM Bas_VendorParameter p WITH(NOLOCK)
LEFT JOIN Bas_VendParameterValue v WITH(NOLOCK)
       ON v.ParameterCode = p.ParameterCode AND v.VendorID = @VendorID
WHERE ISNULL(p.ReportPath, 0) = ISNULL(@ReportPath, 0)
ORDER BY p.ParameterCode";

            return Db.Ado.SqlQuery<VendorParameterModel>(sql,
                new { VendorID = vendorId, ReportPath = reportPath });
        }

        /// <summary>
        /// 设置/修改供应商的参数值（不存在则新增，已存在则更新）
        /// </summary>
        /// <param name="vendorId">供应商ID</param>
        /// <param name="parameterCode">参数代码</param>
        /// <param name="parameterValue">参数值</param>
        /// <returns></returns>
        public HandleArgs SaveVendorParameterValue(int vendorId, string parameterCode, string parameterValue)
        {
            HandleArgs result = new HandleArgs();
            try
            {
                if (vendorId == 0)
                {
                    result.Status = HandleStatus.Fail;
                    result.Message = "参数错误，消息：供应商ID不存在";
                    return result;
                }
                if (string.IsNullOrWhiteSpace(parameterCode))
                {
                    result.Status = HandleStatus.Fail;
                    result.Message = "参数错误，消息：参数代码不存在";
                    return result;
                }

                //参数值长度受限于表字段 varchar(250)
                if (!string.IsNullOrEmpty(parameterValue) && parameterValue.Length > 250)
                {
                    result.Status = HandleStatus.Fail;
                    result.Message = "参数值长度不能大于250！";
                    return result;
                }

                BasVendParameterValue entity = Db.Queryable<BasVendParameterValue>()
                    .First(o => o.VendorID == vendorId && o.ParameterCode == parameterCode);

                if (entity == null)
                {
                    //之前没有设置过、本次也没有填值，不必生成一条空记录
                    if (string.IsNullOrWhiteSpace(parameterValue))
                    {
                        result.Status = HandleStatus.Completed;
                        result.Message = "保存参数值成功！";
                        return result;
                    }

                    entity = new BasVendParameterValue()
                    {
                        VendorID = vendorId,
                        ParameterCode = parameterCode,
                        ParameterValue = parameterValue ?? "",
                        ModifyDate = DateTime.Now
                    };
                    Db.Insertable(entity).ExecuteCommand();
                }
                else
                {
                    Db.Updateable<BasVendParameterValue>()
                        .SetColumns(o => o.ParameterValue == parameterValue)
                        .SetColumns(o => o.ModifyDate == DateTime.Now)
                        .Where(o => o.Id == entity.Id)
                        .ExecuteCommand();
                }

                result.Status = HandleStatus.Completed;
                result.Message = "保存参数值成功！";
                return result;
            }
            catch (Exception ex)
            {
                result.Status = HandleStatus.Fail;
                result.Exception = ex;
                result.Message = "保存参数值异常：" + ex.Message;
                return result;
            }
        }
    }
}
