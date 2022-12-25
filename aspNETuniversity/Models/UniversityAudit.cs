using System;
using System.Collections.Generic;

namespace aspNETuniversity.Models
{
    public partial class UniversityAudit
    {
        public long Id { get; set; }
        public string? EventName { get; set; }
        public string EditBy { get; set; } = null!;
        public DateTime? EventDateTime { get; set; }
    }
}
