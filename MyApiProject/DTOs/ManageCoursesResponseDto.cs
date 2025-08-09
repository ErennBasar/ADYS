using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace MyApiProject.DTOs
{
    public class ManageCoursesResponseDto
    {
        public List<CourseDto> Courses { get; set; }
        public List<AdvisorDto> Advisors { get; set; }
        public List<DepartmentDto> Departments { get; set; }
    }
}