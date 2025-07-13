using System.ComponentModel.DataAnnotations;

namespace ADYS.Models
{
    public class Department
    {
        public int DepartmentId { get; set; }

        [Required]
        public string DepartmentName { get; set; }
    }
}
