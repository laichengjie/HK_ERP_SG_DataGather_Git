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
    public class AdBll_OC : BaseBll
    {
        public List<GatAdPossale_OC> SelectPosSalePageList_OC(int currentPage, int itemsPerPage, int solutionID, ref int totalCount, ref decimal totalQty, ref decimal totalAmount, string shopCode = "", string startBillTime = "", string endBillTime = "", string startBillDate = "", string endBillDate = "", string materialCode = "", string sizeCode = "")
        {
            GatAdPossaleManager_OC manager = new GatAdPossaleManager_OC();
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

        public List<GatAdPossale_OCTB> SelectPosSalePageList_OCTB(int currentPage, int itemsPerPage, int solutionID, ref int totalCount, ref decimal totalQty, ref decimal totalAmount, string shopCode = "", string startBillTime = "", string endBillTime = "", string startBillDate = "", string endBillDate = "", string materialCode = "", string sizeCode = "")
        {
            GatAdPossaleManager_OCTB manager = new GatAdPossaleManager_OCTB();
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

        public List<GatAdMove_OC> SelectMovePageList_OC(int currentPage, int itemsPerPage, int solutionID, ref int totalCount, ref decimal totalQty, ref decimal totalAmount, string outStockCode = "", string inStockCode = "", string startBillTime = "", string endBillTime = "", string startBillDate = "", string endBillDate = "", string materialCode = "", string sizeCode = "")
        {
            GatAdMoveManager_OC manager = new GatAdMoveManager_OC();
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
            if (!string.IsNullOrEmpty(outStockCode))
            {
                conModels.Add(new ConditionalModel() { FieldName = "OutStockCode", ConditionalType = ConditionalType.Like, FieldValue = outStockCode });
            }
            if (!string.IsNullOrEmpty(inStockCode))
            {
                conModels.Add(new ConditionalModel() { FieldName = "InStockCode", ConditionalType = ConditionalType.Like, FieldValue = inStockCode });
            }
            var result = manager.GetPageList(conModels, p).OrderBy(s => s.BillTime).ThenBy(s => s.BillNo);
            totalCount = p.TotalCount;
            totalQty = manager.AsQueryable().Where(conModels).Sum(s => s.Qty);
            //totalAmount = manager.AsQueryable().Where(conModels).Sum(s => s.SaleAmount) ?? 0m;
            totalAmount = 0m;
            return result.ToList();
        }

        public List<GatAdMove_OCTB> SelectMovePageList_OCTB(int currentPage, int itemsPerPage, int solutionID, ref int totalCount, ref decimal totalQty, ref decimal totalAmount, string outStockCode = "", string inStockCode = "", string startBillTime = "", string endBillTime = "", string startBillDate = "", string endBillDate = "", string materialCode = "", string sizeCode = "")
        {
            GatAdMoveManager_OCTB manager = new GatAdMoveManager_OCTB();
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
            if (!string.IsNullOrEmpty(outStockCode))
            {
                conModels.Add(new ConditionalModel() { FieldName = "OutStockCode", ConditionalType = ConditionalType.Like, FieldValue = outStockCode });
            }
            if (!string.IsNullOrEmpty(inStockCode))
            {
                conModels.Add(new ConditionalModel() { FieldName = "InStockCode", ConditionalType = ConditionalType.Like, FieldValue = inStockCode });
            }
            var result = manager.GetPageList(conModels, p).OrderBy(s => s.BillTime).ThenBy(s => s.BillNo);
            totalCount = p.TotalCount;
            totalQty = manager.AsQueryable().Where(conModels).Sum(s => s.Qty);
            //totalAmount = manager.AsQueryable().Where(conModels).Sum(s => s.SaleAmount) ?? 0m;
            totalAmount = 0m;
            return result.ToList();
        }

        public List<GatAdOutIn_OCTB> SelectOutInPageList_OCTB(int currentPage, int itemsPerPage, int solutionID, ref int totalCount, ref decimal totalQty, ref decimal totalAmount, string outStockCode = "", string inStockCode = "", string startBillTime = "", string endBillTime = "", string startBillDate = "", string endBillDate = "", string materialCode = "", string sizeCode = "")
        {
            GatAdOutInManager_OCTB manager = new GatAdOutInManager_OCTB();
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
            if (!string.IsNullOrEmpty(outStockCode))
            {
                conModels.Add(new ConditionalModel() { FieldName = "OutStockCode", ConditionalType = ConditionalType.Like, FieldValue = outStockCode });
            }
            if (!string.IsNullOrEmpty(inStockCode))
            {
                conModels.Add(new ConditionalModel() { FieldName = "InStockCode", ConditionalType = ConditionalType.Like, FieldValue = inStockCode });
            }
            var result = manager.GetPageList(conModels, p).OrderBy(s => s.BillTime).ThenBy(s => s.BillNo);
            totalCount = p.TotalCount;
            totalQty = manager.AsQueryable().Where(conModels).Sum(s => s.Qty);
            //totalAmount = manager.AsQueryable().Where(conModels).Sum(s => s.SaleAmount) ?? 0m;
            totalAmount = 0m;
            return result.ToList();
        }


        public Dictionary<string, object> SelectPosSaleTransferPageList_OC(int page, int pageSize, int solutionID, string startBillDate = "", string endBillDate = "", string shopCode = "", string materialCode = "", string sizeCode = "")
        {
            Dictionary<string, object> result = new Dictionary<string, object>();
            GatAdPossaleTransferManager_OC manager = new GatAdPossaleTransferManager_OC();
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
            result.Add("qty", qty ?? 0);
            result.Add("amount", amount ?? 0);
            return result;
        }

        public Dictionary<string, object> SelectMoveTransferPageList_OC(int page, int pageSize, int solutionID, string startBillDate = "", string endBillDate = "", string outStockCode = "", string inStockCode = "", string materialCode = "", string sizeCode = "")
        {
            Dictionary<string, object> result = new Dictionary<string, object>();
            GatAdMoveTransferManager_OC manager = new GatAdMoveTransferManager_OC();
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
            if (!string.IsNullOrEmpty(outStockCode))
            {
                conModels.Add(new ConditionalModel() { FieldName = "OutStockCode", ConditionalType = ConditionalType.Like, FieldValue = outStockCode });
            }
            if (!string.IsNullOrEmpty(inStockCode))
            {
                conModels.Add(new ConditionalModel() { FieldName = "InStockCode", ConditionalType = ConditionalType.Like, FieldValue = inStockCode });
            }
            var list = manager.GetPageList(conModels, p).OrderBy(s => s.BillTime).ThenBy(s => s.BillNo);
            var totalCount = p.TotalCount;
            var qty = manager.AsQueryable().Where(conModels).Sum(s => s.Qty);
            //var amount = manager.AsQueryable().Where(conModels).Sum(s => s.SalesAmount);

            result.Add("data", list);
            result.Add("totalCount", totalCount);
            result.Add("qty", qty ?? 0);
            result.Add("amount", 0);
            return result;
        }

        public List<GatAdPossale_Wif> SelectPosSalePageList_Wif(int currentPage, int itemsPerPage, int solutionID, ref int totalCount, ref decimal totalQty, ref decimal totalAmount, string shopCode = "", string startBillTime = "", string endBillTime = "", string startBillDate = "", string endBillDate = "", string materialCode = "", string sizeCode = "")
        {
            GatAdPossaleManager_Wif manager = new GatAdPossaleManager_Wif();
            var p = new PageModel() { PageIndex = currentPage, PageSize = itemsPerPage };// 分页查询
            List<IConditionalModel> conModels = new List<IConditionalModel>();
            conModels.Add(new ConditionalModel() { FieldName = "SolutionID", ConditionalType = ConditionalType.Equal, FieldValue = solutionID.ToString() });
            if (!string.IsNullOrEmpty(startBillTime))
            {
                conModels.Add(new ConditionalModel() { FieldName = "billtime", ConditionalType = ConditionalType.GreaterThanOrEqual, FieldValue = startBillTime });
            }
            if (!string.IsNullOrEmpty(endBillTime))
            {
                conModels.Add(new ConditionalModel() { FieldName = "billtime", ConditionalType = ConditionalType.LessThanOrEqual, FieldValue = endBillTime });
            }
            if (!string.IsNullOrEmpty(startBillDate))
            {
                conModels.Add(new ConditionalModel() { FieldName = "billdate", ConditionalType = ConditionalType.GreaterThanOrEqual, FieldValue = startBillDate });
            }
            if (!string.IsNullOrEmpty(endBillDate))
            {
                conModels.Add(new ConditionalModel() { FieldName = "billdate", ConditionalType = ConditionalType.LessThanOrEqual, FieldValue = endBillDate });
            }
            if (!string.IsNullOrEmpty(materialCode))
            {
                conModels.Add(new ConditionalModel() { FieldName = "product_code", ConditionalType = ConditionalType.Like, FieldValue = materialCode });
            }
            if (!string.IsNullOrEmpty(sizeCode))
            {
                conModels.Add(new ConditionalModel() { FieldName = "size_code", ConditionalType = ConditionalType.Like, FieldValue = sizeCode });
            }
            if (!string.IsNullOrEmpty(shopCode))
            {
                conModels.Add(new ConditionalModel() { FieldName = "store_code", ConditionalType = ConditionalType.Like, FieldValue = shopCode });
            }
            var result = manager.GetPageList(conModels, p).OrderBy(s => s.ModifyTime).ThenBy(s => s.BillNo);
            totalCount = p.TotalCount;
            totalQty = manager.AsQueryable().Where(conModels).Sum(s => s.qty);
            totalAmount = manager.AsQueryable().Where(conModels).Sum(s => s.tot_amt_actual) ?? 0m;
            return result.ToList();
        }

        public List<GatAdPossale_WifTB> SelectPosSalePageList_WifTB(int currentPage, int itemsPerPage, int solutionID, ref int totalCount, ref decimal totalQty, ref decimal totalAmount, string shopCode = "", string startBillTime = "", string endBillTime = "", string startBillDate = "", string endBillDate = "", string materialCode = "", string sizeCode = "")
        {
            GatAdPossaleManager_WifTB manager = new GatAdPossaleManager_WifTB();
            var p = new PageModel() { PageIndex = currentPage, PageSize = itemsPerPage };// 分页查询
            List<IConditionalModel> conModels = new List<IConditionalModel>();
            conModels.Add(new ConditionalModel() { FieldName = "SolutionID", ConditionalType = ConditionalType.Equal, FieldValue = solutionID.ToString() });
            if (!string.IsNullOrEmpty(startBillTime))
            {
                conModels.Add(new ConditionalModel() { FieldName = "billtime", ConditionalType = ConditionalType.GreaterThanOrEqual, FieldValue = startBillTime });
            }
            if (!string.IsNullOrEmpty(endBillTime))
            {
                conModels.Add(new ConditionalModel() { FieldName = "billtime", ConditionalType = ConditionalType.LessThanOrEqual, FieldValue = endBillTime });
            }
            if (!string.IsNullOrEmpty(startBillDate))
            {
                conModels.Add(new ConditionalModel() { FieldName = "billdate", ConditionalType = ConditionalType.GreaterThanOrEqual, FieldValue = startBillDate });
            }
            if (!string.IsNullOrEmpty(endBillDate))
            {
                conModels.Add(new ConditionalModel() { FieldName = "billdate", ConditionalType = ConditionalType.LessThanOrEqual, FieldValue = endBillDate });
            }
            if (!string.IsNullOrEmpty(materialCode))
            {
                conModels.Add(new ConditionalModel() { FieldName = "product_code", ConditionalType = ConditionalType.Like, FieldValue = materialCode });
            }
            if (!string.IsNullOrEmpty(sizeCode))
            {
                conModels.Add(new ConditionalModel() { FieldName = "size_code", ConditionalType = ConditionalType.Like, FieldValue = sizeCode });
            }
            if (!string.IsNullOrEmpty(shopCode))
            {
                conModels.Add(new ConditionalModel() { FieldName = "store_code", ConditionalType = ConditionalType.Like, FieldValue = shopCode });
            }
            var result = manager.GetPageList(conModels, p).OrderBy(s => s.ModifyTime).ThenBy(s => s.BillNo);
            totalCount = p.TotalCount;
            totalQty = manager.AsQueryable().Where(conModels).Sum(s => s.qty);
            totalAmount = manager.AsQueryable().Where(conModels).Sum(s => s.tot_amt_actual) ?? 0m;
            return result.ToList();
        }


        public Dictionary<string, object> SelectPosSaleTransferPageList_Wif(int page, int pageSize, int solutionID, string startBillDate = "", string endBillDate = "", string shopCode = "", string materialCode = "", string sizeCode = "")
        {
            Dictionary<string, object> result = new Dictionary<string, object>();
            GatAdPossaleTransferManager_Wif manager = new GatAdPossaleTransferManager_Wif();
            var p = new PageModel() { PageIndex = page, PageSize = pageSize };// 分页查询
            List<IConditionalModel> conModels = new List<IConditionalModel>();
            conModels.Add(new ConditionalModel() { FieldName = "SolutionID", ConditionalType = ConditionalType.Equal, FieldValue = solutionID.ToString() });
            if (!string.IsNullOrEmpty(startBillDate))
            {
                conModels.Add(new ConditionalModel() { FieldName = "billdate", ConditionalType = ConditionalType.GreaterThanOrEqual, FieldValue = startBillDate });
            }
            if (!string.IsNullOrEmpty(endBillDate))
            {
                conModels.Add(new ConditionalModel() { FieldName = "billdate", ConditionalType = ConditionalType.LessThanOrEqual, FieldValue = endBillDate });
            }
            if (!string.IsNullOrEmpty(materialCode))
            {
                conModels.Add(new ConditionalModel() { FieldName = "product_code", ConditionalType = ConditionalType.Like, FieldValue = materialCode });
            }
            if (!string.IsNullOrEmpty(sizeCode))
            {
                conModels.Add(new ConditionalModel() { FieldName = "size_code", ConditionalType = ConditionalType.Like, FieldValue = sizeCode });
            }
            if (!string.IsNullOrEmpty(shopCode))
            {
                conModels.Add(new ConditionalModel() { FieldName = "store_code", ConditionalType = ConditionalType.Like, FieldValue = shopCode });
            }
            var list = manager.GetPageList(conModels, p).OrderBy(s => s.billtime).ThenBy(s => s.BillNo);
            var totalCount = p.TotalCount;
            var qty = manager.AsQueryable().Where(conModels).Sum(s => s.qty);
            var amount = manager.AsQueryable().Where(conModels).Sum(s => s.tot_amt_actual);
            result.Add("data", list);
            result.Add("totalCount", totalCount);
            result.Add("qty", qty);
            result.Add("amount", amount ?? 0);
            return result;
        }

        public List<GatAdPersonnel_Wif> SelectPersonnelPageList_Wif(int currentPage, int itemsPerPage, int solutionID, ref int totalCount, ref decimal totalQty, string staffCode = "", string staffName = "", string shopCode = "")
        {
            GatAdPersonnelManager_Wif manager = new GatAdPersonnelManager_Wif();
            var p = new PageModel() { PageIndex = currentPage, PageSize = itemsPerPage };// 分页查询
            List<IConditionalModel> conModels = new List<IConditionalModel>();
            conModels.Add(new ConditionalModel() { FieldName = "SolutionID", ConditionalType = ConditionalType.Equal, FieldValue = solutionID.ToString() });

            if (!string.IsNullOrEmpty(shopCode))
            {
                conModels.Add(new ConditionalModel() { FieldName = "store_code", ConditionalType = ConditionalType.Like, FieldValue = shopCode });
            }

            if (!string.IsNullOrEmpty(staffCode))
            {
                conModels.Add(new ConditionalModel() { FieldName = "staff_code", ConditionalType = ConditionalType.Like, FieldValue = staffCode });
            }
            if (!string.IsNullOrEmpty(staffName))
            {
                conModels.Add(new ConditionalModel() { FieldName = "name", ConditionalType = ConditionalType.Like, FieldValue = staffName });
            }

            var result = manager.GetPageList(conModels, p).OrderBy(s => s.store_code);
            totalCount = p.TotalCount;
            totalQty = p.TotalCount;


            return result.ToList();
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
