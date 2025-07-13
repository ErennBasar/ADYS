using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace ADYS.Models
{
    public class Course
    {
        public int CourseId { get; set; }  
        public string CourseName { get; set; }
        public int AKTS { get; set; }
        public int Kontenjan { get; set; } 

        public int AdvisorId { get; set; }
        public virtual Advisor Advisor { get; set; }

        public int DepartmentId { get; set; }
        public virtual Department Department { get; set; }

        public virtual ICollection<CourseSelection> CourseSelections { get; set; }
    }

}