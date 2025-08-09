using ADYS.ViewModels;
using MyApiProject.DTOs;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Threading.Tasks;
using System.Web.Mvc;

namespace ADYS.Controllers
{

    [OutputCache(NoStore = true, Duration = 0, VaryByParam = "*")]
    public class StudentController : Controller
    {
        //private readonly UniversityContext db = new UniversityContext();

        public async Task<ActionResult> Dashboard(int studentId)
        {
            if (Session["UserRole"]?.ToString() != "Student"
                || Session["StudentId"] == null
                || (int)Session["StudentId"] != studentId)
            {
                TempData["ErrorMessage"] = "Bu sayfaya erişmek için giriş yapmalısınız.";
                return RedirectToAction("GeneralLogin", "Login");
            }

            StudentDashboardViewModel dashboard = null;

            using (var client = new HttpClient())
            {
                client.BaseAddress = new Uri("https://localhost:44335"); // API portu
                client.DefaultRequestHeaders.Accept.Clear();
                client.DefaultRequestHeaders.Accept.Add(
                    new MediaTypeWithQualityHeaderValue("application/json"));

                var response = await client.GetAsync($"api/StudentApi/Dashboard?id={studentId}");

                if (response.IsSuccessStatusCode)
                {
                    dashboard = await response.Content.ReadAsAsync<StudentDashboardViewModel>();
                }
                else
                {
                    TempData["ErrorMessage"] = "API'den veri alınamadı: " + response.StatusCode;
                    return RedirectToAction("GeneralLogin", "Login");
                }
            }

            //ViewBag.ActiveTermName = "Güz 2025"; // API'den almak istersen ayrıca endpoint yazılır

            return View(dashboard);
        }

        // GET: Student
        // Öğrenci panelis
        //[OutputCache(NoStore = true, Duration = 0, VaryByParam = "*")]
        //public ActionResult Dashboard(int studentId)
        //{
        //if (Session["UserRole"]?.ToString() != "Student"
        //    || Session["StudentId"] == null
        //    || (int)Session["StudentId"] != studentId)
        //{
        //    TempData["ErrorMessage"] = "Bu sayfaya erişmek için giriş yapmalısınız.";
        //    return RedirectToAction("GeneralLogin", "Login");
        //}

        //var activeTerm = db.Terms.FirstOrDefault(t => t.IsActive);
        //ViewBag.ActiveTermName = activeTerm != null ? activeTerm.TermName : "Aktif dönem bulunmamaktadır.";

        //var student = db.Users
        //                 .Include("UserRoles.Role")
        //              // .Include("Advisor")
        //              // .Include("CourseSelections.Course")
        //                 .FirstOrDefault(u => u.UserId == studentId && 
        //                                 u.UserRoles.Any(ur => ur.Role.RoleName == "Student"));

        //if (student == null) return HttpNotFound();

        //var courseSelections = db.CourseSelections
        //                .Include(cs => cs.Course)
        //                .Where(cs => cs.StudentId == studentId)
        //             //   .AsNoTracking()
        //                .ToList();

        //var advisors = db.AdvisorAssignments
        //        .Include(a => a.Advisor)
        //        .Where(a => a.StudentId == studentId)
        //        .Select(a => a.Advisor)
        //      //  .AsNoTracking()
        //        .FirstOrDefault();

        //var advisorAssignment = db.AdvisorAssignments
        //                        .Include(a => a.Advisor)
        //                        .FirstOrDefault(a => a.StudentId == studentId);


        //var advisor = db.Users.Find(advisorAssignment.AdvisorId); //Find() içine nesne değil, ID değeri verilir. //Ogrenci2 danışmana tanımlı değil o yüzden hata veriyor.

        //var selectedCourses = courseSelections
        //              .Select(cs => new SelectedCourseViewModel
        //              {
        //                  CourseName = cs.Course?.CourseName ?? "-",
        //                  AKTS = cs.Course?.AKTS ?? 0,
        //                  IsApproved = cs.IsApprovedByAdvisor
        //              })
        //              .ToList();

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

