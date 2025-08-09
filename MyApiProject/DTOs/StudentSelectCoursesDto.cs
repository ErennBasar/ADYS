using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace MyApiProject.DTOs
{
    public class StudentSelectCoursesDto
    {
        public int StudentId { get; set; }
        public string StudentName { get; set; }
        public string Email { get; set; }
    }
}