using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using ADYS.Data;
using System.Data.Entity;
using ADYS.Models;
using System.Web.Mvc;

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
                return RedirectToAction("Index", "Login");
            }
            var students = db.Students.Include(s => s.Advisor).ToList();
            return View(students);
        }

        // === Öğrenci Tanımlamaları ===
        public ActionResult ManageStudents()
        {
            if (Session["UserRole"] == null || Session["UserRole"].ToString() != "Admin")
            {
                TempData["ErrorMessage"] = "Bu sayfaya erişmek için giriş yapmalısınız.";
                return RedirectToAction("Index", "Login");
            }
            var students = db.Students.Include("Advisor").ToList();
            ViewBag.Advisors = new SelectList(db.Advisors, "AdvisorId", "FullName");
            return View(students);
        }

        [HttpPost]
        public ActionResult AddStudent(Student student)
        {
            if (Session["UserRole"] == null || Session["UserRole"].ToString() != "Admin")
            {
                TempData["ErrorMessage"] = "Bu sayfaya erişmek için giriş yapmalısınız.";
                return RedirectToAction("Index", "Login");
            }
            if (ModelState.IsValid)
            {
                db.Students.Add(student);
                db.SaveChanges();
                return RedirectToAction("ManageStudents");
            }

            ViewBag.Advisors = new SelectList(db.Advisors, "AdvisorId", "FullName");
            var students = db.Students.Include("Advisor").ToList();
            return View("ManageStudents", students);
        }

        public ActionResult DeleteStudent(int id)
        {

            var student = db.Students.Find(id);
            if (student != null)
            {
                db.Students.Remove(student);
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
                return RedirectToAction("Index", "Login");
            }

            var advisors = db.Advisors.ToList();
            return View(advisors);
        }

        // Ekle / Güncelle (GET)
        public ActionResult CreateAdvisor(int? id)
        {
            if (Session["UserRole"] == null || Session["UserRole"].ToString() != "Admin")
            {
                TempData["ErrorMessage"] = "Bu sayfaya erişmek için giriş yapmalısınız.";
                return RedirectToAction("Index", "Login");
            }
            if (id == null)
                return View(new Advisor()); // Yeni kayıt
            var advisor = db.Advisors.Find(id);
            if (advisor == null)
                return HttpNotFound();
            return View(advisor); // Güncelleme
        }

        // Ekle / Güncelle (POST)
        [HttpPost]
        public ActionResult CreateAdvisor(Advisor advisor)
        {

            if (!ModelState.IsValid)
                return View(advisor);

            if (advisor.AdvisorId == 0)
            {
                db.Advisors.Add(advisor); // Yeni
            }
            else
            {
                db.Entry(advisor).State = EntityState.Modified; // Güncelle
            }

            db.SaveChanges();
            return RedirectToAction("ManageAdvisors");
        }

        // Silme işlemi
        public ActionResult DeleteAdvisor(int id)
        {
            var advisor = db.Advisors.Find(id);
            if (advisor == null)
                return HttpNotFound();

            db.Advisors.Remove(advisor);
            db.SaveChanges();
            return RedirectToAction("ManageAdvisors");
        }
        // GET: Admin/ManageCourses
        public ActionResult ManageCourses()
        {
            if (Session["UserRole"] == null || Session["UserRole"].ToString() != "Admin")
            {
                TempData["ErrorMessage"] = "Bu sayfaya erişmek için giriş yapmalısınız.";
                return RedirectToAction("Index", "Login");
            }
            var courses = db.Courses.Include(c => c.Advisor).Include(c => c.Department).ToList();
            return View(courses);
        }

        // GET: Admin/CreateCourse
        public ActionResult CreateCourse()
        {
            if (Session["UserRole"] == null || Session["UserRole"].ToString() != "Admin")
            {
                TempData["ErrorMessage"] = "Bu sayfaya erişmek için giriş yapmalısınız.";
                return RedirectToAction("Index", "Login");
            }
            ViewBag.Advisors = new SelectList(db.Advisors, "AdvisorId", "FullName");
            ViewBag.Departments = new SelectList(db.Departments, "DepartmentId", "DepartmentName");
            return View();
        }

        // POST: Admin/CreateCourse
        [HttpPost]
        public ActionResult CreateCourse(Course course)
        {
            if (ModelState.IsValid)
            {
                // Danışman ID geçerli mi kontrol 
                if (!db.Advisors.Any(a => a.AdvisorId == course.AdvisorId))
                {
                    ModelState.AddModelError("", "Geçerli bir danışman seçmelisiniz.");
                    // ViewBag'leri yeniden doldur
                    ViewBag.Advisors = new SelectList(db.Advisors, "AdvisorId", "FullName", course.AdvisorId);
                    ViewBag.Departments = new SelectList(db.Departments, "DepartmentId", "DepartmentName", course.DepartmentId);
                    return View(course);
                }

                db.Courses.Add(course);
                db.SaveChanges();
                return RedirectToAction("ManageCourses");
            }

            ViewBag.Advisors = new SelectList(db.Advisors, "AdvisorId", "FullName", course.AdvisorId);
            ViewBag.Departments = new SelectList(db.Departments, "DepartmentId", "DepartmentName", course.DepartmentId);
            return View(course);
        }


        // GET: Admin/EditCourse/{id}
        public ActionResult EditCourse(int id)
        {
            if (Session["UserRole"] == null || Session["UserRole"].ToString() != "Admin")
            {
                TempData["ErrorMessage"] = "Bu sayfaya erişmek için giriş yapmalısınız.";
                return RedirectToAction("Index", "Login");
            }
            var course = db.Courses.Find(id);
            if (course == null) return HttpNotFound();

            ViewBag.Advisors = new SelectList(db.Advisors, "AdvisorId", "FullName", course.AdvisorId);
            ViewBag.Departments = new SelectList(db.Departments, "DepartmentId", "DepartmentName", course.DepartmentId);
            return View(course);
        }

        // POST: Admin/EditCourse
        [HttpPost]
        public ActionResult EditCourse(Course course)
        {
            if (ModelState.IsValid)
            {
                db.Entry(course).State = EntityState.Modified;
                db.SaveChanges();
                return RedirectToAction("ManageCourses");
            }

            ViewBag.Advisors = new SelectList(db.Advisors, "AdvisorId", "FullName", course.AdvisorId);
            ViewBag.Departments = new SelectList(db.Departments, "DepartmentId", "DepartmentName", course.DepartmentId);
            return View(course);
        }

        // GET: Admin/DeleteCourse/{id}
        public ActionResult DeleteCourse(int id)
        {
            var course = db.Courses.Find(id);
            if (course == null) return HttpNotFound();

            db.Courses.Remove(course);
            db.SaveChanges();
            return RedirectToAction("ManageCourses");
        }

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
                return RedirectToAction("Index", "Login");
            }
            var departments = db.Departments.ToList();
            return View(departments);
        }

        // GET: Admin/CreateDepartment
        public ActionResult CreateDepartment(int? id)
        {
            if (Session["UserRole"] == null || Session["UserRole"].ToString() != "Admin")
            {
                TempData["ErrorMessage"] = "Bu sayfaya erişmek için giriş yapmalısınız.";
                return RedirectToAction("Index", "Login");
            }
            if (id.HasValue)
            {
                var department = db.Departments.Find(id.Value);
                if (department == null) return HttpNotFound();
                return View(department); // Düzenleme
            }

            return View(new Department()); // Yeni ekleme
        }

        // POST: Admin/CreateDepartment
        [HttpPost]
        public ActionResult CreateDepartment(Department department)
        {
            if (!ModelState.IsValid)
                return View(department);

            if (department.DepartmentId == 0)
            {
                db.Departments.Add(department);
            }
            else
            {
                db.Entry(department).State = System.Data.Entity.EntityState.Modified;
            }

            db.SaveChanges();
            return RedirectToAction("ManageDepartments");
        }

        // GET: Admin/EditDepartment/5
        public ActionResult EditDepartment(int id)
        {
            var department = db.Departments.Find(id);
            if (department == null) return HttpNotFound();
            return View(department);
        }

        // POST: Admin/EditDepartment
        [HttpPost]
        public ActionResult EditDepartment(Department department)
        {
            if (ModelState.IsValid)
            {
                db.Entry(department).State = EntityState.Modified;
                db.SaveChanges();
                return RedirectToAction("ManageDepartments");
            }
            return View(department);
        }
        public ActionResult Review(int studentId)
        {
            if (Session["UserRole"] == null || Session["UserRole"].ToString() != "Admin")
            {
                TempData["ErrorMessage"] = "Bu sayfaya erişmek için giriş yapmalısınız.";
                return RedirectToAction("Index", "Login");
            }
            var student = db.Students
                .Include("CourseSelections.Course")
                .FirstOrDefault(s => s.StudentId == studentId);

            if (student == null)
                return HttpNotFound();

            var model = student.CourseSelections.Select(cs => new ADYS.ViewModels.ReviewCourseViewModel
            {
                CourseSelectionId = cs.CourseSelectionId,
                CourseName = cs.Course.CourseName,
                AKTS = cs.Course.AKTS,
                IsApproved = cs.IsApprovedByAdvisor
            }).ToList();

            ViewBag.StudentName = student.FullName;
            ViewBag.StudentId = student.StudentId;

            return View(model);
        }


    }
}