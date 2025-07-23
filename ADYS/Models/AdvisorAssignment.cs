using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace ADYS.Models
{
    public class AdvisorAssignment
    {
        public int AdvisorAssignmentId { get; set; }

        public int StudentId { get; set; }
        public virtual User Student { get; set; }

        public int AdvisorId { get; set; }
        public virtual User Advisor { get; set; }

    }

}