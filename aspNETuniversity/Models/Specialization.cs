using System;
using System.Collections.Generic;

namespace aspNETuniversity.Models
{
    public partial class Specialization
    {
        public Specialization()
        {
            StudGroups = new HashSet<StudGroup>();
        }

        public int SpecCode { get; set; }
        public string Cvalification { get; set; } = null!;
        public string Name { get; set; } = null!;
        public int FacultyCode { get; set; }

        public virtual Faculty? FacultyCodeNavigation { get; set; } = null!;
        public virtual ICollection<StudGroup> StudGroups { get; set; }
    }
}
