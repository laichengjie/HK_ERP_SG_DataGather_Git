using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Text;

using System.ComponentModel.DataAnnotations;

namespace Gat.DAL
{
    /// <summary>
    /// 供应商店铺
    /// </summary>
    [MetadataTypeAttribute(typeof(VendShopMetaData))]
    public partial class VendShop : ModuleBase
    {
        internal class VendShopMetaData
        {
            [Key]
            public int ID { get; set; }

            //[Required(AllowEmptyStrings = false, ErrorMessage = string.Format(ModuleBase.Null, "供应商代码"))]
            //[StringLength(20, ErrorMessage = string.Format(ModuleBase.MaxLength, "代码", 20))]

            [Required(AllowEmptyStrings = false, ErrorMessage =  "店铺代码不能为空！")]
            [StringLength(20, ErrorMessage = "店铺代码长度不能超过20！")]
            public string ShopCode { get; set; }

            //[Required(AllowEmptyStrings = false, ErrorMessage =  "店铺名称不能为空！")]
            [StringLength(50, ErrorMessage ="店铺名称长度不能超过50！")]
            public string ShopName { get; set; }

            //[Required(AllowEmptyStrings = false, ErrorMessage = "仓库代码不能为空！")]
            [StringLength(20, ErrorMessage =  "仓库代码长度不能超过20！")]
            public string StockCode { get; set; }

            //[Required(AllowEmptyStrings = false, ErrorMessage = "仓库名称不能为空！")]
            [StringLength(50, ErrorMessage = "仓仓库名称长度不能超过50！")]
            public string StockName { get; set; }
        }
    }
}