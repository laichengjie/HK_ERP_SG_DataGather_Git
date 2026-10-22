using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using System.ComponentModel.DataAnnotations;

namespace Gat.DAL
{
    [MetadataTypeAttribute(typeof(SolutionMetaData))]
    public partial class Solution : ModuleBase
    {

        internal class SolutionMetaData
        {
            [Key]
            public int ID { get; set; }

            [Required(AllowEmptyStrings = false, ErrorMessage = "标题不能为空！")]
            [StringLength(50, ErrorMessage ="标题长度不能超过50！")]
            public string Title { get; set; }

        }
    }
}
