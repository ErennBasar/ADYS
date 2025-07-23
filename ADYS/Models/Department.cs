using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace ADYS.Models
{
    public class Department
    {
        public int DepartmentId { get; set; }

        [Required]
        public string DepartmentName { get; set; }
        public virtual ICollection<Course> Courses { get; set; }

    }
}
