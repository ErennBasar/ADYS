using ADYS.Data;
using ADYS.ViewModels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace ADYS.Controllers
{
    public class AdvisorController : Controller
    {
        private readonly UniversityContext db = new UniversityContext();

        // 1. Danışman paneli (danışan listesi)
        public ActionResult Dashboard()
        {
            int advisorId = (int)(Session["AdvisorId"] ?? 0);
            if (advisorId == 0)
                return RedirectToAction("Advisor", "Login");

            var advisor = db.Advisors.Find(advisorId);
            ViewBag.AdvisorName = advisor?.FullName ?? "Danışman";

            var students = db.Students.Where(s => s.AdvisorId == advisorId).ToList();
            return View(students);
        }


        // 2. Ders seçimi yapmış öğrenciler (filtreli)
        public ActionResult Students()
        {
            int advisorId = (int)(Session["AdvisorId"] ?? 0);
            if (advisorId == 0)
                return RedirectToAction("Advisor", "Login");


            var students = db.Students
                .Where(s => s.AdvisorId == advisorId && s.CourseSelections.Any())
                .ToList();

            return View(students);
        }

        // 3. Onay paneli: öğrencinin seçtiği dersler
        public ActionResult Review(int studentId)
        {
            var student = db.Students
                .Include("CourseSelections.Course")
                .FirstOrDefault(s => s.StudentId == studentId);

            if (student == null) return HttpNotFound();

            var model = student.CourseSelections.Select(cs => new ReviewCourseViewModel
            {
                CourseSelectionId = cs.CourseSelectionId,
                CourseName = cs.Course.CourseName,
                AKTS = cs.Course.AKTS,
                IsApproved = cs.IsApprovedByAdvisor
            }).ToList();

            
            ViewBag.StudentName = student.FullName;
            ViewBag.StudentId = studentId;

            return View(model);
        }

        // 4. AJAX işlem: onaylama/red
        [HttpPost]
        public JsonResult UpdateApproval(int courseSelectionId, bool approve)
        {
            var selection = db.CourseSelections.Find(courseSelectionId);
            if (selection == null) return Json(new { success = false });

            selection.IsApprovedByAdvisor = approve;
            db.SaveChanges();

            return Json(new { success = true });
        }
    }

}