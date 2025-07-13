using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace ADYS.ViewModels
{
    public class StudentDashboardViewModel
    {
        public string StudentName { get; set; }
        public string AdvisorName { get; set; }
        public string AdvisorEmail { get; set; }
        public int TotalAKTS { get; set; }
        public bool AllApproved { get; set; }
        public List<SelectedCourseViewModel> SelectedCourses { get; set; }

    }
}