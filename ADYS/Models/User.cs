using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace ADYS.Models
{
    public class User
    {
        public int UserId { get; set; }
        public string UserName { get; set; }
        public string Email { get; set; }
        public string Password { get; set; }

        public bool IsActive { get; set; } = true;

        public bool IsDeleted { get; set; } = false;

        public DateTime? DeleteDate { get; set; }

        public DateTime CreateDate { get; set; } = DateTime.Now;

        public DateTime? UpdateDate { get; set; }

        public virtual ICollection<AdvisorAssignment> AdvisorStudents { get; set; } // Bu danışmanın öğrencileri
        public virtual ICollection<AdvisorAssignment> AdvisorAssignments { get; set; } // Bu öğrencinin danışman atamaları


        public virtual ICollection<UserRole> UserRoles { get; set; }
        public virtual ICollection<CourseSelection> CourseSelections { get; set; }
        //public virtual ICollection<User> Students { get; set; }

    }
}