using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GatNewBll.Models.LiningQqd
{
   public  class LQPOSSalOrderDataReq
    {
		public List<LQdata> datas { get; set; }
		
    }
	public class LQdata
	{

		/// <summary>
		/// 日期（年月日） yyyymmdd
		/// </summary>
		public string docDate { get; set; }
		/// <summary>
		/// 销售流水
		/// </summary>
		public string gmv { get; set; }
		/// <summary>
		/// 下单门店编码 A158
		/// </summary>
		public string shopCode { get; set; }

	}
}
