using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace MyApiProject.DTOs
{
    public class TermsDto
    {
        public int? TermId { get; set; }
        public string TermName { get; set; }
        public bool IsActive { get; set; }
        public DateTime? TermStartTime {  get; set; }
        public DateTime? TermEndTime { get; set; }
    }
}