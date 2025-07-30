using ADYS.Data;
using ADYS.Models;
using ADYS.ViewModels;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Data.Entity;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using ADYS.Helpers;

namespace ADYS.Controllers
{
    public class AdminController : Controller
    {
        private readonly UniversityContext db = new UniversityContext();

        // Genel Dashboard
        public ActionResult Dashboard()
        {
            if (Session["UserRole"] == null || Session["UserRole"].ToString() != "Admin")
            {
                TempData["ErrorMessage"] = "Bu sayfaya erişmek için giriş yapmalısınız.";
                return RedirectToAction("GeneralLogin", "Login");
            }

            // Öğrenci ve danışman bilgilerini AdvisorAssignment üzerinden al
            var studentsWithAdvisor = db.AdvisorAssignments
                    .Select(a => new StudentDashboardViewModel
                    {
                        StudentId = a.Student.UserId,
                        StudentName = a.Student.UserName,
                        StudentEmail = a.Student.Email,
                        AdvisorName = a.Advisor.UserName,
                        AdvisorEmail = a.Advisor.Email
                    })
                    .ToList();                      

            return View(studentsWithAdvisor);
        }

        // === Öğrenci Tanımlamaları ===
        public ActionResult ManageStudents()
        {
            if (Session["UserRole"] == null || Session["UserRole"].ToString() != "Admin")
            {
                TempData["ErrorMessage"] = "Bu sayfaya erişmek için giriş yapmalısınız.";
                return RedirectToAction("GeneralLogin", "Login");
            }

            // Öğrenci ve danışman bilgilerini AdvisorAssignment üzerinden al
            var studentsWithAdvisor = db.AdvisorAssignments
                    .Select(a => new StudentDashboardViewModel
                    {
                        StudentId = a.Student.UserId,
                        StudentName = a.Student.UserName,
                        StudentEmail = a.Student.Email,
                        AdvisorName = a.Advisor.UserName,
                        AdvisorEmail = a.Advisor.Email
                    })
                    .ToList();

            ViewBag.Advisors = new SelectList(
                    db.Users.Where(u => u.UserRoles.Any(ur => ur.Role.RoleName == "Advisor")),
                    "UserId",
                    "UserName"
            );

            return View(studentsWithAdvisor);
        }

        //[HttpPost]
        //public ActionResult AddStudent(string FullName, string Email, string Password, int AdvisorId)
        //{
        //    if (Session["UserRole"] == null || Session["UserRole"].ToString() != "Admin")
        //    {
        //        TempData["ErrorMessage"] = "Bu sayfaya erişmek için giriş yapmalısınız.";
        //        return RedirectToAction("GeneralLogin", "Login");
        //    }

        //    var student = new User
        //    {
        //        UserName = FullName,
        //        Email = Email,

        //        Password = Password
        //    };

        //    db.Users.Add(student);
        //    db.SaveChanges();

        //    var studentRole = db.Roles.FirstOrDefault(r => r.RoleName == "Student");

        //    if (studentRole != null)
        //    {
        //        db.UserRoles.Add(new UserRole
        //        {
        //            UserId = student.UserId,
        //            RoleId = studentRole.RoleId
        //        });
        //    }

        //    // AdvisorAssignments tablosuna eşleştirme 
        //    db.AdvisorAssignments.Add(new AdvisorAssignment
        //    {
        //        StudentId = student.UserId,
        //        AdvisorId = AdvisorId
        //    });

        //    db.SaveChanges();

        //    return RedirectToAction("ManageStudents");

