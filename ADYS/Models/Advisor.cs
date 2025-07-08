using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace ADYS.Models
{
    public class Advisor
    {
        public int AdvisorId { get; set; }
        public string FullName { get; set; }  
        public string Email { get; set; }     

        public virtual ICollection<Student> Students { get; set; }
    }

}