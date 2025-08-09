using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace MyApiProject.DTOs
{
    public class StudentDashboardDto
    {
        public int StudentId { get; set; }
        public string StudentName { get; set; }
        public string StudentEmail { get; set; }
        public string AdvisorName { get; set; }
        public string AdvisorEmail { get; set; }
        public List<SelectedCourseDto> SelectedCourses { get; set; }
        public int TotalAKTS { get; set; }
        public bool AllApproved { get; set; }
    }

    public class SelectedCourseDto
    {
        public string CourseName { get; set; }
        public int AKTS { get; set; }
        public bool? IsApproved { get; set; }
    }
}