        //}
        [HttpPost]
        public ActionResult AddOrUpdateStudent(int? StudentId, string FullName, string Email, string Password, int AdvisorId)
        {
            if (StudentId.HasValue)
            {
                // GÜNCELLEME
                var existing = db.Users.FirstOrDefault(x => x.UserId == StudentId.Value);
                if (existing != null)
                {
                    existing.UserName = FullName;
                    existing.Email = Email;

                    if (!string.IsNullOrWhiteSpace(Password))
                        existing.Password = Password;

                    // UpdateDate güncelle
                    existing.UpdateDate = DateTime.Now;

                    // AdvisorId güncellemesi AdvisorAssignments tablosunda yapılmalı
                    var assignment = db.AdvisorAssignments.FirstOrDefault(a => a.StudentId == existing.UserId);
                    if (assignment != null)
                    {
                        assignment.AdvisorId = AdvisorId;
                    }
                    else
                    {
                        // Eğer öğrenciye ait eşleştirme yoksa yeni oluştur
                        db.AdvisorAssignments.Add(new AdvisorAssignment
                        {
                            StudentId = existing.UserId,
                            //AdvisorId = AdvisorId
                        });
                    }

                    TempData["Success"] = "Öğrenci bilgileri güncellendi.";
                }
            }
            else
            {
                // EKLEME
                var student = new User
                {
                    UserName = FullName,
                    Email = Email,
                    Password = Password,
                    CreateDate = DateTime.Now,    
                    IsActive = true

                };

                db.Users.Add(student);
                db.SaveChanges();

                var studentRole = db.Roles.FirstOrDefault(r => r.RoleName == "Student");
                if (studentRole != null)
                {
                    db.UserRoles.Add(new UserRole
                    {
                        UserId = student.UserId,
                        RoleId = studentRole.RoleId
                    });
                }
                // AdvisorAssignments tablosuna eşleştirme
                db.AdvisorAssignments.Add(new AdvisorAssignment
                {
                    StudentId = student.UserId,
                    AdvisorId = AdvisorId
                });

                db.SaveChanges();

                TempData["Success"] = "Yeni öğrenci eklendi.";
            }

                db.SaveChanges();
                return RedirectToAction("ManageStudents");
        }
        

        public ActionResult DeleteStudent(int id)
        {

            var student = db.Users
                .Include(u => u.CourseSelections)
                .Include(u => u.AdvisorAssignments)
                .Include(u => u.UserRoles)
                .FirstOrDefault(u => u.UserId == id);


            if (student != null)
            {
                db.CourseSelections.RemoveRange(student.CourseSelections);
                db.AdvisorAssignments.RemoveRange(student.AdvisorAssignments);
                db.UserRoles.RemoveRange(student.UserRoles);

                student.IsDeleted = true;
                student.IsActive = false;
                student.DeleteDate = DateTime.Now;

                db.SaveChanges();
            }
            return RedirectToAction("ManageStudents");
        }


        // === Danışman Tanımlamaları ===
        // Liste
        public ActionResult ManageAdvisors()
        {
            if (Session["UserRole"] == null || Session["UserRole"].ToString() != "Admin")
            {
                TempData["ErrorMessage"] = "Bu sayfaya erişmek için giriş yapmalısınız.";
                return RedirectToAction("GeneralLogin", "Login");
            }        

            var advisors = db.Users
                .Where(u => u.UserRoles.Any(ur => ur.Role.RoleName == "Advisor"))
                .ToList();
          

            return View(advisors);
        }

        //        // Ekle / Güncelle (GET)
        //        public ActionResult CreateAdvisor(int? id)
        //        {
        //            if (Session["UserRole"] == null || Session["UserRole"].ToString() != "Admin")
        //            {
        //                TempData["ErrorMessage"] = "Bu sayfaya erişmek için giriş yapmalısınız.";
        //                return RedirectToAction("Index", "Login");
        //            }
        //            if (id == null)
        //                return View(new Advisor()); // Yeni kayıt
        //            var advisor = db.Advisors.Find(id);
        //            if (advisor == null)
        //                return HttpNotFound();
        //            return View(advisor); // Güncelleme
        //        }

        //        // Ekle / Güncelle (POST)
        //[HttpPost]
        //public ActionResult CreateAdvisor(string FullName, string Email, string Password)
        //{
        //    var advisor = new User
        //    {
        //        UserName = FullName,
        //        Email = Email,
        //        Password = Password
        //    };

        //    db.Users.Add(advisor);
        //    db.SaveChanges();

        //    var advisorRole = db.Roles.FirstOrDefault(r => r.RoleName == "Advisor");

        //    if (advisorRole != null)
        //    {
        //        db.UserRoles.Add(new UserRole
        //        {
        //            UserId = advisor.UserId,
        //            RoleId = advisorRole.RoleId
        //        });
        //    }

        //    db.SaveChanges();
        //    TempData["SuccessMessage"] = "Danışman başarıyla eklendi.";

        //    return RedirectToAction("ManageAdvisors");
        //}

