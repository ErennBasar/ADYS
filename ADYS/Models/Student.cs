using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace ADYS.Models
{
    public class Student
    {
        public int StudentId { get; set; }      
        public string FullName { get; set; }
        public int AdvisorId { get; set; }

        public virtual Advisor Advisor { get; set; }
        public virtual ICollection<CourseSelection> CourseSelections { get; set; }
    }

}