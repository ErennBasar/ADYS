using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace MyApiProject.DTOs
{
    public class CourseSelectionDto
    {
        public int CourseId { get; set; }
        public string CourseName { get; set; }
        public int AKTS { get; set; }
        public string DayOfWeek { get; set; }
        public TimeSpan StartTime { get; set; }
        public TimeSpan EndTime { get; set; }
        public bool IsSelected { get; set; }
        public string AdvisorName { get; set; }
        public int Capacity { get; set; }
        public int SelectedStudentCount { get; set; }
    }
  
}