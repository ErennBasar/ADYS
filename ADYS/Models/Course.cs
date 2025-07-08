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

        public virtual ICollection<CourseSelection> CourseSelections { get; set; }
    }

}