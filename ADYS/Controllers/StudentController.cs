using ADYS.Data;
using ADYS.Models;
using ADYS.ViewModels;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data.Entity;
using System.Linq;
using System.Web;
using System.Web.Mvc;


namespace ADYS.Controllers
{
    public class StudentController : Controller
    {
        private readonly UniversityContext db = new UniversityContext();

        // GET: Student
        // Öğrenci panelis
        //[OutputCache(NoStore = true, Duration = 0, VaryByParam = "*")]
        public ActionResult Dashboard(int studentId)
        {
            if (Session["UserRole"]?.ToString() != "Student"
                || Session["StudentId"] == null
                || (int)Session["StudentId"] != studentId)
            {
                TempData["ErrorMessage"] = "Bu sayfaya erişmek için giriş yapmalısınız.";
                return RedirectToAction("GeneralLogin", "Login");
            }

            var activeTerm = db.Terms.FirstOrDefault(t => t.IsActive);
            ViewBag.ActiveTermName = activeTerm != null ? activeTerm.TermName : "Aktif dönem bulunmamaktadır.";

            var student = db.Users
                             .Include("UserRoles.Role")
                          // .Include("Advisor")
                          // .Include("CourseSelections.Course")
                             .FirstOrDefault(u => u.UserId == studentId && 
                                             u.UserRoles.Any(ur => ur.Role.RoleName == "Student"));

            if (student == null) return HttpNotFound();

            var courseSelections = db.CourseSelections
                            .Include(cs => cs.Course)
                            .Where(cs => cs.StudentId == studentId)
                         //   .AsNoTracking()
                            .ToList();

            var advisors = db.AdvisorAssignments
                    .Include(a => a.Advisor)
                    .Where(a => a.StudentId == studentId)
                    .Select(a => a.Advisor)
                  //  .AsNoTracking()
                    .FirstOrDefault();

            var advisorAssignment = db.AdvisorAssignments
                                    .Include(a => a.Advisor)
                                    .FirstOrDefault(a => a.StudentId == studentId);


            var advisor = db.Users.Find(advisorAssignment.AdvisorId); //Find() içine nesne değil, ID değeri verilir. //Ogrenci2 danışmana tanımlı değil o yüzden hata veriyor.

            var selectedCourses = courseSelections
                          .Select(cs => new SelectedCourseViewModel
                          {
                              CourseName = cs.Course?.CourseName ?? "-",
                              AKTS = cs.Course?.AKTS ?? 0,
                              IsApproved = cs.IsApprovedByAdvisor
                          })
                          .ToList();

            //var courseSelections = db.CourseSelections
            //                 .Include(cs => cs.Course)
            //                 .Where(cs => cs.StudentId == studentId)
            //                 .ToList();

            //var selectedCourses = student.CourseSelections.Select(
            //    cs => new SelectedCourseViewModel
            //    {
            //        CourseName = cs.Course.CourseName,
            //        AKTS = cs.Course.AKTS,
            //        IsApproved = cs.IsApprovedByAdvisor

            //    }).ToList();

            var model = new StudentDashboardViewModel
            {
                StudentId = student.UserId,
                StudentName = student.UserName,
                AdvisorName = advisor?.UserName ?? "Danışman atanmamış",
                AdvisorEmail = advisor?.Email ?? "-",
                SelectedCourses = selectedCourses,
                TotalAKTS = selectedCourses.Sum(c => c.AKTS),
                AllApproved = selectedCourses.All(c => c.IsApproved == true)
            };

            return View(model);
        }
        // GET: Student/SelectCourses
        //[OutputCache(NoStore = true, Duration = 0, VaryByParam = "*")]
        public ActionResult SelectCourses()
        {
            if (Session["UserRole"]?.ToString() != "Student"
               || Session["StudentId"] == null)
            {
                TempData["ErrorMessage"] = "Bu sayfaya erişmek için giriş yapmalısınız.";
                return RedirectToAction("GeneralLogin", "Login");
            }

            int studentId = (int)Session["StudentId"];

            // Aktif dönem kontrolü
            var activeTerm = db.Terms.FirstOrDefault(t => t.IsActive);

            //if (activeTerm == null)
            //{
            //    // admin aktif dönem ayarlanana kadar RedirectToAction student/selectcourses sonra student/dashboard
            //    TempData["ErrorMessage"] = "Şu anda aktif bir dönem bulunmamaktadır. Ders seçimi yapılamaz.";
            //    return RedirectToAction("Dashboard", "Student", new { studentId });
            //}

            //  Tarih aralığı kontrolü
            var now = DateTime.Now;
            if (activeTerm.CourseSelectionStart.HasValue && activeTerm.CourseSelectionEnd.HasValue)
            {
                if (now < activeTerm.CourseSelectionStart.Value || now > activeTerm.CourseSelectionEnd.Value)
                {
                    TempData["ErrorMessage"] = $"Ders seçimi yalnızca {activeTerm.CourseSelectionStart:dd.MM.yyyy} - {activeTerm.CourseSelectionEnd:dd.MM.yyyy} tarihleri arasında yapılabilir.";
                    return RedirectToAction("Dashboard", "Student", new { studentId });
                }
            }

            var selectedCourseIds = db.CourseSelections
                .Where(cs => cs.StudentId == studentId)
                .Select(cs => cs.CourseId)
                .ToList();

            var courses = db.Courses.Include(c => c.Advisor).ToList();

            var model = courses.Select(c => new CourseSelectionViewModel
            {
                CourseId = c.CourseId,
                CourseName = c.CourseName,
                AKTS = c.AKTS,
                StartTime = c.StartTime,
                DayOfWeek = c.DayOfWeek,
                EndTime = c.EndTime,
                IsSelected = selectedCourseIds.Contains(c.CourseId),
                AdvisorName = c.Advisor?.UserName ?? "Belirtilmemiş",

                // Dersi seçen öğrencilerin sayısı hesaplanıyor
                Capacity = c.Kontenjan,
                SelectedStudentCount = db.CourseSelections.Count(cs => cs.CourseId == c.CourseId)
            }).ToList();


            return View(model);
        }


        // POST: Student/SelectCourses
        //[OutputCache(NoStore = true, Duration = 0, VaryByParam = "*")]
        [HttpPost]
        public ActionResult SelectCourses(List<CourseSelectionViewModel> selectedCourses)
        {
           // int studentId = (int)(Session["StudentId"] ?? 0);

            int studentId = (int)(Session["StudentId"]);

            if (Session["UserRole"]?.ToString() != "Student"
                || Session["StudentId"] == null
                || (int)Session["StudentId"] != studentId)
            {
                TempData["ErrorMessage"] = "Bu sayfaya erişmek için giriş yapmalısınız.";
                return RedirectToAction("GeneralLogin", "Login");
            }


            var selected = selectedCourses
                .Where(c => c.IsSelected)
                .ToList();

            int totalAkts = selected.Sum(c => c.AKTS);
         
            if (totalAkts > 30)
            {

                TempData["ErrorMessage"] = "Toplam AKTS 30'dan fazla olamaz.";

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
                    IsApprovedByAdvisor = null
                });
            }

            db.SaveChanges();
            return RedirectToAction("Dashboard", "Student", new { StudentId = studentId });
            
        }
        public JsonResult CheckCapacity(int courseId)
        {
            var course = db.Courses.Find(courseId);
            var selectedCount = db.CourseSelections.Count(c => c.CourseId == courseId);
            bool isFull = selectedCount >= course.Kontenjan;
            return Json(new { isFull = isFull }, JsonRequestBehavior.AllowGet);
        }
    }
}