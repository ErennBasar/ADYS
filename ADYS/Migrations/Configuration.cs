namespace ADYS.Migrations
{
    using ADYS.Models;
    using System;
    using System.Collections.Generic;
    using System.Data.Entity;
    using System.Data.Entity.Migrations;
    using System.Linq;

    internal sealed class Configuration : DbMigrationsConfiguration<ADYS.Data.UniversityContext>
    {
        public Configuration()
        {
            AutomaticMigrationsEnabled = false;
        }

        protected override void Seed(ADYS.Data.UniversityContext context)
        {
            //  This method will be called after migrating to the latest version.

            //  You can use the DbSet<T>.AddOrUpdate() helper extension method
            //  to avoid creating duplicate seed data.

            if (!context.Roles.Any())
            {
                context.Roles.AddOrUpdate(
                    new Role { RoleId = 1, RoleName = "Student" },
                    new Role { RoleId = 2, RoleName = "Advisor" },
                    new Role { RoleId = 3, RoleName = "Admin" }
                );
                context.SaveChanges();
            }

            // Kullanıcılar
            if (!context.Users.Any())
            {
                var adminUser_2 = new User
                {
                    UserName = "admin",
                    Email = "admin@uni.edu.tr",
                    Password = "123",
                    IsActive = true,
                    CreateDate = DateTime.Now,

                };
                var advisor_2 = new User
                {
                    UserName = "Advisor",
                    Email = "advisor@uni.edu.tr",
                    Password = "123",
                    IsActive = true,
                    CreateDate = DateTime.Now,
                };

                context.Users.Add(advisor_2);
                context.Users.Add(adminUser_2);

                context.SaveChanges();

                var student_2 = new User
                {
                    UserName = "Ogrenci",
                    Email = "ogrenci@uni.edu.tr",
                    Password = "123",
                    IsActive = true,
                    CreateDate = DateTime.Now,

                };

                context.Users.Add(student_2);

                var studentRole = context.Roles.First(r => r.RoleName == "Student");
                var advisorRole = context.Roles.First(r => r.RoleName == "Advisor");
                var adminRole = context.Roles.First(r => r.RoleName == "Admin");

                if (!context.UserRoles.Any())
                {
                    context.UserRoles.AddOrUpdate(
                        new UserRole { UserId = adminUser_2.UserId, RoleId = adminRole.RoleId },
                        new UserRole { UserId = student_2.UserId, RoleId = studentRole.RoleId },
                        new UserRole { UserId = advisor_2.UserId, RoleId = advisorRole.RoleId }
                    );
                    context.SaveChanges();
                }
            }

            // 3. Bölüm
            if (!context.Departments.Any())
            {
                context.Departments.Add(new Department
                {
                    DepartmentName = "Bilgisayar Mühendisliği"
                });
                context.SaveChanges();
            }

            // 5. Dersler
            if (!context.Courses.Any())
            {
                var firstAdvisor = context.Users
                        .Include("UserRoles.Role")
                        .FirstOrDefault(u => u.UserRoles.Any(ur => ur.Role.RoleName == "Advisor"));

                var firstDepartment = context.Departments.FirstOrDefault();

                if (firstAdvisor != null && firstDepartment != null)
                {
                    var courses = new List<Course>
                {
                    new Course { CourseName = "Yapay Zeka", AKTS = 6, AdvisorId = firstAdvisor.UserId, DepartmentId = firstDepartment.DepartmentId,DayOfWeek = "Pazartesi",StartTime = new TimeSpan(9, 0, 0),EndTime = new TimeSpan(11, 0, 0) },
                    new Course { CourseName = "Veri Tabanı Sistemleri", AKTS = 5, AdvisorId = firstAdvisor.UserId, DepartmentId = firstDepartment.DepartmentId,DayOfWeek = "Pazartesi",StartTime = new TimeSpan(10, 0, 0),EndTime = new TimeSpan(12, 0, 0) },
                    new Course { CourseName = "Web Programlama", AKTS = 6,AdvisorId = firstAdvisor.UserId, DepartmentId = firstDepartment.DepartmentId,DayOfWeek = "Salı",StartTime = new TimeSpan(13, 0, 0),EndTime = new TimeSpan(15, 0, 0) },
                    new Course { CourseName = "Mobil Uygulama Geliştirme", AKTS = 6, AdvisorId = firstAdvisor.UserId, DepartmentId = firstDepartment.DepartmentId,DayOfWeek = "Cuma",StartTime = new TimeSpan(14, 0, 0),EndTime = new TimeSpan(16, 0, 0) },
                    new Course { CourseName = "Bilgisayar Ağları", AKTS = 5, AdvisorId = firstAdvisor.UserId, DepartmentId = firstDepartment.DepartmentId,DayOfWeek = "Çarşamba",StartTime = new TimeSpan(12, 0, 0),EndTime = new TimeSpan(15, 0, 0) },
                    new Course { CourseName = "İşletim Sistemleri", AKTS = 4, AdvisorId = firstAdvisor.UserId, DepartmentId = firstDepartment.DepartmentId,DayOfWeek = "Perşembe",StartTime = new TimeSpan(8, 0, 0),EndTime = new TimeSpan(10, 0, 0) },
                    new Course { CourseName = "Siber Güvenlik", AKTS = 3, AdvisorId = firstAdvisor.UserId, DepartmentId = firstDepartment.DepartmentId,DayOfWeek = "Cuma",StartTime = new TimeSpan(9, 0, 0),EndTime = new TimeSpan(11, 0, 0) },
                    new Course { CourseName = "Makine Öğrenmesi", AKTS = 5, AdvisorId = firstAdvisor.UserId, DepartmentId = firstDepartment.DepartmentId,DayOfWeek = "Salı",StartTime = new TimeSpan(9, 0, 0),EndTime = new TimeSpan(11, 0, 0) },
                    new Course { CourseName = "İnsan Bilgisayar Etkileşimi", AKTS = 2, AdvisorId = firstAdvisor.UserId, DepartmentId = firstDepartment.DepartmentId,DayOfWeek = "Pazartesi",StartTime = new TimeSpan(15, 0, 0),EndTime = new TimeSpan(17, 0, 0) },
                    new Course { CourseName = "Nesneye Dayalı Programlama", AKTS = 4,AdvisorId = firstAdvisor.UserId, DepartmentId = firstDepartment.DepartmentId,DayOfWeek = "Salı",StartTime = new TimeSpan(11, 0, 0),EndTime = new TimeSpan(13, 0, 0) }
                };

                    context.Courses.AddRange(courses);
                    context.SaveChanges();
                }
            }

            if (!context.AdvisorAssignments.Any())
            {
                // Öğrenci ve danışmanları veritabanından al
                var advisor = context.Users.FirstOrDefault(u => u.UserName == "Advisor1");
                var student = context.Users.FirstOrDefault(u => u.UserName == "Ogrenci1");

                if (advisor != null && student != null)
                {
                    context.AdvisorAssignments.Add(new AdvisorAssignment
                    {
                        AdvisorId = advisor.UserId,
                        StudentId = student.UserId
                    });
                }
            }


            // 6. Dönemler
            if (!context.Terms.Any())
            {
                var terms = new List<Term>
            {
                new Term
                {
                    TermName = "2025 Güz",
                    IsActive = true,
                    CourseSelectionStart = new DateTime(2025, 9, 1),
                    CourseSelectionEnd = new DateTime(2025, 9, 15)
                },
                new Term
                {
                    TermName = "2026 Bahar",
                    IsActive = false,
                    CourseSelectionStart = new DateTime(2026, 2, 1),
                    CourseSelectionEnd = new DateTime(2026, 2, 15)
                }
            };

                context.Terms.AddRange(terms);
                context.SaveChanges();
            }
        }
    }
}

