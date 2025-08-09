using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using MyApiProject.DTOs;

namespace MyApiProject.DTOs
{
    public class SelectCourseDto
    {
        public int CourseId { get; set; }
        public int StudentId { get; set; }
    }
}