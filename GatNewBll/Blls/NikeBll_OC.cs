using System;
using System.Collections.Generic;
using System.Data.SqlTypes;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Gat.Utils;
using GatEntities.DbModels;
using GatNewBll.Models.LiningMember;
using GatSqlSugarDAL.Services;
using SqlSugar;

namespace GatNewBll.Blls 
{
    public class NikeBll_OC : BaseBll 
    {
        public List<GatNikePosSale_OC> SelectPosSalePageList_OC(int currentPage, int itemsPerPage, int solutionID, ref int totalCount, ref decimal totalQty, ref decimal totalAmount, string shopCode = "", string startBillTime = "", string endBillTime = "", string startBillDate = "", string endBillDate = "", string materialCode = "", string sizeCode = "")
        {
            GatNikePossaleManager_OC manager = new GatNikePossaleManager_OC();
            var p = new PageModel() { PageIndex = currentPage, PageSize = itemsPerPage };// 分页查询
            List<IConditionalModel> conModels = new List<IConditionalModel>();
            conModels.Add(new ConditionalModel() { FieldName = "SolutionID", ConditionalType = ConditionalType.Equal, FieldValue = solutionID.ToString() });
            if (!string.IsNullOrEmpty(startBillTime))
            {
                conModels.Add(new ConditionalModel() { FieldName = "BillTime", ConditionalType = ConditionalType.GreaterThanOrEqual, FieldValue = startBillTime });
            }
            if (!string.IsNullOrEmpty(endBillTime))
            {
                conModels.Add(new ConditionalModel() { FieldName = "BillTime", ConditionalType = ConditionalType.LessThanOrEqual, FieldValue = endBillTime });
            }
            if (!string.IsNullOrEmpty(startBillDate))
            {
                conModels.Add(new ConditionalModel() { FieldName = "BillDate", ConditionalType = ConditionalType.GreaterThanOrEqual, FieldValue = startBillDate });
            }
            if (!string.IsNullOrEmpty(endBillDate))
            {
                conModels.Add(new ConditionalModel() { FieldName = "BillDate", ConditionalType = ConditionalType.LessThanOrEqual, FieldValue = endBillDate });
            }
            if (!string.IsNullOrEmpty(materialCode))
            {
                conModels.Add(new ConditionalModel() { FieldName = "MaterialCode", ConditionalType = ConditionalType.Like, FieldValue = materialCode });
            }
            if (!string.IsNullOrEmpty(sizeCode))
            {
                conModels.Add(new ConditionalModel() { FieldName = "SizeName", ConditionalType = ConditionalType.Like, FieldValue = sizeCode });
            }
            if (!string.IsNullOrEmpty(shopCode))
            {
                conModels.Add(new ConditionalModel() { FieldName = "ShopCode", ConditionalType = ConditionalType.Like, FieldValue = shopCode });
            }
            var result = manager.GetPageList(conModels, p).OrderBy(s => s.BillTime).ThenBy(s => s.BillNo);
            totalCount = p.TotalCount;
            totalQty = manager.AsQueryable().Where(conModels).Sum(s => s.Qty);
            totalAmount = manager.AsQueryable().Where(conModels).Sum(s => s.SaleAmount) ?? 0m;
            return result.ToList();
        }