        [HttpPost]
        public ActionResult CreateOrUpdateAdvisor(int? AdvisorId, string UserName, string Email, string Password)
        {
            if (AdvisorId.HasValue)
            {
                var existing = db.Users.FirstOrDefault(x => x.UserId == AdvisorId);
                if (existing != null)
                {
                    existing.UserName = UserName;
                    existing.Email = Email;
                    if (!string.IsNullOrWhiteSpace(Password))
                        existing.Password = Password;
                    TempData["Success"] = "Danışman güncellendi.";
                }
            }
            else
            {
                var advisor = new User
                {
                    UserName = UserName,
                    Email = Email,
                    Password = Password,
                    CreateDate = DateTime.Now,
                    IsActive = true
                };
                db.Users.Add(advisor);
                db.SaveChanges();

                var advisorRole = db.Roles.FirstOrDefault(r => r.RoleName == "Advisor");

                if (advisorRole != null)
                {
                    db.UserRoles.Add(new UserRole
                    {
                        UserId = advisor.UserId,
                        RoleId = advisorRole.RoleId
                    });
                    db.SaveChanges();
                }

                TempData["Success"] = "Yeni danışman eklendi.";
            }
            db.SaveChanges();
            return RedirectToAction("ManageAdvisors");
        }

        [HttpPost]
        public ActionResult DeleteAdvisor(int id)
        {
            var advisor = db.Users.FirstOrDefault(u => u.UserId == id);
            if (advisor == null)
            {
                return Json(new
                {
                    success = false,
                    message = "Danışman bulunamadı."
                }, JsonRequestBehavior.AllowGet);
            }

            // Soft delete işlemleri
            advisor.IsActive = false;
            advisor.IsDeleted = true;
            advisor.DeleteDate = DateTime.Now;

            db.SaveChanges();

            return Json(new
            {
                success = true,
                message = "Danışman başarıyla pasif hale getirildi."
            }, JsonRequestBehavior.AllowGet);
        }





        //        // Silme işlemi
        //        public ActionResult DeleteAdvisor(int id)
        //        {
        //            var advisor = db.Advisors.Find(id);
        //            if (advisor == null)
        //                return HttpNotFound();

        //            db.Advisors.Remove(advisor);
        //            db.SaveChanges();
        //            return RedirectToAction("ManageAdvisors");
        //        }
        //        // GET: Admin/ManageCourses
        public ActionResult ManageCourses()
        {
            if (Session["UserRole"] == null || Session["UserRole"].ToString() != "Admin")
            {
                TempData["ErrorMessage"] = "Bu sayfaya erişmek için giriş yapmalısınız.";
                return RedirectToAction("GeneralLogin", "Login");
            }

            // Danışman ve bölüm bilgilerini içeren kursları listele
            var courses = db.Courses
                .Include(c => c.Advisor)
                .Include(c => c.Department)
                .ToList();

            // Danışmanları sadece "Advisor" rolüne sahip kullanıcılar arasından seç
            var advisors = db.Users
                .Where(u => u.UserRoles.Any(r => r.Role.RoleName == "Advisor"))
                .ToList();

            ViewBag.Advisors = new SelectList(advisors, "UserId", "UserName");

            ViewBag.Departments = new SelectList(db.Departments.ToList(), "DepartmentId", "DepartmentName");


            return View(courses);
        }

        //// GET: Admin/CreateCourse
        //public ActionResult CreateCourse()
        //{
        //    if (Session["UserRole"] == null || Session["UserRole"].ToString() != "Admin")
        //    {
        //        TempData["ErrorMessage"] = "Bu sayfaya erişmek için giriş yapmalısınız.";
        //        return RedirectToAction("GeneralLogin", "Login");
        //    }

        //    var advisors = db.Users
        //        .Include(u => u.UserRoles.Select(ur => ur.Role))
        //        .Where(u => u.UserRoles.Any(ur => ur.Role.RoleName == "Advisor"))
        //        .ToList();

        //    var departments = db.Departments;

        //    ViewBag.Advisors = new SelectList(advisors, "UserId", "UserName");
        //    ViewBag.Departments = new SelectList(departments, "DepartmentId", "DepartmentName");

        //    return View("ManageCourses");
        //}

