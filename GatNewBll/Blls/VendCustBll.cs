using GatEntities.DbModels;
using GatSqlSugarDAL.Services;
using GatNewBll.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GatNewBll.Blls
{
    public class VendCustBll : BaseBll
    {
        /// <summary>
        /// 获取提报方案下的客户
        /// </summary>
        /// <param name="solutionId"></param>
        /// <returns></returns>
        public List<GatVendcustright> GetVendCust(int solutionId)
        {
            GatVendcustrightManager manager = new GatVendcustrightManager();
            return manager.GetList(s => s.SolutionID == solutionId);
        }

        /// <summary>
        /// 同步erp客户到提报方案
        /// </summary>
        /// <param name="solutionId"></param>
        public void AyncErpCust(int solutionId)
        {
            try
            {

                //获取提报方案档案
                GatSolutionManager solutionManager = new GatSolutionManager();
                var solution = solutionManager.GetById(solutionId);
                if (solution == null)
                {
                    throw new Exception("未找到提报方案");
                }
                //通过公司账套Id获取客户
                var erpVendCustList = ErpDb.Ado.SqlQuery<ErpVendCust>($@"SELECT A.VendCustId,A.VendCustCode,A.VendCustName FROM  dbo.Bas_InterCompany AS A
         LEFT OUTER JOIN dbo.Bas_InterCompanyParam AS B
           ON A.VendCustID = B.VendCustID
           AND A.Lan = B.Lan
           WHERE B.IsCustomer = 1 AND A.CompanyID = '{solution.CompanyID}'");

                if (erpVendCustList == null && erpVendCustList.Count > 0)
                {
                    throw new Exception($"未找到账套{solution.CompanyID}客户数据");
                }
                //如果对应提报方案没有数据，全量新增
                GatVendcustrightManager vcrManager = new GatVendcustrightManager();
                var vcrData = vcrManager.GetList(s => s.SolutionID == solutionId);
                if (vcrData == null || vcrData?.Count == 0)
                {
                    List<GatVendcustright> insertDatas = new List<GatVendcustright>();
                    foreach (var item in erpVendCustList)
                    {
                        insertDatas.Add(new GatVendcustright()
                        {
                            SolutionID = solutionId,
                            CompanyID = solution.CompanyID,
                            VendCustCode = item.VendCustCode,
                            VendCustID = item.VendCustId,
                            VendCustName = item.VendCustName,
                            AllowUsed = false,
                            ModifyDTM = DateTime.Now
                        });
                    }
                    vcrManager.InsertRange(insertDatas);
                }
                //有客户数据则存在的更新，不存在的新增
                else
                {
                    List<GatVendcustright> updateDatas = new List<GatVendcustright>();
                    List<GatVendcustright> insertDatas = new List<GatVendcustright>();
                    foreach (var item in erpVendCustList)
                    {
                        var existVndCustData = vcrData.FirstOrDefault(s => s.VendCustID == item.VendCustId);
                        if (existVndCustData != null)
                        {
                            existVndCustData.VendCustCode = item.VendCustCode;
                            existVndCustData.VendCustName = item.VendCustName;
                            updateDatas.Add(existVndCustData);
                        }
                        else
                        {
                            insertDatas.Add(new GatVendcustright()
                            {
                                SolutionID = solutionId,
                                CompanyID = solution.CompanyID,
                                VendCustCode = item.VendCustCode,
                                VendCustID = item.VendCustId,
                                VendCustName = item.VendCustName,
                                AllowUsed = false,
                                ModifyDTM = DateTime.Now
                            });
                        }
                    }
                    vcrManager.UpdateRange(updateDatas);
                    vcrManager.InsertRange(insertDatas);
                }

            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        /// <summary>
        /// 更新店铺权限，只更新部分字段
        /// </summary>
        /// <param name="data"></param>
        /// <param name="columns">需要更新的字段名</param>
        /// <returns></returns>
        public bool Update(List<GatVendcustright> data, params string[] columns)
        {
            GatVendcustrightManager manager = new GatVendcustrightManager();
            return manager.BatchUpdateWithColumn(data, columns);
        }
    }
}
