using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace ADYS.Models
{
    public class CourseSelection
    {
        public int CourseSelectionId { get; set; }  

        public int StudentId { get; set; }
        public virtual User Student { get; set; }

        public int CourseId { get; set; }
        public virtual Course Course { get; set; }

        public bool? IsApprovedByAdvisor { get; set; }
    }
}