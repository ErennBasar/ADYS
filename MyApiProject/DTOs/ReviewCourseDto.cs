using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace MyApiProject.DTOs
{
    public class ReviewCourseDto
    {
        public int CourseSelectionId { get; set; }
        public string CourseName { get; set; }
        public int AKTS { get; set; }
        public bool? IsApproved { get; set; }
    }
    public class ReviewStudentCoursesDto
    {
        public int StudentId { get; set; }
        public string StudentName { get; set; }
        public List<ReviewCourseDto> Courses { get; set; }
    }
}