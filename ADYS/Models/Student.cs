using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Web;

namespace ADYS.Models
{
    public class Student
    {
        public int StudentId { get; set; }
       
        [Required]
        public string FullName { get; set; }

        [Required]
        [EmailAddress]
        public string Email { get; set; }

        [Required]
        public string Password { get; set; }

        [Display(Name = "Danışman")]
        public int AdvisorId { get; set; }

        [ForeignKey("AdvisorId")]
        public virtual Advisor Advisor { get; set; }

        public virtual ICollection<CourseSelection> CourseSelections { get; set; }
    }

}