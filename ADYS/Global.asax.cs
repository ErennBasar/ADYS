using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.Web.Optimization;
using System.Web.Routing;

namespace ADYS
{
    public class MvcApplication : System.Web.HttpApplication
    {
        protected void Application_Start()
        {
            AreaRegistration.RegisterAllAreas();
            FilterConfig.RegisterGlobalFilters(GlobalFilters.Filters);
            RouteConfig.RegisterRoutes(RouteTable.Routes);
            BundleConfig.RegisterBundles(BundleTable.Bundles);

            SeedTestData();
        }
        private void SeedTestData()
        {
            using (var db = new ADYS.Data.UniversityContext())
            {
                // Danışman + öğrenci varsa yeniden ekleme
                if (!db.Advisors.Any())
                {
                    var advisor = new ADYS.Models.Advisor
                    {
                        FullName = "Dr. Ahmet Yıldız",
                        Email = "ahmet.yildiz@uni.edu.tr"
                    };

                    db.Advisors.Add(advisor);

                    var student = new ADYS.Models.Student
                    {
                        FullName = "Emircan Turan",
                        Advisor = advisor
                    };

                    db.Students.Add(student);
                }

                // Dersler boşsa örnek dersleri ekle
                if (!db.Courses.Any())
                {
                    var courses = new List<ADYS.Models.Course>
            {
                new ADYS.Models.Course { CourseName = "Yapay Zeka", AKTS = 6 },
                new ADYS.Models.Course { CourseName = "Veri Tabanı Sistemleri", AKTS = 5 },
                new ADYS.Models.Course { CourseName = "Web Programlama", AKTS = 6 },
                new ADYS.Models.Course { CourseName = "Mobil Uygulama Geliştirme", AKTS = 6 },
                new ADYS.Models.Course { CourseName = "Bilgisayar Ağları", AKTS = 5 },
                new ADYS.Models.Course { CourseName = "İşletim Sistemleri", AKTS = 4 },
                new ADYS.Models.Course { CourseName = "Siber Güvenlik", AKTS = 3 },
                new ADYS.Models.Course { CourseName = "Makine Öğrenmesi", AKTS = 5 },
                new ADYS.Models.Course { CourseName = "İnsan Bilgisayar Etkileşimi", AKTS = 2 },
                new ADYS.Models.Course { CourseName = "Nesneye Dayalı Programlama", AKTS = 4 }
            };

                    db.Courses.AddRange(courses);
                }

                db.SaveChanges();
            }
        }

    }
}
