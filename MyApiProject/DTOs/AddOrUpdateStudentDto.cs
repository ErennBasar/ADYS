using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace MyApiProject.DTOs
{
    public class AddOrUpdateStudentDto
    {
        public int? StudentId { get; set; }
        public string FullName { get; set; }
        public string Email { get; set; }
        public string Password { get; set; }
        public int AdvisorId { get; set; }
    }
}