using ADYS.Models;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Web;

namespace ADYS.ViewModels
{
    public class CourseSelectionViewModel
    {
        public int CourseId { get; set; }
        public string CourseName { get; set; }
        public int AKTS { get; set; }
        public bool IsSelected { get; set; }
        public string AdvisorName { get; set; }
        public string DayOfWeek { get; set; }
        public TimeSpan StartTime { get; set; }
        public TimeSpan EndTime { get; set; }
        public string DisplayTime => $"{DayOfWeek} {StartTime:hh\\:mm} - {EndTime:hh\\:mm}";
    }

}