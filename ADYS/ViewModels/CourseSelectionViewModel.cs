using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace ADYS.ViewModels
{
    public class CourseSelectionViewModel
    {
        public int CourseId { get; set; }
        public string CourseName { get; set; }
        public int AKTS { get; set; }
        public bool IsSelected { get; set; }
    }

}