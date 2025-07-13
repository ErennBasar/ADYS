using ADYS.Data;
using ADYS.ViewModels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace ADYS.Controllers
{
    public class LoginController : Controller
    {
        private readonly UniversityContext db = new UniversityContext();

        // GET: Login
        public ActionResult Index()
        {
            return View();
        }
        // GET: Login/Student
        public ActionResult Student()
        {
            if (Session["UserRole"] != null)
            {
                ViewBag.Message = "Giriş yapmak için önce mevcut oturumdan çıkış yapınız.";
                return View("AlreadyLoggedIn"); // Yeni bir View açacağız.
            }
            return View(new LoginViewModel());
        }

        // POST: Login/Student
        [HttpPost]
        public ActionResult Student(LoginViewModel model)
        {
            if (!ModelState.IsValid)
            {
                return View(model);
            }

            var student = db.Students.FirstOrDefault(s => s.Email == model.Email && s.Password == model.Password);
            if (student == null)
            {
                ModelState.AddModelError("", "Geçersiz e-posta veya şifre.");
                return View(model);
            }

            Session["StudentId"] = student.StudentId;
            Session["StudentLoginTime"] = DateTime.Now;
            Session["UserRole"] = "Student";
            return RedirectToAction("Dashboard", "Student", new { studentId = student.StudentId });

        }



        // GET: Login/Advisor
        public ActionResult Advisor()
        {
            if (Session["UserRole"] != null)
            {
                ViewBag.Message = "Giriş yapmak için önce mevcut oturumdan çıkış yapınız.";
                return View("AlreadyLoggedIn");
            }
            return View(new LoginViewModel());
        }

        // POST: Login/Advisor
        [HttpPost]
        public ActionResult Advisor(LoginViewModel model)
        {
            if (!ModelState.IsValid) return View(model);

            var advisor = db.Advisors.FirstOrDefault(a => a.Email == model.Email);
            if (advisor == null)
            {
                ModelState.AddModelError("", "Danışman bulunamadı.");
                return View(model);
            }

            Session["AdvisorId"] = advisor.AdvisorId;
            Session["AdvisorLoginTime"] = DateTime.Now;
            Session["UserRole"] = "Advisor";
            return RedirectToAction("Dashboard", "Advisor", new {advisorId = advisor.AdvisorId});
        }


        // GET: Login/Admin
        public ActionResult Admin()
        {
            if (Session["UserRole"] != null)
            {
                ViewBag.Message = "Giriş yapmak için önce mevcut oturumdan çıkış yapınız.";
                return View("AlreadyLoggedIn");
            }
            return View(new LoginViewModel());
        }

        // POST: Login/Admin
        [HttpPost]
        public ActionResult Admin(LoginViewModel model)
        {
            if (model.Email == "admin" && model.Password == "admin123")
            {
                Session["UserRole"] = "Admin";
                Session["AdminLoginTime"] = DateTime.Now;
                return RedirectToAction("Dashboard", "Admin");
            }

            ModelState.AddModelError("", "Geçersiz admin giriş bilgisi.");
            return View(model);
        }
        public ActionResult Logout()
        {
            TempData["LogoutMessage"] = "Başarıyla çıkış yaptınız.";
            
            Session.Clear(); // Tüm oturum verilerini temizler
            //Session.Abandon(); // Oturumu sonlandırır (güvenlik için)

            Response.Cache.SetExpires(DateTime.UtcNow.AddDays(-1));
            Response.Cache.SetValidUntilExpires(false);
            Response.Cache.SetRevalidation(HttpCacheRevalidation.AllCaches);
            Response.Cache.SetCacheability(HttpCacheability.NoCache);
            Response.Cache.SetNoStore();

            return RedirectToAction("Index", "Login"); 
        }


    }
}
