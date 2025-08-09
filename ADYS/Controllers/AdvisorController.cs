using ADYS.Filters;
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
    public class AdvisorController : Controller
    {
        //private readonly UniversityContext db = new UniversityContext();

        // Danışman paneli (öğrenci listesi)

        //[Authorize(Roles = "Advisor")]
        [NoCache]
        public async Task<ActionResult> Dashboard(int advisorId)
        {
            // Eski query-string geldiyse kendini temiz URL’ye yönlendir
            //if (!string.IsNullOrEmpty(Request.QueryString["advisorId"]))
            //    return RedirectToAction("Students"); // canonical URL

           // var advisorId = Session["AdvisorId"] as int?;
            if (Session["UserRole"]?.ToString() != "Advisor"
                || Session["AdvisorId"] == null
                || (int)Session["AdvisorId"] != advisorId)
            //if (advisorId == 0) return RedirectToAction("GeneralLogin", "Login");
            {
                TempData["ErrorMessage"] = "Bu sayfaya erişmek için giriş yapmalısınız.";
                return RedirectToAction("GeneralLogin", "Login");
            }
            //int advisorId = (int)(Session["AdvisorId"] ?? 0);
            //if (advisorId == 0) return RedirectToAction("GeneralLogin", "Login");

            AdvisorDashboardViewModel model = null;

            using (var client = new HttpClient())
            {
                client.BaseAddress = new Uri("https://localhost:44335"); // API portun
                client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));

                var response = await client.GetAsync($"api/AdvisorApi/Dashboard?advisorId={advisorId}");
                if (response.IsSuccessStatusCode)
                {
                    var dto = await response.Content.ReadAsAsync<AdvisorDashboardDto>();

                    model = new AdvisorDashboardViewModel
                    {
                        AdvisorName = dto.AdvisorName,
                        Students = dto.Students.Select(s => new StudentViewModel
                        {
                            StudentId = s.StudentId,
                            StudentName = s.StudentName,
                            Email = s.Email
                        }).ToList()
                    };
                }
                else
                {
                    TempData["ErrorMessage"] = "API'den danışman verisi alınamadı.";
                    return RedirectToAction("GeneralLogin", "Login");
                }
            }

            return View(model);
        }
        //public ActionResult Dashboard()
        //{
        //    int advisorId = (int)(Session["AdvisorId"] ?? 0);

        //    if (advisorId == 0)
        //        return RedirectToAction("Advisor", "Login");

        //    var advisor = db.Users.Find(advisorId);
        //    ViewBag.AdvisorName = advisor?.UserName ?? "Danışman";

        //    // danışmana ait öğrenciler
        //    var studentList = db.AdvisorAssignments
        //        .Where(a => a.AdvisorId == advisorId)
        //        .Select(a => a.Student)
        //        .ToList();

        //    return View(studentList);
        //}

        //Ders seçimi yapmış öğrenciler 
        public async Task<ActionResult> Students(int advisorId)
        {
            if (Session["UserRole"]?.ToString() != "Advisor"
                || Session["AdvisorId"] == null
                || (int)Session["AdvisorId"] != advisorId)
            {
                TempData["ErrorMessage"] = "Bu sayfaya erişmek için giriş yapmalısınız.";
                return RedirectToAction("GeneralLogin", "Login");
            }
            //int advisorId = (int)(Session["AdvisorId"] ?? 0);
            //if (advisorId == 0)
            //    return RedirectToAction("GeneralLogin", "Login");

            List<StudentSelectCoursesDto> students = null;


            using (var client = new HttpClient())
            {
                client.BaseAddress = new Uri("https://localhost:44335");
                var response = await client.GetAsync($"api/AdvisorApi/Students?advisorId={advisorId}");

                if (response.IsSuccessStatusCode)
                {
                    students = await response.Content.ReadAsAsync<List<StudentSelectCoursesDto>>();
                }
                else
                {
                    TempData["ErrorMessage"] = "Öğrenciler API'den getirilemedi.";
                    return RedirectToAction("Dashboard");
                }
            }

            return View(students);
        }
        // 2. Ders seçimi yapmış öğrenciler (filtreli)
        //public ActionResult Students()
        //{
        //    int advisorId = (int)(Session["AdvisorId"] ?? 0);

        //    if (advisorId == 0)
        //        return RedirectToAction("Advisor", "Login");

        //    var advisor = db.Users.Find(advisorId);
        //    ViewBag.AdvisorName = advisor?.UserName ?? "Danışman";


        //    // danışmana ait öğrenciler
        //    var studentList = db.AdvisorAssignments
        //        .Where(a => a.AdvisorId == advisorId)
        //        .Select(a => a.Student)
        //        .Where(s => db.CourseSelections.Any(cs => cs.StudentId == s.UserId))
        //        .ToList();

        //    return View(studentList);
        //}
        public async Task<ActionResult> Review(int studentId)
        {
            //int advisorId = (int)(Session["AdvisorId"] ?? 0);
            //if (advisorId == 0) return RedirectToAction("GeneralLogin", "Login");     

            using (var client = new HttpClient())
            {
                client.BaseAddress = new Uri("https://localhost:44335"); // API base
                var response = await client.GetAsync($"api/AdvisorApi/Review?studentId={studentId}");

                if (response.IsSuccessStatusCode)
                {
                    var studentData = await response.Content.ReadAsAsync<ReviewStudentCoursesDto>();
                    ViewBag.StudentName = studentData.StudentName;
                    ViewBag.StudentId = studentData.StudentId;
                    return View(studentData.Courses);
                }
                else
                {
                    TempData["ErrorMessage"] = "Öğrenciye ait dersler getirilemedi.";
                    return RedirectToAction("Dashboard");
                }
            }
        }
        //        // 3. Onay paneli: öğrencinin seçtiği dersler
        //        public ActionResult Review(int studentId)
        //        {
        //            int advisorId = (int)(Session["AdvisorId"] ?? 0);

        //            var assignment = db.AdvisorAssignments
        //                       .FirstOrDefault(a => a.StudentId == studentId && a.AdvisorId == advisorId);

        //            var student = db.Users
        //                   .Include("CourseSelections.Course")
        //                  .FirstOrDefault(s => s.UserId == studentId);

        //            if (student == null) return HttpNotFound();

        //            var selections = db.CourseSelections
        //                .Include("Course")
        //                .Where(cs => cs.StudentId == studentId)
        //                .ToList();

        //            var model = selections.Select(cs => new ReviewCourseViewModel
        //            {
        //                CourseSelectionId = cs.CourseSelectionId,
        //                CourseName = cs.Course.CourseName,
        //                AKTS = cs.Course.AKTS,
        //                IsApproved = cs.IsApprovedByAdvisor
        //            }).ToList();


        //            ViewBag.StudentName = student.UserName;
        //            ViewBag.StudentId = studentId;

        //            return View(model);
        //        }

        // 4. AJAX işlem: onaylama/red
        //[HttpPost]
        //public JsonResult UpdateApproval(int courseSelectionId, bool approve)
        //{
        //    var selection = db.CourseSelections.Find(courseSelectionId);
        //    if (selection == null) return Json(new { success = false });

        //    selection.IsApprovedByAdvisor = approve;
        //    db.SaveChanges();

        //    return Json(new { success = true });
        //}
    }

}