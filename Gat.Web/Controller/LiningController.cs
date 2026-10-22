using Gat.Utils;
using Gat.Web.Controller.Dtos;
using GatNewBll.Blls;
using System;
using System.Collections.Generic;
using System.Data;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Reflection;
using System.Web;
using System.Web.Http;
using System.Web.Mvc;

namespace Gat.Web.Controller
{
    public class LiningController : ApiController
    {
        [System.Web.Http.HttpPost]
        public LiningPosSalePageDto GetPageList(LiningPosSaleQueryDto input)
        {
            LiningPosSalePageDto result = new LiningPosSalePageDto();
            LiningBll bll = new LiningBll();
            int totalCount = 0;
            decimal totalQty = 0;
            decimal? totalAmount = 0;
            int solutionIdValue = 0;
            if (input.SolutionId.HasValue)
            {
                solutionIdValue = input.SolutionId.Value;
            }
            else
            {
                SolutionBll solBll = new SolutionBll();
                var solution = solBll.GetFirstSolutionByVendorId(input.VendorId.Value);
                solutionIdValue = solution.Id;
            }
            var pageList = bll.GetPageList(solutionIdValue, input.Page, input.PageSize, input.StartBillTime, input.EndBillTime,
                input.StartBillDate, input.EndBillDate, input.ShopCode, input.MaterialCode, input.SizeName, ref totalCount, ref totalQty, ref totalAmount);
            result.TotalCount = totalCount;
            result.Data = pageList;
            result.Qty = totalQty;
            result.Amount = totalAmount;
            return result;
        }

        [System.Web.Http.HttpPost]
        public CommonResponseDto<object> DeleteSearch(LiningDeleteSearchInputDto input)
        {
            CommonResponseDto<object> result = new CommonResponseDto<object>();
            if (input.SolutionId == 0)
            {
                result.Success = false;
                result.Message = "solutionId参数不能为空";
                return result;
            }
            try
            {
                LiningBll bll = new LiningBll();
                var delRes = bll.Delete(null, input.SolutionId, input.StartBillTime, input.EndBillTime,
                input.StartBillDate, input.EndBillDate, input.ShopCode, input.MaterialCode, input.SizeName);
                if (delRes)
                {
                    result.Success = true;
                }
            }
            catch (Exception ex)
            {
                result.Success = false;
                result.Message = ex.ToString();
            }

            return result;
        }

        [System.Web.Http.HttpPost]
        public CommonResponseDto<object> DeleteById(List<int> ids)
        {
            CommonResponseDto<object> result = new CommonResponseDto<object>();
            if (ids == null || ids.Count() == 0)
            {
                result.Success = false;
                result.Message = "ids参数不能为空";
                return result;
            }
            try
            {
                LiningBll bll = new LiningBll();
                var delRes = bll.Delete(ids);
                if (delRes)
                {
                    result.Success = true;
                }
            }
            catch (Exception ex)
            {
                result.Success = false;
                result.Message = ex.ToString();
            }

            return result;
        }

        [System.Web.Http.HttpPost]
        public IHttpActionResult ExportExcel(LiningDeleteSearchInputDto input)
        {
            LiningBll bll = new LiningBll();
            var exportList = bll.GetExportList(input.SolutionId, input.StartBillTime, input.EndBillTime, input.StartBillDate, input.EndBillDate, input.ShopCode, input.MaterialCode, input.SizeName);

            System.Data.DataTable dtSourceStock = exportList.ToDataTable(new string[] { "ShopCode", "ShopName", "BillNo", "BillDate", "MaterialCode", "SizeName", "BarCode", "SaleAmount", "Qty", "PersonelName", "SaleType", "MemberMobile" });
            string fileName = "李宁的销售提报.xlsx";
            dtSourceStock.Columns["ShopCode"].ColumnName = "门店代码";
            dtSourceStock.Columns["ShopName"].ColumnName = "店铺名称";
            dtSourceStock.Columns["BillNo"].ColumnName = "交易单号";
            dtSourceStock.Columns["BillDate"].ColumnName = "交易日期";
            dtSourceStock.Columns["MaterialCode"].ColumnName = "商品编码";
            dtSourceStock.Columns["SizeName"].ColumnName = "产品尺码";
            dtSourceStock.Columns["BarCode"].ColumnName = "条码";
            dtSourceStock.Columns["SaleAmount"].ColumnName = "销售金额";
            dtSourceStock.Columns["Qty"].ColumnName = "销售数量";
            dtSourceStock.Columns["PersonelName"].ColumnName = "业务员名称";
            dtSourceStock.Columns["SaleType"].ColumnName = "销售类型";
            dtSourceStock.Columns["MemberMobile"].ColumnName = "会员手机号";

            ExcelHelper Excel = new ExcelHelper();
            MemoryStream ms = Excel.GridToExcelXSSFByNPOI(dtSourceStock, fileName);
            //HttpResponseMessage response = new HttpResponseMessage(HttpStatusCode.OK);
            //response.Content = new StreamContent(ms);
            //response.Content.Headers.ContentType = new MediaTypeHeaderValue("application/octet-stream");
            //response.Content.Headers.ContentDisposition = new ContentDispositionHeaderValue("attachment")
            //{
            //    FileName = HttpUtility.UrlEncode(DateTime.Now.ToShortDateString() + "订单数据.xls")
            //};
            //return response;
            //OutputAction act = new OutputAction(ms.ToArray());
            //act.ContentType = "application/octet-stream";
            //act.DownloadFileName = DateTime.Now.ToShortDateString() + "订单数据.xls";
            //return act;
            var result = new FileByteResult(ms.ToArray(), "application/octet-stream", DateTime.Now.ToShortDateString() + "订单数据.xls");
            return result;
        }

        /// <summary>
        /// 将泛类型集合List类转换成DataTable
        /// </summary>
        /// <param name="list">泛类型集合</param>
        /// <returns></returns>
        public DataTable ListToDataTable<T>(List<T> entitys)
        {
            //检查实体集合不能为空
            if (entitys == null || entitys.Count < 1)
            {
                throw new Exception("需转换的集合为空");
            }
            //取出第一个实体的所有Propertie
            Type entityType = entitys[0].GetType();
            PropertyInfo[] entityProperties = entityType.GetProperties();

            //生成DataTable的structure
            //生产代码中，应将生成的DataTable结构Cache起来，此处略
            DataTable dt = new DataTable();
            for (int i = 0; i < entityProperties.Length; i++)
            {
                //dt.Columns.Add(entityProperties[i].Name, entityProperties[i].PropertyType);
                dt.Columns.Add(entityProperties[i].Name);
            }
            //将所有entity添加到DataTable中
            foreach (object entity in entitys)
            {
                //检查所有的的实体都为同一类型
                if (entity.GetType() != entityType)
                {
                    throw new Exception("要转换的集合元素类型不一致");
                }
                object[] entityValues = new object[entityProperties.Length];
                for (int i = 0; i < entityProperties.Length; i++)
                {
                    entityValues[i] = entityProperties[i].GetValue(entity, null);
                }
                dt.Rows.Add(entityValues);
            }
            return dt;
        }
    }
}