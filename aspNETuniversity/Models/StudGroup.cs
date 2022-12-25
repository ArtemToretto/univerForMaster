using System;
using System.Collections.Generic;

namespace aspNETuniversity.Models
{
    public partial class StudGroup
    {
        public StudGroup()
        {
            Students = new HashSet<Student>();
        }

        public string StudGroupCode { get; set; } = null!;
        public short Year { get; set; }
        public int? SpecializationCode { get; set; }

        public virtual Specialization? SpecializationCodeNavigation { get; set; }
        public virtual ICollection<Student> Students { get; set; }
    }
}
