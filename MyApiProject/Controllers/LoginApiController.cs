using MyApiProject.DTOs;
using MyApiProject.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web.Http;
using System.Web.UI.WebControls;

namespace MyApiProject.Controllers
{
    public class LoginApiController : ApiController
    {
        private ContextDBEntities db = new ContextDBEntities();

        [HttpPost]
        public IHttpActionResult Authenticate(LoginDto loginDto)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }
            var user = db.Users
            .Where(u => u.Email == loginDto.Email && u.Password == loginDto.Password)
            .Select(u => new AuthenticatedUserDto
            {
                UserId = u.UserId,
                UserName = u.UserName,
                Email = u.Email,
                Role = u.UserRoles.Select(r => r.Roles.RoleName).FirstOrDefault()
            })
            .FirstOrDefault();

            if (user == null)
                return NotFound(); // veya Unauthorized()

            return Ok(user);
        }
    }
}
