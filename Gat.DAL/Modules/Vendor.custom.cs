using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using System.ComponentModel.DataAnnotations;

namespace Gat.DAL
{
    [MetadataTypeAttribute(typeof(VendorMetaData))]
    public partial class Vendor : ModuleBase
    {
        internal class VendorMetaData
        {
            [Key]
            public int ID { get; set; }

            [RegularExpression("^[0-9a-zA-Z_]+$", ErrorMessage = "代码必须由字母、数字或者下划线组成")]
            [Required(AllowEmptyStrings = false, ErrorMessage = "供应商代码不能为空！")]
            [StringLength(20, ErrorMessage = "供应商代码长度不能超过20！")]
            public string VendorCode { get; set; }

            [Required(AllowEmptyStrings = false, ErrorMessage = "供应商名称不能为空！")]
            [StringLength(50, ErrorMessage = "供应商名称长度不能超过50！")]
            public string VendorName { get; set; }

        }
    }
}