        [HttpPost]
        public ActionResult CreateOrUpdateCourse(int? CourseId, string CourseName, string DayOfWeek, TimeSpan StartTime, TimeSpan EndTime, int AKTS, int Kontenjan, int AdvisorId, int DepartmentId)
        {
            Course course;

            if (CourseId.HasValue && CourseId.Value > 0)
            {
                course = db.Courses.Find(CourseId.Value);
                if (course == null)
                {
                    TempData["ErrorMessage"] = "Ders bulunamadı.";
                    return RedirectToAction("ManageCourses");
                }

                // Güncelleme
                course.CourseName = CourseName;
                course.DayOfWeek = DayOfWeek;
                course.StartTime = StartTime;
                course.EndTime = EndTime;
                course.AKTS = AKTS;
                course.Kontenjan = Kontenjan;
                course.AdvisorId = AdvisorId;
                course.DepartmentId = DepartmentId;

                TempData["SuccessMessage"] = "Ders başarıyla güncellendi.";
            }
            else
            {
                // Yeni ders ekleme
                course = new Course
                {
                    CourseName = CourseName,
                    DayOfWeek = DayOfWeek,
                    StartTime = StartTime,
                    EndTime = EndTime,
                    AKTS = AKTS,
                    Kontenjan = Kontenjan,
                    AdvisorId = AdvisorId,
                    DepartmentId = DepartmentId
                };

                db.Courses.Add(course);
                TempData["SuccessMessage"] = "Yeni ders başarıyla eklendi.";
            }

            db.SaveChanges();
            return RedirectToAction("ManageCourses");
        }

        [HttpPost]
        public JsonResult DeleteCourse(int id)
        {
            var course = db.Courses.Find(id);
            if (course != null)
            {
                db.Courses.Remove(course);
                db.SaveChanges();
                return Json(new { success = true, message = "Ders başarıyla silindi." });
            }
            return Json(new { success = false, message = "Ders bulunamadı." });
        }



        // POST: Admin/CreateCourse
        //[HttpPost]
        //public ActionResult CreateCourse(Course course)
        //{
        //    if (ModelState.IsValid)
        //    {
        //        bool isAdvisorValid = db.Users
        //            .Any(u => u.UserId == course.AdvisorId && 
        //                      u.UserRoles.Any(ur => ur.Role.RoleName == "Advisor"));

        //        if (!isAdvisorValid)
        //        {
        //            ModelState.AddModelError("", "Geçerli bir danışman seçmelisiniz.");
        //            LoadDropdowns(course);
        //            return View(course);
        //        }

        //        db.Courses.Add(course);
        //        db.SaveChanges();
        //        return RedirectToAction("ManageCourses");
        //        //// Danışman ID geçerli mi kontrol 
        //        //if (!db.Advisors.Any(a => a.AdvisorId == course.AdvisorId))
        //        //{
        //        //    ModelState.AddModelError("", "Geçerli bir danışman seçmelisiniz.");
        //        //    // ViewBag'leri yeniden doldur
        //        //    ViewBag.Advisors = new SelectList(db.Advisors, "AdvisorId", "FullName", course.AdvisorId);
        //        //    ViewBag.Departments = new SelectList(db.Departments, "DepartmentId", "DepartmentName", course.DepartmentId);
        //        //    return View(course);
        //        //}

        //        //db.Courses.Add(course);
        //        //db.SaveChanges();
        //        //return RedirectToAction("ManageCourses");
        //    }
        //    LoadDropdowns(course);
        //    return View(course);

        //    //ViewBag.Advisors = new SelectList(db.Advisors, "AdvisorId", "FullName", course.AdvisorId);
        //    //ViewBag.Departments = new SelectList(db.Departments, "DepartmentId", "DepartmentName", course.DepartmentId);
        //    //return View(course);
        //}
        //private void LoadDropdowns(Course course)
        //{
        //    var advisors = db.Users
        //        .Where(u => u.UserRoles.Any(ur => ur.Role.RoleName == "Advisor"))
        //        .Select(u => new
        //        {
        //            AdvisorId = u.UserId,
        //            UserName = u.UserName
        //        }).ToList();

        //    ViewBag.Advisors = new SelectList(advisors, "AdvisorId", "UserName", course.AdvisorId);
        //    ViewBag.Departments = new SelectList(db.Departments.ToList(), "DepartmentId", "DepartmentName", course.DepartmentId);
        //}