        public List<GatNikeMove_OC> SelectMovePageList_OC(int currentPage, int itemsPerPage, int solutionID, ref int totalCount, ref decimal totalOutQty, ref decimal totalInQty, string outStockCode = "", string inStockCode = "", string startBillTime = "", string endBillTime = "", string startBillDate = "", string endBillDate = "", string materialCode = "", string sizeCode = "")
        {
            GatNikeMoveManager_OC manager = new GatNikeMoveManager_OC();
            var p = new PageModel() { PageIndex = currentPage, PageSize = itemsPerPage };// 分页查询
            List<IConditionalModel> conModels = new List<IConditionalModel>();
            conModels.Add(new ConditionalModel() { FieldName = "SolutionID", ConditionalType = ConditionalType.Equal, FieldValue = solutionID.ToString() });
            //if (!string.IsNullOrEmpty(startBillTime))
            //{
            //    conModels.Add(new ConditionalModel() { FieldName = "BillTime", ConditionalType = ConditionalType.GreaterThanOrEqual, FieldValue = startBillTime });
            //}
            //if (!string.IsNullOrEmpty(endBillTime))
            //{
            //    conModels.Add(new ConditionalModel() { FieldName = "BillTime", ConditionalType = ConditionalType.LessThanOrEqual, FieldValue = endBillTime });
            //}
            if (!string.IsNullOrEmpty(startBillDate))
            {
                conModels.Add(new ConditionalModel() { FieldName = "BillDate", ConditionalType = ConditionalType.GreaterThanOrEqual, FieldValue = startBillDate });
            }
            if (!string.IsNullOrEmpty(endBillDate))
            {
                conModels.Add(new ConditionalModel() { FieldName = "BillDate", ConditionalType = ConditionalType.LessThanOrEqual, FieldValue = endBillDate });
            }
            if (!string.IsNullOrEmpty(materialCode))
            {
                conModels.Add(new ConditionalModel() { FieldName = "MaterialCode", ConditionalType = ConditionalType.Like, FieldValue = materialCode });
            }
            if (!string.IsNullOrEmpty(sizeCode))
            {
                conModels.Add(new ConditionalModel() { FieldName = "SizeCode", ConditionalType = ConditionalType.Like, FieldValue = sizeCode });
            }
            if (!string.IsNullOrEmpty(outStockCode))
            {
                conModels.Add(new ConditionalModel() { FieldName = "OutStockCode", ConditionalType = ConditionalType.Like, FieldValue = outStockCode });
            }
            if (!string.IsNullOrEmpty(inStockCode))
            {
                conModels.Add(new ConditionalModel() { FieldName = "InStockCode", ConditionalType = ConditionalType.Like, FieldValue = inStockCode });
            }
            var result = manager.GetPageList(conModels, p).OrderBy(s => s.BillDate).ThenBy(s => s.BillNo);
            totalCount = p.TotalCount;
            totalOutQty = manager.AsQueryable().Where(conModels).Where(s=>s.BillTypeID == "Inv_MovOut").Sum(s => s.Qty);
            totalInQty = manager.AsQueryable().Where(conModels).Where(s=>s.BillTypeID == "Inv_MovIn").Sum(s => s.Qty);
            return result.ToList();
        }


        public List<GatNikeOutIn_OC> SelectOutInPageList_OC(int currentPage, int itemsPerPage, int solutionID, ref int totalCount, ref decimal totalQty, ref decimal totalAmount, string outStockCode = "", string inStockCode = "", string startBillTime = "", string endBillTime = "", string startBillDate = "", string endBillDate = "", string materialCode = "", string sizeCode = "")
        {
            GatNikeOutInManager_OC manager = new GatNikeOutInManager_OC(); 
            var p = new PageModel() { PageIndex = currentPage, PageSize = itemsPerPage };// 分页查询
            List<IConditionalModel> conModels = new List<IConditionalModel>();
            conModels.Add(new ConditionalModel() { FieldName = "SolutionID", ConditionalType = ConditionalType.Equal, FieldValue = solutionID.ToString() });
            if (!string.IsNullOrEmpty(startBillTime))
            {
                conModels.Add(new ConditionalModel() { FieldName = "ModifyTime", ConditionalType = ConditionalType.GreaterThanOrEqual, FieldValue = startBillTime });
            }
            if (!string.IsNullOrEmpty(endBillTime))
            {
                conModels.Add(new ConditionalModel() { FieldName = "ModifyTime", ConditionalType = ConditionalType.LessThanOrEqual, FieldValue = endBillTime });
            }
            if (!string.IsNullOrEmpty(startBillDate))
            {
                conModels.Add(new ConditionalModel() { FieldName = "BillDate", ConditionalType = ConditionalType.GreaterThanOrEqual, FieldValue = startBillDate });
            }
            if (!string.IsNullOrEmpty(endBillDate))
            {
                conModels.Add(new ConditionalModel() { FieldName = "BillDate", ConditionalType = ConditionalType.LessThanOrEqual, FieldValue = endBillDate });
            }
            if (!string.IsNullOrEmpty(materialCode))
            {
                conModels.Add(new ConditionalModel() { FieldName = "MaterialCode", ConditionalType = ConditionalType.Like, FieldValue = materialCode });
            }
            if (!string.IsNullOrEmpty(sizeCode))
            {
                conModels.Add(new ConditionalModel() { FieldName = "SizeCode", ConditionalType = ConditionalType.Like, FieldValue = sizeCode });
            }
            if (!string.IsNullOrEmpty(outStockCode))
            {
                conModels.Add(new ConditionalModel() { FieldName = "OutStockCode", ConditionalType = ConditionalType.Like, FieldValue = outStockCode });
            }
            if (!string.IsNullOrEmpty(inStockCode))
            {
                conModels.Add(new ConditionalModel() { FieldName = "InStockCode", ConditionalType = ConditionalType.Like, FieldValue = inStockCode });
            }
            var result = manager.GetPageList(conModels, p).OrderBy(s => s.BillDate).ThenBy(s => s.BillNo);
            totalCount = p.TotalCount;
            totalQty = manager.AsQueryable().Where(conModels).Sum(s => s.Qty);
            totalAmount = manager.AsQueryable().Where(conModels).Sum(s => s.Amount) ?? 0m;
            return result.ToList();
        }


