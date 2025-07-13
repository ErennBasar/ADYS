using System;
using System.ComponentModel.DataAnnotations;

namespace ADYS.Models
{
    public class Term
    {
        public int TermId { get; set; }

        [Required]
        public string TermName { get; set; }

        public bool IsActive { get; set; }

        [DataType(DataType.Date)]
        public DateTime? CourseSelectionStart { get; set; }

        [DataType(DataType.Date)]
        public DateTime? CourseSelectionEnd { get; set; }
    }
}
