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
            int studentId = (int)(Session["StudentId"] ?? 0);
            if (studentId == 0)
                return RedirectToAction("Student", "Login");

            var student = db.Students
                             .Include("Advisor")
                             .Include("CourseSelections.Course")
                             .FirstOrDefault(s => s.StudentId == studentId);

            if (student == null) return HttpNotFound();

            var selectedCourses = student.CourseSelections.Select(cs => new SelectedCourseViewModel
            {
                CourseName = cs.Course.CourseName,
                AKTS = cs.Course.AKTS,
                IsApproved = cs.IsApprovedByAdvisor
            }).ToList();

            var model = new StudentDashboardViewModel
            {
                StudentName = student.FullName,
                AdvisorName = student.Advisor.FullName,
                AdvisorEmail = student.Advisor.Email,
                SelectedCourses = selectedCourses,
                TotalAKTS = selectedCourses.Sum(c => c.AKTS),
                AllApproved = selectedCourses.All(c => c.IsApproved == true)
            };

            return View(model);
        }
        // GET: Student/SelectCourses
        public ActionResult SelectCourses()
        {
            int studentId = (int)(Session["StudentId"] ?? 0);
            if (studentId == 0)
                return RedirectToAction("Student", "Login");

            var selectedCourseIds = db.CourseSelections
                .Where(cs => cs.StudentId == studentId)
                .Select(cs => cs.CourseId)
                .ToList();

            var courses = db.Courses.ToList();

            var model = courses.Select(c => new CourseSelectionViewModel
            {
                CourseId = c.CourseId,
                CourseName = c.CourseName,
                AKTS = c.AKTS,
                IsSelected = selectedCourseIds.Contains(c.CourseId)
            }).ToList();

            
            return View(model);
        }


        // POST: Student/SelectCourses
        [HttpPost]
        public ActionResult SelectCourses(List<CourseSelectionViewModel> selectedCourses)
        {
            int studentId = (int)(Session["StudentId"] ?? 0);
            if (studentId == 0)
                return RedirectToAction("Student", "Login");

            var selected = selectedCourses
                .Where(c => c.IsSelected)
                .ToList();

            int totalAkts = selected.Sum(c => c.AKTS);

            if (totalAkts > 30)
            {
                ModelState.AddModelError("", "Toplam AKTS 30'dan fazla olamaz.");

                // Ders listesini tekrar yükle (çünkü View yeniden çiziliyor)
                var allCourses = db.Courses.ToList();
                var courseViewModels = allCourses.Select(c => new CourseSelectionViewModel
                {
                    CourseId = c.CourseId,
                    CourseName = c.CourseName,
                    AKTS = c.AKTS,
                    IsSelected = selectedCourses.Any(s => s.CourseId == c.CourseId && s.IsSelected)
                }).ToList();


                return View(courseViewModels);
            }

            // Önce varsa eski seçimleri sil
            var existingSelections = db.CourseSelections.Where(cs => cs.StudentId == studentId);
            db.CourseSelections.RemoveRange(existingSelections);

            // Seçilenleri ekle
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