        public Dictionary<string, object> SelectPosSaleTransferPageList_OC(int page, int pageSize, int solutionID, string startBillDate = "", string endBillDate = "", string shopCode = "", string materialCode = "", string sizeCode = "")
        {
            Dictionary<string, object> result = new Dictionary<string, object>();
            GatNikePosSaleTransferManager_OC manager = new GatNikePosSaleTransferManager_OC();
            var p = new PageModel() { PageIndex = page, PageSize = pageSize };// 分页查询
            List<IConditionalModel> conModels = new List<IConditionalModel>();
            conModels.Add(new ConditionalModel() { FieldName = "SolutionID", ConditionalType = ConditionalType.Equal, FieldValue = solutionID.ToString() });
            if (!string.IsNullOrEmpty(startBillDate))
            {
                conModels.Add(new ConditionalModel() { FieldName = "BillDate", ConditionalType = ConditionalType.GreaterThanOrEqual, FieldValue = startBillDate });
            }
            if (!string.IsNullOrEmpty(endBillDate))
            {
                conModels.Add(new ConditionalModel() { FieldName = "BillDate", ConditionalType = ConditionalType.LessThanOrEqual, FieldValue = endBillDate });
            }
            if (!string.IsNullOrEmpty(materialCode))
            {
                conModels.Add(new ConditionalModel() { FieldName = "MaterialCode", ConditionalType = ConditionalType.Like, FieldValue = materialCode });
            }
            if (!string.IsNullOrEmpty(sizeCode))
            {
                conModels.Add(new ConditionalModel() { FieldName = "SizeName", ConditionalType = ConditionalType.Like, FieldValue = sizeCode });
            }
            if (!string.IsNullOrEmpty(shopCode))
            {
                conModels.Add(new ConditionalModel() { FieldName = "ShopCode", ConditionalType = ConditionalType.Like, FieldValue = shopCode });
            }
            var list = manager.GetPageList(conModels, p).OrderBy(s => s.BillTime).ThenBy(s => s.BillNo);
            var totalCount = p.TotalCount;
            var qty = manager.AsQueryable().Where(conModels).Sum(s => s.Qty);
            var amount = manager.AsQueryable().Where(conModels).Sum(s => s.SaleAmount);
            result.Add("data", list);
            result.Add("totalCount", totalCount);
            result.Add("qty", qty);
            result.Add("amount", amount ?? 0);
            return result;
        }