        //var model = new StudentDashboardViewModel
        //{
        //    StudentId = student.UserId,
        //    StudentName = student.UserName,
        //    AdvisorName = advisor?.UserName ?? "Danışman atanmamış",
        //    AdvisorEmail = advisor?.Email ?? "-",
        //    SelectedCourses = selectedCourses,
        //    TotalAKTS = selectedCourses.Sum(c => c.AKTS),
        //    AllApproved = selectedCourses.All(c => c.IsApproved == true)
        //};
        //return View(model);
        //    return View();
        //}

        public async Task<ActionResult> SelectCourses(int studentId)
        {
            if (Session["UserRole"]?.ToString() != "Student"
                || Session["StudentId"] == null
                || (int)Session["StudentId"] != studentId)
            {
                TempData["ErrorMessage"] = "Bu sayfaya erişmek için giriş yapmalısınız.";
                return RedirectToAction("GeneralLogin", "Login");
            }
            //if (Session["UserRole"]?.ToString() != "Student" || Session["StudentId"] == null)
            //{
            //    TempData["ErrorMessage"] = "Bu sayfaya erişmek için giriş yapmalısınız.";
            //    return RedirectToAction("GeneralLogin", "Login");
            //}

            // int studentId = (int)Session["StudentId"];
            List<CourseSelectionViewModel> model = new List<CourseSelectionViewModel>();

            using (var client = new HttpClient())
            {
                client.BaseAddress = new Uri("https://localhost:44335");
                client.DefaultRequestHeaders.Accept.Clear();
                client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));

                var response = await client.GetAsync($"api/StudentApi/SelectCourses?studentId={studentId}");
                if (response.IsSuccessStatusCode)
                {
                    model = await response.Content.ReadAsAsync<List<CourseSelectionViewModel>>();
                }
                else if (response.StatusCode == System.Net.HttpStatusCode.BadRequest)
                {
                    TempData["ErrorMessage"] = "Şu anda aktif bir dönem bulunmamaktadır.";
                    return RedirectToAction("Dashboard", "Student", new { studentId });
                }
                else
                {
                    TempData["ErrorMessage"] = "API'den dersler çekilemedi.";
                }
            }

