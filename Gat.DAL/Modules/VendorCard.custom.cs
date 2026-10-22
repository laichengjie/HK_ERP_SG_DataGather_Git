using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using System.ComponentModel.DataAnnotations;

namespace Gat.DAL
{
    /// <summary>
    /// 供应商品牌
    /// </summary>
    [MetadataTypeAttribute(typeof(VendorCardMetaData))]
    public partial class VendorCard : ModuleBase
    {
        internal class VendorCardMetaData
        {
            [RegularExpression(@"^[1-9]+[0-9]*$",ErrorMessage = "请您先指定供应商！")]
            public int VendorID { get; set; }

            [Required(AllowEmptyStrings = false, ErrorMessage = "品牌不能为空！")]
            public string CardID { get; set; }
        }
    }
}
