using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GatNewBll.Models.LiningNop
{
    public class WholesaleMasterData
    {
        /// <summary>
        /// 公司编码
        /// </summary>
        public string companyCode { get; set; }

        /// <summary>
        /// 单据编号
        /// </summary>
        public string externalOrderno { get; set; }

        /// <summary>
        /// 单据日期 20221027
        /// </summary>
        public DateTime orderDate { get; set; }

        /// <summary>
        /// 期货（QH）/现货（XH）
        /// </summary>
        public string qxh { get; set; }

        /// <summary>
        /// 收货方编码（分销商编码或分销商下属店铺编码，如传分销商编码则进入分销下默认店铺或仓库）
        /// </summary>
        public string receivingCode { get; set; }

        /// <summary>
        /// 备注
        /// </summary>
        public string remarks { get; set; }

        /// <summary>
        /// 客户编码
        /// </summary>
        public string VendCustCode { get; set; }

        /// <summary>
        /// 客户名称
        /// </summary>
        public string VendCustName { get; set; }
    }
}