        // GET: Admin/EditCourse/{id}
        //public ActionResult EditCourse(int id)
        //{
        //    if (Session["UserRole"] == null || Session["UserRole"].ToString() != "Admin")
        //    {
        //        TempData["ErrorMessage"] = "Bu sayfaya erişmek için giriş yapmalısınız.";
        //        return RedirectToAction("Index", "Login");
        //    }
        //    var course = db.Courses.Find(id);
        //    if (course == null) return HttpNotFound();

        //    var advisors = db.Users
        //        .Include(u => u.UserRoles.Select(ur => ur.Role))
        //        .Where(u => u.UserRoles.Any(ur => ur.Role.RoleName == "Advisor"))
        //        .ToList();

        //    ViewBag.Advisors = new SelectList(advisors, "AdvisorId", "UserName", course.AdvisorId);
        //    ViewBag.Departments = new SelectList(db.Departments, "DepartmentId", "DepartmentName", course.DepartmentId);
        //    return View(course);
        //}

        //// POST: Admin/EditCourse
        //[HttpPost]
        //public ActionResult EditCourse(Course course)
        //{
        //    if (ModelState.IsValid)
        //    {
        //        db.Entry(course).State = EntityState.Modified;
        //        db.SaveChanges();
        //        return RedirectToAction("ManageCourses");
        //    }

        //    var advisors = db.Users
        //        .Include(u => u.UserRoles.Select(ur => ur.Role))
        //        .Where(u => u.UserRoles.Any(ur => ur.Role.RoleName == "Advisor"))
        //        .ToList();

        //    ViewBag.Advisors = new SelectList(advisors, "UserId", "UserName", course.AdvisorId);
        //    ViewBag.Departments = new SelectList(db.Departments, "DepartmentId", "DepartmentName", course.DepartmentId);
        //    return View(course);
        //}

        // GET: Admin/DeleteCourse/{id}
        //public ActionResult DeleteCourse(int id)
        //{
        //    var course = db.Courses.Find(id);
        //    if (course == null) return HttpNotFound();

        //    db.Courses.Remove(course);
        //    db.SaveChanges();
        //    return RedirectToAction("ManageCourses");
        //}

        // GET: Admin/DeleteDepartment/{id}
        public ActionResult DeleteDepartment(int id)
        {
            var department = db.Departments.Find(id);
            if (department == null) return HttpNotFound();

            db.Departments.Remove(department);
            db.SaveChanges();
            return RedirectToAction("ManageDepartments");
        }

        // GET: Admin/ManageDepartments
        public ActionResult ManageDepartments()
        {
            if (Session["UserRole"] == null || Session["UserRole"].ToString() != "Admin")
            {
                TempData["ErrorMessage"] = "Bu sayfaya erişmek için giriş yapmalısınız.";
                return RedirectToAction("GeneralLogin", "Login");
            }
            var departments = db.Departments.ToList();
            ViewBag.Departments = departments;

            return View(departments);
        }

        // GET: Admin/CreateDepartment
        //public ActionResult CreateDepartment(int? id)
        //{
        //    if (Session["UserRole"] == null || Session["UserRole"].ToString() != "Admin")
        //    {
        //        TempData["ErrorMessage"] = "Bu sayfaya erişmek için giriş yapmalısınız.";
        //        return RedirectToAction("GeneralLogin", "Login");
        //    }
        //    if (id.HasValue)
        //    {
        //        var department = db.Departments.Find(id.Value);
        //        if (department == null) return HttpNotFound();
        //        return View(department); // Düzenleme
        //    }

        //    return View(new Department()); // Yeni ekleme
        //}
        //[HttpGet]
        //public JsonResult GetDepartmentName(int id)
        //{
        //    var department = db.Departments.Find(id);
        //    if (department == null)
        //    {
        //        return Json(new { success = false }, JsonRequestBehavior.AllowGet);
        //    }

        //    return Json(new { success = true, name = department.DepartmentName }, JsonRequestBehavior.AllowGet);
        //}


        //[HttpPost]
        //public ActionResult CreateDepartmentInline(string DepartmentName)
        //{
        //    if (!string.IsNullOrWhiteSpace(DepartmentName))
        //    {
        //        db.Departments.Add(new Department { DepartmentName = DepartmentName });
        //        db.SaveChanges();
        //    }

