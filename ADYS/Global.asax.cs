using ADYS.Models;
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
                // 1. Bölümler yoksa bir tane ekle
                if (!db.Departments.Any())
                {
                    db.Departments.Add(new Department
                    {
                        DepartmentName = "Bilgisayar Mühendisliği"
                    });
                    db.SaveChanges(); // önce kaydet ki id oluşsun
                }
                // Danışmanlar yoksa oluştur
                if (!db.Advisors.Any())
                {
                    var advisors = new List<Advisor>
                    {
                        new Advisor { FullName = "Dr. Ahmet Yıldız", Email = "ahmet@uni.edu.tr" },
                        new Advisor { FullName = "Dr. Merve Güneş", Email = "merve@uni.edu.tr" },
                        new Advisor { FullName = "Dr. Ali Yılmaz", Email = "ali@uni.edu.tr" }
                    };

                    db.Advisors.AddRange(advisors);
                    db.SaveChanges();
                }

                // Öğrenciler yoksa oluştur
                if (!db.Students.Any())
                {
                    var allAdvisors = db.Advisors.ToList();
                    int studentCounter = 1;

                    foreach (var advisor in allAdvisors)
                    {
                        for (int i = 1; i <= 10; i++)
                        {
                            db.Students.Add(new Student
                            {
                                FullName = $"Öğrenci {studentCounter}",
                                Email = $"student{studentCounter}@ogrenci.edu.tr",
                                Password = "1234",
                                AdvisorId = advisor.AdvisorId
                            });

                            studentCounter++;
                        }
                    }
                    db.SaveChanges();
                }

                // Dersler yoksa oluştur
                if (!db.Courses.Any())
                {
                    var firstAdvisor = db.Advisors.First(); // herhangi bir danışmanı seç
                    var firstDepartment = db.Departments.FirstOrDefault();


                    if (firstAdvisor != null && firstDepartment != null)
                    {
                        var courses = new List<Course>
                        {
                            new Course { CourseName = "Yapay Zeka", AKTS = 6, AdvisorId = firstAdvisor.AdvisorId, DepartmentId = firstDepartment.DepartmentId },
                            new Course { CourseName = "Veri Tabanı Sistemleri", AKTS = 5, AdvisorId = firstAdvisor.AdvisorId, DepartmentId = firstDepartment.DepartmentId },
                            new Course { CourseName = "Web Programlama", AKTS = 6, AdvisorId = firstAdvisor.AdvisorId, DepartmentId = firstDepartment.DepartmentId },
                            new Course { CourseName = "Mobil Uygulama Geliştirme", AKTS = 6, AdvisorId = firstAdvisor.AdvisorId, DepartmentId = firstDepartment.DepartmentId },
                            new Course { CourseName = "Bilgisayar Ağları", AKTS = 5, AdvisorId = firstAdvisor.AdvisorId, DepartmentId = firstDepartment.DepartmentId },
                            new Course { CourseName = "İşletim Sistemleri", AKTS = 4, AdvisorId = firstAdvisor.AdvisorId, DepartmentId = firstDepartment.DepartmentId },
                            new Course { CourseName = "Siber Güvenlik", AKTS = 3, AdvisorId = firstAdvisor.AdvisorId, DepartmentId = firstDepartment.DepartmentId },
                            new Course { CourseName = "Makine Öğrenmesi", AKTS = 5, AdvisorId = firstAdvisor.AdvisorId, DepartmentId = firstDepartment.DepartmentId },
                            new Course { CourseName = "İnsan Bilgisayar Etkileşimi", AKTS = 2, AdvisorId = firstAdvisor.AdvisorId, DepartmentId = firstDepartment.DepartmentId },
                            new Course { CourseName = "Nesneye Dayalı Programlama", AKTS = 4, AdvisorId = firstAdvisor.AdvisorId, DepartmentId = firstDepartment.DepartmentId }
                        };

                        db.Courses.AddRange(courses);
                        db.SaveChanges();
                    }

                }

            }

        }
    }
}
