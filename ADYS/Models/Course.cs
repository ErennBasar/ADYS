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
        public virtual User Advisor { get; set; }

        public int DepartmentId { get; set; }
        public virtual Department Department { get; set; }

        public string DayOfWeek { get; set; } 
        public TimeSpan StartTime { get; set; } 
        public TimeSpan EndTime { get; set; }   

        public virtual ICollection<CourseSelection> CourseSelections { get; set; }
    }

}