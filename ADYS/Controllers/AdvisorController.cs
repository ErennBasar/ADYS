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

        // 1. Danışman paneli (öğrenci listesi)
        public ActionResult Dashboard()
        {
            int advisorId = (int)(Session["AdvisorId"] ?? 0);

            if (advisorId == 0)
                return RedirectToAction("Advisor", "Login");

            var advisor = db.Users.Find(advisorId);
            ViewBag.AdvisorName = advisor?.UserName ?? "Danışman";

            // danışmana ait öğrenciler
            var studentList = db.AdvisorAssignments
                .Where(a => a.AdvisorId == advisorId)
                .Select(a => a.Student)
                .ToList();

            return View(studentList);
        }


        // 2. Ders seçimi yapmış öğrenciler (filtreli)
        public ActionResult Students()
        {
            int advisorId = (int)(Session["AdvisorId"] ?? 0);

            if (advisorId == 0)
                return RedirectToAction("Advisor", "Login");

            var advisor = db.Users.Find(advisorId);
            ViewBag.AdvisorName = advisor?.UserName ?? "Danışman";


            // danışmana ait öğrenciler
            var studentList = db.AdvisorAssignments
                .Where(a => a.AdvisorId == advisorId)
                .Select(a => a.Student)
                .Where(s => db.CourseSelections.Any(cs => cs.StudentId == s.UserId))
                .ToList();

            return View(studentList);
        }

        // 3. Onay paneli: öğrencinin seçtiği dersler
        public ActionResult Review(int studentId)
        {
            int advisorId = (int)(Session["AdvisorId"] ?? 0);

            var assignment = db.AdvisorAssignments
                       .FirstOrDefault(a => a.StudentId == studentId && a.AdvisorId == advisorId);

            var student = db.Users
                   .Include("CourseSelections.Course")
                  .FirstOrDefault(s => s.UserId == studentId);

            if (student == null) return HttpNotFound();

            var selections = db.CourseSelections
                .Include("Course")
                .Where(cs => cs.StudentId == studentId)
                .ToList();

            var model = selections.Select(cs => new ReviewCourseViewModel
            {
                CourseSelectionId = cs.CourseSelectionId,
                CourseName = cs.Course.CourseName,
                AKTS = cs.Course.AKTS,
                IsApproved = cs.IsApprovedByAdvisor
            }).ToList();


            ViewBag.StudentName = student.UserName;
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