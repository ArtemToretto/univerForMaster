using System;
using System.Collections.Generic;

namespace aspNETuniversity.Models
{
    public partial class StudentsAndAverageSalary
    {
        public string Asname { get; set; } = null!;
        public string GroupCode { get; set; } = null!;
        public string SpecializationName { get; set; } = null!;
        public string FacultyName { get; set; } = null!;
        public int? AverageGroupSalary { get; set; }
    }
}