        public Dictionary<string, object> SelectMoveTransferPageList_OC(int page, int pageSize, int solutionID, string startBillDate = "", string endBillDate = "", string outStockCode = "", string inStockCode = "", string materialCode = "", string sizeCode = "")
        {
            Dictionary<string, object> result = new Dictionary<string, object>();
            GatNikeMoveTransferManager_OC manager = new GatNikeMoveTransferManager_OC();
            var p = new PageModel() { PageIndex = page, PageSize = pageSize };// 分页查询
            List<IConditionalModel> conModels = new List<IConditionalModel>();
            conModels.Add(new ConditionalModel() { FieldName = "SolutionID", ConditionalType = ConditionalType.Equal, FieldValue = solutionID.ToString() });
            if (!string.IsNullOrEmpty(startBillDate))
            {
                conModels.Add(new ConditionalModel() { FieldName = "BillDate", ConditionalType = ConditionalType.GreaterThanOrEqual, FieldValue = startBillDate });
            }
            if (!string.IsNullOrEmpty(endBillDate))
            {
                conModels.Add(new ConditionalModel() { FieldName = "BillDate", ConditionalType = ConditionalType.LessThanOrEqual, FieldValue = endBillDate });
            }
            if (!string.IsNullOrEmpty(materialCode))
            {
                conModels.Add(new ConditionalModel() { FieldName = "MaterialCode", ConditionalType = ConditionalType.Like, FieldValue = materialCode });
            }
            if (!string.IsNullOrEmpty(sizeCode))
            {
                conModels.Add(new ConditionalModel() { FieldName = "SizeCode", ConditionalType = ConditionalType.Like, FieldValue = sizeCode });
            }
            if (!string.IsNullOrEmpty(outStockCode))
            {
                conModels.Add(new ConditionalModel() { FieldName = "OutStockCode", ConditionalType = ConditionalType.Like, FieldValue = outStockCode });
            }
            if (!string.IsNullOrEmpty(inStockCode))
            {
                conModels.Add(new ConditionalModel() { FieldName = "InStockCode", ConditionalType = ConditionalType.Like, FieldValue = inStockCode });
            }
            var list = manager.GetPageList(conModels, p).OrderBy(s => s.BillDate).ThenBy(s => s.BillNo);
            var totalCount = p.TotalCount;

            var totalOutQty = manager.AsQueryable().Where(conModels).Where(s => s.BillTypeID == "Inv_MovOut").Sum(s => s.Qty);
            var totalInQty = manager.AsQueryable().Where(conModels).Where(s => s.BillTypeID == "Inv_MovIn").Sum(s => s.Qty);

            result.Add("data", list);
            result.Add("totalCount", totalCount);
            result.Add("totalOutQty", totalOutQty ?? 0);
            result.Add("totalInQty", totalInQty ?? 0);
            return result;
        }

     

  
   
        /// <summary>
        /// 从ERP提取库获取提报数据
        /// </summary>
        /// <param name="VendorCode">供应商代码</param>
        /// <param name="GatType">采集类型，1：正常采集；0：补采</param> 
        /// <param name="CompanyID">公司ID</param>
        /// <param name="CardID">品牌ID</param>
        /// <param name="StartTime">补采时开始时间</param>
        /// <param name="EndTime">补采结束时间</param>
        /// <param name="IsTB">是否三小时提报</param>
        /// <returns></returns>
        /// <exception cref="Exception"></exception>
        public HandleArgs GetExtractorData(string ProcedureName, string VendorCode, int GatType, string CompanyID, string CardID, string StartTime, string EndTime, int IsTB = 0)
        {
            HandleArgs result = new HandleArgs();
            try
            {
                if (ProcedureName == "sp_SGIS_ExecBatch")
                {
                    var data = ExtractorDB.Ado.UseStoredProcedure()
                    .SqlQuery<string>(ProcedureName,
                    new { VendorCode = VendorCode, GatType = GatType, CompanyID = CompanyID, CardID = CardID, StartTime = StartTime, EndTime = EndTime });
                }
                else
                {
                    var data = ExtractorDB.Ado.UseStoredProcedure()
                        .SqlQuery<string>(ProcedureName,
                        new { VendorCode = VendorCode, GatType = GatType, CompanyID = CompanyID, CardID = CardID, StartTime = StartTime, EndTime = EndTime, IsTB = IsTB });

                }
                result.Status = HandleStatus.Completed;
                result.Message = "提报成功";
            }
            catch (Exception ex)
            {
                result.Status = HandleStatus.Fail;
                result.Exception = ex;
                result.Message = ex.Message;
                return result;
            }
            return result;


        }
    }
}
