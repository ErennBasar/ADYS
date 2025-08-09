using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace ADYS.ViewModels
{
    public class AdvisorDashboardViewModel
    {
        public string AdvisorName { get; set; }
        public List<StudentViewModel> Students { get; set; }
    }

    public class StudentViewModel
    {
        public int StudentId { get; set; }
        public string StudentName { get; set; }
        public string Email { get; set; }
    }
}