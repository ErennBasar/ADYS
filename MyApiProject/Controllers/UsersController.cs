using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Web.Http; 
using MyApiProject.Models;
using MyApiProject.DTOs;

namespace MyApiProject.Controllers
{
    public class UsersController : ApiController
    {
        private ContextDBEntities db = new ContextDBEntities();

        [HttpGet]
        //[Route("api/users")]
        public IHttpActionResult GetAllUsers()
        {
            
                var users = db.Users.Select(u => new UserDto
                {

                    UserId = u.UserId,
                    UserName = u.UserName,
                    Email = u.Email

                }).ToList();

                return Ok(users);
                                               
        }
    }
}
