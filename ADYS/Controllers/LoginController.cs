using ADYS.Data;
using ADYS.ViewModels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Threading.Tasks;
using System.Web;
using System.Web.Mvc;
using System.Net.Http.Headers;
using System.Net.Http.Formatting;

namespace ADYS.Controllers
{

    [OutputCache(NoStore = true, Duration = 0, VaryByParam = "*")]
    public class LoginController : Controller
    {
        //private readonly UniversityContext db = new UniversityContext();
        //GET: GeneralLogin
        public ActionResult GeneralLogin()
        {
            if (Session["UserRole"] != null)
            {
                ViewBag.Message = "Giriş yapmak için önce mevcut oturumdan çıkış yapınız.";
                return View("AlreadyLoggedIn"); // Yeni bir View açacağız.
                //return View("GeneralLogin");
            }

            return View();
        }

        
        [HttpPost]
        public async Task<ActionResult> GeneralLogin(LoginViewModel model)
        {
            if (!ModelState.IsValid)
                return View(model);

            using (var client = new HttpClient())
            {
                client.BaseAddress = new Uri("https://localhost:44335"); // API portun

                var response = await client.PostAsJsonAsync("api/LoginApi/Authenticate", model);
                if (response.IsSuccessStatusCode)
                {
                    var user = await response.Content.ReadAsAsync<AuthenticatedUserViewModel>();

                    Session["UserRole"] = user.Role;

                    if (user.Role == "Student")
                    {
                        Session["StudentId"] = user.UserId;
                        return RedirectToAction("Dashboard", "Student", new { studentId = user.UserId });
                    }
                    else if (user.Role == "Advisor")
                    {
                        Session["AdvisorId"] = user.UserId;
                        return RedirectToAction("Dashboard", "Advisor", new { advisorId = user.UserId });
                    }
                    else if (user.Role == "Admin")
                    {
                        Session["AdminId"] = user.UserId;
                        return RedirectToAction("Dashboard", "Admin", new {adminId = user.UserId});
                    }
                }
                else
                {
                    ModelState.AddModelError("", "Geçersiz e-posta veya şifre.");
                }
            }

            return View(model);
        }



        // POST: GeneralLogin
        // [HttpPost]
        //public ActionResult GeneralLogin(LoginViewModel model)
        //{
        //    if (!ModelState.IsValid)
        //    {
        //        return View(model);
        //    }
        //var user = db.Users
        //    .Include("UserRoles.Role")
        //    .FirstOrDefault(s => s.Email == model.Email && s.Password == model.Password);

        //if (user == null)
        //{
        //    ModelState.AddModelError("", "Geçersiz e-posta veya şifre.");
        //    return View(model);
        //}

        //var userRole = user.UserRoles.FirstOrDefault();
        //var roleName = userRole.Role.RoleName;

        //Session["UserRole"] = roleName;

        //if (roleName == "Student")
        //{
        //    Session["StudentId"] = user.UserId;
        //    return RedirectToAction("Dashboard", "Student", new { studentId = user.UserId });
        //}
        //else if (roleName == "Advisor")
        //{
        //    Session["AdvisorId"] = user.UserId;
        //    return RedirectToAction("Dashboard", "Advisor", new { advisorId = user.UserId });
        //}
        //else if (roleName == "Admin")
        //{
        //    return RedirectToAction("Dashboard", "Admin");
        //}
        //else
        //{
        //    Session.Clear();
        //    return RedirectToAction("GeneralLogin","Login");
        //}
        // }  
        public ActionResult Logout()
        {
            TempData["LogoutMessage"] = "Başarıyla Çıkış Yaptınız";

            Session.RemoveAll();
            Session.Abandon();
            Response.Cookies.Clear();

            return RedirectToAction("GeneralLogin", "Login");
        }
    }
}