            return View(model);
        }

        // GET: Student/SelectCourses

        //public ActionResult SelectCourses()
        //{
        //    if (Session["UserRole"]?.ToString() != "Student"
        //       || Session["StudentId"] == null)
        //    {
        //        TempData["ErrorMessage"] = "Bu sayfaya erişmek için giriş yapmalısınız.";
        //        return RedirectToAction("GeneralLogin", "Login");
        //    }

        //    int studentId = (int)Session["StudentId"];

        // Aktif dönem kontrolü
        //var activeTerm = db.Terms.FirstOrDefault(t => t.IsActive);

        //if (activeTerm == null)
        //{
        //    // admin aktif dönem ayarlanana kadar RedirectToAction student/selectcourses sonra student/dashboard
        //    TempData["ErrorMessage"] = "Şu anda aktif bir dönem bulunmamaktadır. Ders seçimi yapılamaz.";
        //    return RedirectToAction("Dashboard", "Student", new { studentId });
        //}



        //var selectedCourseIds = db.CourseSelections 
        //    .Where(cs => cs.StudentId == studentId)
        //    .Select(cs => cs.CourseId)
        //    .ToList();

        //var courses = db.Courses.Include(c => c.Advisor).ToList();

        //var model = courses.Select(c => new CourseSelectionViewModel
        //{
        //    CourseId = c.CourseId,
        //    CourseName = c.CourseName,
        //    AKTS = c.AKTS,
        //    StartTime = c.StartTime,
        //    DayOfWeek = c.DayOfWeek,
        //    EndTime = c.EndTime,
        //    IsSelected = selectedCourseIds.Contains(c.CourseId),
        //    AdvisorName = c.Advisor?.UserName ?? "Belirtilmemiş",

        //    // Dersi seçen öğrencilerin sayısı hesaplanıyor
        //    Capacity = c.Kontenjan,
        //    SelectedStudentCount = db.CourseSelections.Count(cs => cs.CourseId == c.CourseId)
        //}).ToList();

        //return View(model);
        //    return View();
        //}

        //private bool HasTimeConflict(List<Course> selectedCourses)
        //{
        //    for (int i = 0; i < selectedCourses.Count; i++)
        //    {
        //        for (int j = i + 1; j < selectedCourses.Count; j++)
        //        {
        //            if (selectedCourses[i].DayOfWeek == selectedCourses[j].DayOfWeek)
        //            {
        //                var aStart = selectedCourses[i].StartTime;
        //                var aEnd = selectedCourses[i].EndTime;
        //                var bStart = selectedCourses[j].StartTime;
        //                var bEnd = selectedCourses[j].EndTime;

        //                if (aStart < bEnd && bStart < aEnd)
        //                {
        //                    return true;
        //                }
        //            }
        //        }
        //    }

        //    return false;
        //}

        // POST: Student/SelectCourses
        // [OutputCache(NoStore = true, Duration = 0, VaryByParam = "*")]
        [HttpPost]
        public async Task<ActionResult> SelectCourses(List<CourseSelectionViewModel> selectedCourses, int studentId)
        {
            // Sadece seçilenleri al
            var selected = selectedCourses
                .Where(c => c.IsSelected)
                .Select(c => new SelectCourseDto
                {
                    CourseId = c.CourseId,
                    StudentId = studentId
                })
                .ToList();

            using (var client = new HttpClient())
            {
                client.BaseAddress = new Uri("https://localhost:44335"); // API portu
                client.DefaultRequestHeaders.Accept.Clear();
                client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));

                var response = await client.PostAsJsonAsync("api/StudentApi/SubmitSelectedCourses", selected);
                if (response.IsSuccessStatusCode)
                {
                    TempData["SuccessMessage"] = "Dersler başarıyla seçildi.";
                    return RedirectToAction("Dashboard", "Student", new { studentId });
                }
                else
                {
                    TempData["ErrorMessage"] = "Ders seçimi başarısız oldu.";
                    return RedirectToAction("SelectCourses", new { studentId });
                }
            }
        }

        // Önce varsa eski seçimleri sil
        //var existingSelections = db.CourseSelections.Where(cs => cs.StudentId == studentId);
        //db.CourseSelections.RemoveRange(existingSelections);

        //// Seçilenleri ekle
        //foreach (var course in selected)
        //{
        //    db.CourseSelections.Add(new CourseSelection
        //    {
        //        StudentId = studentId,
        //        CourseId = course.CourseId,
        //        IsApprovedByAdvisor = null
        //    });
        //}

        //    db.SaveChanges();
        //    return RedirectToAction("Dashboard", "Student", new { StudentId = studentId });

        //}
        //[HttpPost]
        //public ActionResult SelectCourses(int studentId, List<int> courseIds)
        //{
        //    var selectedCourses = db.Courses
        //        .Where(c => courseIds.Contains(c.CourseId))
        //        .ToList();    

        //    // Eski seçimleri sil
        //    var existingSelections = db.CourseSelections.Where(cs => cs.StudentId == studentId);
        //    db.CourseSelections.RemoveRange(existingSelections);

        //    // Yeni seçimleri kaydet
        //    foreach (var courseId in courseIds)
        //    {
        //        db.CourseSelections.Add(new CourseSelection
        //        {
        //            StudentId = studentId,
        //            CourseId = courseId
        //        });
        //    }

        //    db.SaveChanges();

        //    TempData["SuccessMessage"] = "Dersler başarıyla seçildi.";
        //    return RedirectToAction("Dashboard", new { id = studentId });
        //}

        //public JsonResult CheckCapacity(int courseId)
        //{
        //    var course = db.Courses.Find(courseId);
        //    var selectedCount = db.CourseSelections.Count(c => c.CourseId == courseId);
        //    bool isFull = selectedCount >= course.Kontenjan;
        //    return Json(new { isFull = isFull }, JsonRequestBehavior.AllowGet);
        //}
        // }
    }
}