using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Threading.Tasks;
using System.Web;
using System.Web.Mvc;
using System.Net.Http.Formatting;
using ADYS.ViewModels;
using System.Net.Http.Headers;

namespace ADYS.Controllers
{
    public class UserController : Controller
    {
        // GET: User
        private readonly string apiBaseUrl = "https://localhost:44335"; // API'nin doğru portu

        public async Task<ActionResult> List()
        {
            List<UserViewModel> users = new List<UserViewModel>();

            using (HttpClient client = new HttpClient())
            {

                client.BaseAddress = new System.Uri(apiBaseUrl);

                // Accept header'ı JSON olarak ayarlandı
                client.DefaultRequestHeaders.Accept.Clear();
                client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));

                try
                {
                    var response = await client.GetAsync("api/users/GetAllUsers"); // Eğer API'de Route yoksa bu şekilde olmalı

                    if (response.IsSuccessStatusCode)
                    {
                        users = await response.Content.ReadAsAsync<List<UserViewModel>>();
                    }
                    else
                    {
                        TempData["ErrorMessage"] = "API'den veri alınamadı. Hata: " + response.StatusCode;
                    }
                }
                catch (Exception ex)
                {
                    TempData["ErrorMessage"] = "API bağlantı hatası: " + ex.Message;
                }
            }

            // Hata mesajı ViewBag'e yazılır
            if (users == null || !users.Any())
            {
                ViewBag.Error = "Kullanıcı listesi boş.";
            }

            return View(users);
        }
    }
}