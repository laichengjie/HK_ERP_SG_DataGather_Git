using Gat.BLL.Modules;
using Gat.DAL;
using Gat.Utils;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using System.Text;


namespace Gat.BLL
{
    public partial class GatRepository : IDisposable
    {


        #region 

        /// <summary>
        /// 查询库存中转数据
        /// </summary>
        /// <param name="page"></param>
        /// <param name="pageSize"></param>
        /// <param name="solutionID"></param> 
        /// <param name="startBillDate"></param> 
        /// <param name="endBillDate"></param>
        /// <param name="shopCode"></param> 
        /// <param name="materialCode"></param>
        /// <param name="sizeCode"></param>
        /// <returns></returns>
        public Dictionary<string, object> SelectMaterialBalaPricePageList(int page, int pageSize, string companyid = "",string materialCode = "")
        {
            Dictionary<string, object> result = new Dictionary<string, object>();
            List<GatMaterialBalaPriceB> posSalelist = new List<GatMaterialBalaPriceB>();
            var query = _Service.QueryEntity(ExpHelper.True<GatMaterialBalaPrice>());
            if (!string.IsNullOrEmpty(companyid)) query = query.Where(g => g.CompanyID==companyid);
            if (!string.IsNullOrWhiteSpace(materialCode))
            {
                query = query.Where(e => e.MaterialCode.Contains(materialCode));
            }
          
            var list = query
                .OrderBy(p => p.ModifyDtm)
                .ThenBy(p => p.MaterialCode);

            var totalCount = list.Count();
            posSalelist = list
                        .Select(g => new GatMaterialBalaPriceB(g))
                        .Skip((page - 1) * pageSize)
                        .Take(pageSize)
                        .ToList();

            var total = list.ToList();
            result.Add("data", posSalelist);
            result.Add("totalCount", totalCount);
            return result;
        }


        /// <summary>
        /// 
        /// </summary>
        /// <param name="CompanyID"></param>
        /// <param name="MaterialCode"></param>
        /// <param name="BalaPrice"></param>
        /// <param name="TransIDstr">id</param>
        /// <param name="Type"> 0 导入 1删除</param>
        /// <returns></returns>
        public HandleArgs MaterialBalaPriceSaveAll(string CompanyID, string MaterialCode, decimal BalaPrice, string TransIDstr,int Type)
        {
            HandleArgs handle = new HandleArgs();
            string command = $" EXEC sp_SGIS_MaterialBalaPrice  @CompanyID='{CompanyID}',@MaterialCode='{MaterialCode}',@BalaPrice={BalaPrice},@TransIDstr= '{TransIDstr}',@Type='{Type}' ";
            command = string.Format(command);
            HandleArgs HANDLE = _Service.ExecuteCommand(command);
            return handle;
        }
        #endregion
    }
}
