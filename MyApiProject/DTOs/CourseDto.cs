using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace MyApiProject.DTOs
{
    public class CourseDto
    {
        public int CourseId { get; set; }
        public string CourseName { get; set; }
        public int Credits { get; set; }
        public int Quota { get; set; }

        public int AdvisorId { get; set; }
        public string AdvisorName { get; set; }

        public int DepartmentId { get; set; }
        public string DepartmentName { get; set; }

        public string Day { get; set; }
        public TimeSpan StartTime { get; set; }
        public TimeSpan EndTime { get; set; }
    }
}