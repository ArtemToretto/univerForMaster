using System;
using System.Collections.Generic;

namespace aspNETuniversity.Models
{
    public partial class Student
    {
        public int Zachetka { get; set; }
        public string Name { get; set; } = null!;
        public int? SalaryFather { get; set; }
        public int? SalaryMother { get; set; }
        public byte? FamilyKol { get; set; }
        public string StudGroupCode { get; set; } = null!;

        public virtual StudGroup? StudGroupCodeNavigation { get; set; } = null!;
    }
}
