using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace ADYS.ViewModels
{
    public class AuthenticatedUserViewModel
    {
        public int UserId { get; set; }
        public string UserName { get; set; }
        public string Email { get; set; }
        public string Role { get; set; }
    }
}