using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web.Http;
using System.Data.Entity;
using MyApiProject.Models;
using MyApiProject.DTOs;
using System.Web.Http.Cors;

namespace MyApiProject.Controllers
{
    [EnableCors(origins: "*", headers: "*", methods: "*")]
    public class AdvisorApiController : ApiController
    {
        private ContextDBEntities db = new ContextDBEntities();

        // GET: api/AdvisorApi
        // (öğrenci listesi)
        [HttpGet]
        public IHttpActionResult Dashboard(int advisorId)
        {
            var advisor = db.Users.Find(advisorId);
            if (advisorId == 0) return NotFound();

            // Danışmana ait öğrenciler
            var studentList = db.AdvisorAssignments
                .Where(a => a.AdvisorId == advisorId)
                .Select(a => new AdvisorStudentDto
                {
                    StudentId = a.Student.UserId,
                    StudentName = a.Student.UserName,
                    Email = a.Student.Email,
                })
                .ToList();

            var model = new AdvisorDashboardDto
            {
                AdvisorName = advisor.UserName,
                Students = studentList
            };

            return Ok(model);        
        }

        [HttpGet]
        public IHttpActionResult Students(int advisorId)
        {
            if (advisorId == 0) return NotFound();

            var advisor = db.Users.Find(advisorId);

            var studentList = db.AdvisorAssignments
                .Where(a => a.AdvisorId == advisorId)
                .Select(a => a.Student)
                .Where(s => db.CourseSelections.Any(cs => cs.StudentId == s.UserId))
                .Select(s => new StudentSelectCoursesDto
                {
                    StudentId = s.UserId,
                    StudentName = s.UserName,
                    Email = s.Email
                })
                .ToList();

            return Ok(studentList);
        }

        [HttpGet]
        public IHttpActionResult Review(int studentId)
        {
            var student = db.Users
                .Include(s => s.CourseSelections.Select(cs => cs.Courses))
                .FirstOrDefault(s => s.UserId == studentId);

            if (student == null)
                return NotFound();

            var courseDtos = student.CourseSelections
                .Select(cs => new ReviewCourseDto
                {
                    CourseSelectionId = cs.CourseSelectionId,
                    CourseName = cs.Courses.CourseName,
                    AKTS = cs.Courses.AKTS,
                    IsApproved = cs.IsApprovedByAdvisor
                }).ToList();

            var response = new ReviewStudentCoursesDto
            {
                StudentId = studentId,
                StudentName = student.UserName,
                Courses = courseDtos
            };

            return Ok(response);
        }

        [HttpPost]
        public IHttpActionResult UpdateApproval(int courseSelectionId, bool approve)
        {
            var selection = db.CourseSelections.Find(courseSelectionId);
            if (selection == null)
                return BadRequest("Ders bulunamadı.");

            selection.IsApprovedByAdvisor = approve;
            db.SaveChanges();

            return Ok(new { success = true });
        }

    }
}