        //    return RedirectToAction("ManageDepartments");
        //}
        // POST: Admin/UpdateDepartmentInline
        //[HttpPost]
        //public ActionResult UpdateDepartmentInline(int SelectedDepartmentId, string NewDepartmentName)
        //{
        //    var department = db.Departments.Find(SelectedDepartmentId);
        //    if (department != null && !string.IsNullOrWhiteSpace(NewDepartmentName))
        //    {
        //        department.DepartmentName = NewDepartmentName;
        //        db.SaveChanges();
        //    }

        //    return RedirectToAction("ManageDepartments");
        //}

        [HttpPost]
        public ActionResult SaveDepartment(int? DepartmentId, string DepartmentName)
        {
            if (string.IsNullOrWhiteSpace(DepartmentName))
            {
                TempData["ErrorMessage"] = "Bölüm adı boş olamaz.";
                return RedirectToAction("ManageDepartments");
            }

            if (DepartmentId.HasValue && DepartmentId.Value > 0)
            {
                // Güncelle
                var dept = db.Departments.Find(DepartmentId.Value);
                if (dept != null)
                {
                    dept.DepartmentName = DepartmentName;
                    TempData["SuccessMessage"] = "Bölüm başarıyla güncellendi.";
                }
            }
            else
            {
                // Yeni Ekle
                var newDept = new Department { DepartmentName = DepartmentName };
                db.Departments.Add(newDept);
                TempData["SuccessMessage"] = "Yeni bölüm başarıyla eklendi.";
            }

            db.SaveChanges();
            return RedirectToAction("ManageDepartments");
        }

        //// POST: Admin/CreateDepartment SİLİNECEK
        //[HttpPost]
        //public ActionResult CreateDepartment(Department Department)
        //{
        //    if (!ModelState.IsValid)
        //        return View(Department);

        //    if (Department.DepartmentId == 0)
        //    {
        //        db.Departments.Add(Department);
        //    }
        //    else
        //    {
        //       db.Entry(Department).State = System.Data.Entity.EntityState.Modified;
        //    }

        //    db.SaveChanges();
        //    return RedirectToAction("ManageDepartments");
        //}

        // GET: Admin/EditDepartment/5
        //public ActionResult EditDepartment(int id)
        //{
        //    var department = db.Departments.Find(id);
        //    if (department == null) return HttpNotFound();
        //    return View(department);
        //}

        //// POST: Admin/EditDepartment
        //[HttpPost]
        //public ActionResult EditDepartment(Department department)
        //{
        //    if (ModelState.IsValid)
        //    {
        //        db.Entry(department).State = EntityState.Modified;
        //        db.SaveChanges();
        //        return RedirectToAction("ManageDepartments");
        //    }
        //    return View(department);
        //}
        public ActionResult Review(int studentId)
        {
            if (Session["UserRole"] == null || Session["UserRole"].ToString() != "Admin")
            {
                TempData["ErrorMessage"] = "Bu sayfaya erişmek için giriş yapmalısınız.";
                return RedirectToAction("GeneralLogin", "Login");
            }

            var student = db.Users
                .FirstOrDefault(u => u.UserId == studentId);

            if (student == null)
                return HttpNotFound();

            // Öğrencinin seçtiği dersleri CourseSelections üzerinden al
            var courseSelections = db.CourseSelections
                .Include(cs => cs.Course)
                .Where(cs => cs.StudentId == studentId)
                .ToList();

            // Dersler listesi ViewModel'e aktarılıyor
            var model = courseSelections.Select(cs => new ReviewCourseViewModel
            {
                CourseSelectionId = cs.CourseSelectionId,
                CourseName = cs.Course.CourseName,
                AKTS = cs.Course.AKTS,
                IsApproved = cs.IsApprovedByAdvisor
            }).ToList();

            ViewBag.StudentName = student.UserName;
            ViewBag.StudentId = student.UserId;

            return View(model);
        }
        // GET: Admin/ManageTerms
        public ActionResult ManageTerms()
        {
            if (Session["UserRole"] == null || Session["UserRole"].ToString() != "Admin")
            {
                TempData["ErrorMessage"] = "Bu sayfaya erişmek için giriş yapmalısınız.";
                return RedirectToAction("GeneralLogin", "Login");
            }

            var terms = db.Terms.ToList();
            return View(terms);
        }

