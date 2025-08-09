using MyApiProject.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using MyApiProject.DTOs;
using System.Net;
using System.Net.Http;
using System.Web.Http;
using System.Data.Entity;


namespace MyApiProject.Controllers
{
    public class StudentApiController : ApiController
    {
        private ContextDBEntities db = new ContextDBEntities();

        // GET: api/StudentApi
        // GET: api/student/dashboard/{id}
        [HttpGet]

        public IHttpActionResult Dashboard(string id)
        {
            if (int.TryParse(id, out var gelenStudentId) && gelenStudentId == 0) return NotFound();

            var student = db.Users.FirstOrDefault(u => u.UserId == gelenStudentId); // Student bilgilerini al

            if (student == null) return NotFound();

            
            var advisorAssignment = db.AdvisorAssignments // Danışman atamalarını içeren ilişkiyi dahil et
                               .Include(a => a.Advisor) 
                               .FirstOrDefault(a => a.StudentId == gelenStudentId); // Danışman atamasını al

            var advisor = advisorAssignment?.Advisor; // Danışmanı al

            var selectedCourses = db.CourseSelections // Seçilen dersleri içeren ilişkiyi dahil et
                .Where(cs => cs.StudentId == gelenStudentId)
                .Include(cs => cs.Courses)
                .ToList();

            
            var selectedCoursesDto = selectedCourses.Select(cs => new SelectedCourseDto // Seçilen dersleri DTO'ya dönüştür
            {
                CourseName = cs.Courses?.CourseName ?? "-",
                AKTS = cs.Courses?.AKTS ?? 0,
                IsApproved = cs.IsApprovedByAdvisor
            }).ToList();

            var dto = new StudentDashboardDto // Öğrenci paneli için DTO oluştur
            {
                StudentId = student.UserId,
                StudentName = student.UserName,
                AdvisorName = advisor?.UserName ?? "Danışman atanmamış",
                AdvisorEmail = advisor?.Email ?? "-",
                SelectedCourses = selectedCoursesDto,
                TotalAKTS = selectedCoursesDto.Sum(x => x.AKTS),
                AllApproved = selectedCoursesDto.All(c => c.IsApproved == true)
            };
            return Ok(dto);
        }

        // GET /api/StudentApi/SelectCourses?studentId=6
        [HttpGet]
        public IHttpActionResult SelectCourses(int studentId)
        {
            var activeTerm = db.Terms.FirstOrDefault(t => t.IsActive);
            if(activeTerm == null)
            {
                return BadRequest("Aktif bir dönem bulunamadı.");
            }

            var selectedCourseIds = db.CourseSelections // Seçilen derslerin ID'lerini al
              .Where(cs => cs.StudentId == studentId)
              .Select(cs => cs.CourseId)
              .ToList();

            var courses = db.Courses.Include(c => c.Users).ToList();  //advisor bilgisi alındı
            //var courses = db.Courses.Include("Advisor").ToList();

            var model = courses.Select(c => new CourseSelectionDto
            {
                CourseId = c.CourseId,
                CourseName = c.CourseName,
                AKTS = c.AKTS,
                StartTime = c.StartTime,
                DayOfWeek = c.DayOfWeek,
                EndTime = c.EndTime,
                IsSelected = selectedCourseIds.Contains(c.CourseId),
                AdvisorName = c.Users?.UserName ?? "Belirtilmemiş",

                // Dersi seçen öğrencilerin sayısı hesaplanıyor
                Capacity = c.Kontenjan,
                SelectedStudentCount = db.CourseSelections.Count(cs => cs.CourseId == c.CourseId)
            }).ToList();

            return Ok(model);
                
        }
        [HttpPost]
        public IHttpActionResult SubmitSelectedCourses(List<SelectCourseDto> selectedCourses)
        {
            if (selectedCourses == null || !selectedCourses.Any())
            {
                return BadRequest("Seçili ders bulunamadı.");
            }

            int studentId = selectedCourses.First().StudentId;

            // Eski seçimleri sil
            var existingSelections = db.CourseSelections
                .Where(cs => cs.StudentId == studentId)
                .ToList();

            db.CourseSelections.RemoveRange(existingSelections);

            // Yeni seçimleri kaydet
            foreach (var course in selectedCourses)
            {
                db.CourseSelections.Add(new CourseSelections
                {
                    StudentId = course.StudentId,
                    CourseId = course.CourseId,
                    IsApprovedByAdvisor = null
                });
            }

            db.SaveChanges();

            return Ok("Ders seçimleri başarıyla güncellendi.");
        }


    }
}
