﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace ADYS.Models
{
    public class Role
    {
        public int RoleId { get; set; }
        public string RoleName { get; set; }
        public virtual ICollection <UserRole> UserRoles { get; set; }
    }
}