        [HttpPost]
        public ActionResult CreateOrUpdateTerm(int? TermId, string TermName, DateTime CourseSelectionStart, DateTime CourseSelectionEnd, bool IsActive)
        {
            try
            {
                if (IsActive)
                {
                    var aktifDonemVarMi = db.Terms.Any(t => t.IsActive && (!TermId.HasValue || t.TermId != TermId.Value));

                    if (aktifDonemVarMi)
                    {
                        TempData["AlertMessage"] = "Aynı anda iki donem aktif olamaz.";
                        TempData["ShowAlert"] = true;
                        return RedirectToAction("ManageTerms");
                    }
                }

                Term term;

                if (TermId.HasValue && TermId.Value > 0)
                {
                    term = db.Terms.Find(TermId.Value);
                    if (term == null)
                    {
                        TempData["ErrorMessage"] = "Donem bulunamadi.";
                        return RedirectToAction("ManageTerms");
                    }

                    term.TermName = TermName;
                    term.CourseSelectionStart = CourseSelectionStart;
                    term.CourseSelectionEnd = CourseSelectionEnd;
                    term.IsActive = IsActive;

                    TempData["SuccessMessage"] = "Donem basariyla guncellendi.";
                }
                else
                {
                    term = new Term
                    {
                        TermName = TermName,
                        CourseSelectionStart = CourseSelectionStart,
                        CourseSelectionEnd = CourseSelectionEnd,
                        IsActive = IsActive
                    };

                    db.Terms.Add(term);
                    TempData["SuccessMessage"] = "Yeni donem basariyla eklendi.";
                }

                db.SaveChanges();
                return RedirectToAction("ManageTerms");
            }
            catch (Exception ex)
            {
                TempData["ErrorMessage"] = "Bir hata olustu.";
                return RedirectToAction("ManageTerms");
            }
        }


        //public ActionResult GetTermDetails(int id)
        //{
        //    var term = db.Terms.Find(id);
        //    if (term == null) return Json(null, JsonRequestBehavior.AllowGet);

        //    var result = new
        //    {
        //        term.TermName,
        //        CourseSelectionStartString = term.CourseSelectionStart?.ToString("yyyy-MM-dd"),
        //        CourseSelectionEndString = term.CourseSelectionEnd?.ToString("yyyy-MM-dd"),
        //        term.IsActive
        //    };

        //    return Json(result, JsonRequestBehavior.AllowGet);
        //}


        // GET: Admin/CreateOrEditTerm
        //public ActionResult CreateOrEditTerm(int? id)
        //{
        //    if (Session["UserRole"] == null || Session["UserRole"].ToString() != "Admin")
        //    {
        //        TempData["ErrorMessage"] = "Bu sayfaya erişmek için giriş yapmalısınız.";
        //        return RedirectToAction("GeneralLogin", "Login");
        //    }
        //    if (id == null)
        //        return View(new Term()); // Yeni dönem

        //    var term = db.Terms.Find(id);
        //    if (term == null)
        //        return HttpNotFound();

        //    return View(term); // Güncelleme
        //}

        //// POST: Admin/CreateOrEditTerm
        //[HttpPost]
        //public ActionResult CreateOrEditTerm(Term term)
        //{
        //    if (!ModelState.IsValid)
        //        return View(term);

        //    // Eğer yeni eklenmek istenen ya da güncellenen dönem aktifse, başka aktif dönem olup olmadığı kontrol edilir
        //    if (term.IsActive)
        //    {
        //        bool anotherActiveExists = db.Terms.Any(t => t.IsActive && t.TermId != term.TermId);
        //        if (anotherActiveExists)
        //        {
        //            TempData["ErrorMessage"] = "Zaten aktif bir dönem var. Aynı anda birden fazla dönem aktif olamaz.";
        //            return View(term);
        //        }
        //    }

        //    if (term.TermId == 0)
        //    {
        //        db.Terms.Add(term); // Yeni dönem
        //    }
        //    else
        //    {
        //        db.Entry(term).State = System.Data.Entity.EntityState.Modified; // Güncelle
        //    }

        //    db.SaveChanges();
        //    TempData["SuccessMessage"] = "Dönem başarıyla kaydedildi.";
        //    return RedirectToAction("ManageTerms");
        //}

        // GET: Admin/DeleteTerm
        public ActionResult DeleteTerm(int id)
        {
            var term = db.Terms.Find(id);
            if (term == null) return HttpNotFound();

            db.Terms.Remove(term);
            db.SaveChanges();
            return RedirectToAction("ManageTerms");
        }



    }
}