using System;
using System.Collections.Generic;

namespace aspNETuniversity.Models
{
    public partial class AverageSalarySmaller
    {
        public string Name { get; set; } = null!;
        public string GroupCode { get; set; } = null!;
        public int? AverageSalaryForOnePeople { get; set; }
    }
}
