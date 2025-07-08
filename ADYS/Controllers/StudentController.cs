using ADYS.Data;
using ADYS.Models;
using ADYS.ViewModels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace ADYS.Controllers
{
    public class StudentController : Controller
    {
        private readonly UniversityContext db = new UniversityContext();

        // GET: Student
        // Öğrenci paneli
        public ActionResult Dashboard()
        {
            // Giriş yapan öğrenci ID'sini al (örnek olarak 1 yazıldı, giriş sistemine göre güncellenmeli)
            int studentId = 1;

            var student = db.Students.Include("Advisor")
                             .FirstOrDefault(s => s.StudentId == studentId);

            if (student == null)
                return HttpNotFound();

            var model = new StudentDashboardViewModel
            {
                StudentName = student.FullName,
                AdvisorName = student.Advisor.FullName,
                AdvisorEmail = "danisman@universite.edu.tr" // Advisor modeline eklenmişse direkt kullanılabilir
            };

            return View(model);
        }
        // GET: Student/SelectCourses
        public ActionResult SelectCourses()
        {
            var courses = db.Courses.ToList();

            var model = courses.Select(c => new CourseSelectionViewModel
            {
                CourseId = c.CourseId,
                CourseName = c.CourseName,
                AKTS = c.AKTS,
                IsSelected = false
            }).ToList();

            return View(model);
        }

        // POST: Student/SelectCourses
        [HttpPost]
        public ActionResult SelectCourses(List<CourseSelectionViewModel> selectedCourses)
        {
            int studentId = 1; // Test için sabit

            var selected = selectedCourses
                .Where(c => c.IsSelected)
                .ToList();

            int totalAkts = selected.Sum(c => c.AKTS);

            if (totalAkts > 30)
            {
                ModelState.AddModelError("", "Toplam AKTS 30'dan fazla olamaz.");
                return View(selectedCourses);
            }

            foreach (var course in selected)
            {
                db.CourseSelections.Add(new CourseSelection
                {
                    StudentId = studentId,
                    CourseId = course.CourseId,
                    IsApprovedByAdvisor = false
                });
            }

            db.SaveChanges();
            return RedirectToAction("Dashboard");
        }

    }
}