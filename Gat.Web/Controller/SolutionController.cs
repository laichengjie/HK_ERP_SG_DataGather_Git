using Gat.Utils;
using Gat.Web.Controller.Dtos;
using GatNewBll.Blls;
using GatNewBll.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Http;

namespace Gat.Web.Controller
{
    public class SolutionController : ApiController
    {
        /// <summary>
        /// 获取提报方案
        /// </summary>
        /// <param name="vendorId">供应商Id</param>
        /// <returns></returns>
        [HttpPost]
        public List<SolutionBaseData> GetVendorSolution(int vendorId)
        {
            SolutionBll bll = new SolutionBll();
            var result = bll.GetSolutionByVendorId(vendorId);
            return result;
        }

        /// <summary>
        /// 手动提报
        /// </summary>
        /// <param name="solutionId">解决方案Id</param>
        /// <param name="brandCode">品牌编码</param>
        /// <param name="startTime">开始时间</param>
        /// <param name="endTime">结束时间</param>
        /// <param name="positive">是否正常销售，true为正常销售，false为退货，仅支持李宁会员供应商品牌</param>
        /// <returns></returns>
        [HttpPost]
        public ResponseResultDto<object> ManualGat(int solutionId, string brandCode, DateTime startTime, DateTime endTime, bool positive=false)
        {
            string VendorCode = brandCode.ToLower();
            if (brandCode.ToLower() == "vans")
            {
                VansBill vansbill = new VansBill();
                ReportMap Report = vansbill.GetVendorReportPath(solutionId);
                if (Report == ReportMap.Vans)
                {
                    VendorCode = "vans_em".ToLower();
                }
            }
            ResponseResultDto<object> result = new ResponseResultDto<object>();
            try
            {
                SolutionBll bll = new SolutionBll();
                HandleArgs HANDLE = new HandleArgs();
                switch (VendorCode)
                {
                    case "skechers":
                        HANDLE = bll.SkechersPost(solutionId, startTime, endTime, false);
                        break;
                    case "crocs":
                        HANDLE = bll.CrocsPost(solutionId, startTime, endTime, false);
                        break;
                    case "lining_memeber":
                        HANDLE = new LiningMemberBll().NormalPost(solutionId, startTime, endTime, positive ? "1" : "0");
                        break;
                    case "lining_fx":
                        HANDLE = new LiningNopBll().NormalPost(solutionId, startTime, endTime);
                        break;
                    case "tumi":
                        HANDLE = new TuMiBll().NormalTuMiPost(solutionId, startTime, endTime);
                        break; 
                    case "on":
                        HANDLE = new OnBill().NormalOnPost(solutionId, startTime, endTime, positive);
                        break;
                    case "ln_oms":
                        HANDLE = new LiningQqdBll().NormalPost(solutionId, startTime, endTime,false);
                        break;
                    case "arcteryx":
                        HANDLE = new ArcTeryxBill().PushAllArcTeryxPos(solutionId, startTime, endTime, positive);
                        break;
                    case "burgeon":
                        HANDLE = new BurgeonBill().PushAllBurgeonPos(solutionId, startTime, endTime, positive);
                        break;
                    case "vans_em":
                        HANDLE = new VansBill().VansPush(solutionId, startTime, endTime, 0);
                        break;
                    default:
                        break;
                }
                if (HANDLE.Status == HandleStatus.Completed)
                {
                    result.Success = true;
                }
                else
                {
                    result.Success = false;
                    result.Message = HANDLE.Message;
                }
                return result;
            }
            catch (Exception ex)
            {
                result.Success = false;
                result.Message = ex.ToString();
                return result;
            }
        }

        /// <summary>
        /// 获取所有后台提报的提报计划
        /// </summary>
        /// <returns></returns>
        [HttpPost]
        public List<SolutionModel> GetOnlyBackRunSolution()
        {
            SolutionBll bll = new SolutionBll();
            return bll.GetOnlyBackRunSolution();
        }
    }
}