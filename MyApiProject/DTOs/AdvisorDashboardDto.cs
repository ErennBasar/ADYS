using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace MyApiProject.DTOs
{
    public class AdvisorDashboardDto
    {
        public string AdvisorName { get; set; }
        public List<AdvisorStudentDto> Students { get; set; }
    }
    public class AdvisorStudentDto
    {
        public int StudentId { get; set; }
        public string StudentName { get; set; }
        public string Email { get; set; }
    }
}