using System;
using System.Collections.Generic;

namespace aspNETuniversity.Models
{
    public partial class Faculty
    {
        public Faculty()
        {
            Specializations = new HashSet<Specialization>();
        }

        public int FacultyCode { get; set; }
        public string DeanName { get; set; } = null!;
        public string FacultyName { get; set; } = null!;

        public virtual ICollection<Specialization> Specializations { get; set; }
    }
}
