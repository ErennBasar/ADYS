using MyApiProject.DTOs;
using MyApiProject.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web.Http;
using System.Data.Entity;

namespace MyApiProject.Controllers
{
    public class AdminApiController : ApiController
    {
        private ContextDBEntities db = new ContextDBEntities();

        [HttpGet]
        public IHttpActionResult Dashboard()
        {
            var model = db.AdvisorAssignments
                .Include(a => a.Student)
                .Where(a => a.Student.IsActive)
                .Include(a => a.Advisor)
                .Select(a => new StudentDashboardDto
                {
                    StudentId = a.Student.UserId,
                    StudentName = a.Student.UserName,
                    StudentEmail = a.Student.Email,
                    AdvisorName = a.Advisor.UserName,
                    AdvisorEmail = a.Advisor.Email
                })
                .ToList();

            return Ok(model);
        }

        [HttpGet]
        public IHttpActionResult Review(int studentId)
        {
            var student = db.Users.FirstOrDefault(u => u.UserId == studentId);
            if (student == null) return NotFound();

            var courseSelections = db.CourseSelections
                .Include(cs => cs.Courses)
                .Where(cs => cs.StudentId == studentId)
                .ToList();

            var model = courseSelections.Select(cs => new ReviewCourseDto
            {
                CourseSelectionId = cs.CourseSelectionId,
                CourseName = cs.Courses.CourseName,
                AKTS = cs.Courses.AKTS,
                IsApproved = cs.IsApprovedByAdvisor
            }).ToList();

            return Ok(new
            {
                // öğrenci adı ve ID ViewBag ile gönderildiği için bu şekilde gönderdik
                StudentName = student.UserName,
                StudentId = student.UserId,
                Courses = model,
            });
        }

        [HttpGet]
        public IHttpActionResult ManageStudents()
        {
            var studentsWithAdvisors = db.AdvisorAssignments
                .Include(a => a.Student)
                .Where(a => a.Student.IsActive)
                .Include(a => a.Advisor)
                .Select(a => new StudentDashboardDto
                {
                    StudentId = a.Student.UserId,
                    StudentName = a.Student.UserName,
                    StudentEmail = a.Student.Email,
                    AdvisorName = a.Advisor.UserName,
                    AdvisorEmail = a.Advisor.Email,
                })
                .ToList();

            return Ok(studentsWithAdvisors);
        }
        
        // Advisor Selectlist için
        [HttpGet]
        public IHttpActionResult GetAdvisors()
        {
            var advisorSelectlist = db.Users
                .Where(u => u.UserRoles.Any(r => r.Roles.RoleName == "Advisor"))
                .Select(u => new
                {
                    UserId = u.UserId,
                    UserName = u.UserName
                })
                .ToList();

            return Ok(advisorSelectlist);
        }
        
        [HttpPost]
        public IHttpActionResult AddOrUpdateStudent(AddOrUpdateStudentDto dto)
        {
            if (dto == null)
                return BadRequest("Geçersiz veri.");

            if (string.IsNullOrWhiteSpace(dto.FullName) || string.IsNullOrWhiteSpace(dto.Email))
                return BadRequest("İsim ve email boş olamaz.");

            if (dto.StudentId.HasValue)
            {
                var existing = db.Users.FirstOrDefault(x => x.UserId == dto.StudentId.Value);
                if (existing == null)
                    return NotFound();

                existing.UserName = dto.FullName;
                existing.Email = dto.Email;
                if (!string.IsNullOrWhiteSpace(dto.Password))
                    existing.Password = dto.Password;
                existing.UpdateDate = DateTime.Now;

                var assignment = db.AdvisorAssignments.FirstOrDefault(a => a.StudentId == existing.UserId);
                if (assignment != null)
                {
                    assignment.AdvisorId = dto.AdvisorId;
                }
                else
                {
                    db.AdvisorAssignments.Add(new AdvisorAssignments
                    {
                        StudentId = existing.UserId,
                        AdvisorId = dto.AdvisorId
                    });
                }

                db.SaveChanges();
                return Ok(new { success = true, message = "Öğrenci güncellendi." });
            }
            else
            {
                var student = new Users
                {
                    UserName = dto.FullName,
                    Email = dto.Email,
                    Password = dto.Password,
                    CreateDate = DateTime.Now,
                    IsActive = true
                };

                db.Users.Add(student);
                db.SaveChanges();

                var studentRole = db.Roles.FirstOrDefault(r => r.RoleName == "Student");
                if (studentRole != null)
                {
                    db.UserRoles.Add(new UserRoles
                    {
                        UserId = student.UserId,
                        RoleId = studentRole.RoleId
                    });
                }

                db.AdvisorAssignments.Add(new AdvisorAssignments
                {
                    StudentId = student.UserId,
                    AdvisorId = dto.AdvisorId
                });

                db.SaveChanges();
                return Ok(new { success = true, message = "Yeni öğrenci eklendi." });
            }
        }

        [HttpDelete]        
        public IHttpActionResult DeleteStudent(int id)
        {
            var student = db.Users
                .Include(u => u.CourseSelections)
                .Include(u => u.AdvisorAssignments)
                .Include(u => u.UserRoles)
                .FirstOrDefault(u => u.UserId == id);

            if (student == null)
            {
                return NotFound();
            }

            // İlişkili verileri sil
            db.CourseSelections.RemoveRange(student.CourseSelections);
            db.AdvisorAssignments.RemoveRange(student.AdvisorAssignments);
            db.UserRoles.RemoveRange(student.UserRoles);

            // Soft delete
            student.IsDeleted = true;
            student.IsActive = false;
            student.DeleteDate = DateTime.Now;

            db.SaveChanges();

            return Ok(new { success = true });
        }

        [HttpGet]
        public IHttpActionResult ManageAdvisors()
        {
            var advisors = db.Users
                .Where(u =>
                    u.IsActive &&
                    u.UserRoles.Any(ur => ur.Roles.RoleName == "Advisor")
                )
                .Select(u => new AdvisorDto
                {
                    UserId = u.UserId,
                    UserName = u.UserName,
                    Email = u.Email,

                })
                .ToList();

            return Ok(advisors);
        }
        [HttpPost]
        public IHttpActionResult CreateOrUpdateAdvisor(AdvisorAddOrUpdateDto dto)
        {
            if (dto.AdvisorId.HasValue)
            {
                // GÜNCELLEME
                var existing = db.Users.FirstOrDefault(x => x.UserId == dto.AdvisorId);
                if (existing == null)
                    return NotFound();

                existing.UserName = dto.UserName;
                existing.Email = dto.Email;
                if (!string.IsNullOrWhiteSpace(dto.Password))
                    existing.Password = dto.Password;

                existing.UpdateDate = DateTime.Now;

                db.SaveChanges();

                return Ok(new { success = true, message = "Danışman güncellendi." });
            }
            else
            {
                // EKLEME
                var advisor = new Users
                {
                    UserName = dto.UserName,
                    Email = dto.Email,
                    Password = dto.Password,
                    CreateDate = DateTime.Now,
                    IsActive = true
                };

                db.Users.Add(advisor);
                db.SaveChanges();

                var advisorRole = db.Roles.FirstOrDefault(r => r.RoleName == "Advisor");
                if (advisorRole != null)
                {
                    db.UserRoles.Add(new UserRoles
                    {
                        UserId = advisor.UserId,
                        RoleId = advisorRole.RoleId
                    });
                }

                db.SaveChanges();

                return Ok(new { success = true, message = "Yeni danışman eklendi." });
            }
        }

        [HttpDelete]
        public IHttpActionResult DeleteAdvisor(int advisorId)
        {
            var advisor = db.Users.FirstOrDefault(u => u.UserId == advisorId);
            if(advisor == null)
            {
                return Json(new
                {
                    succes = false,
                    message = "Danışman Bulunamadı"
                });
            }

            advisor.IsActive = false;
            advisor.IsDeleted = true;
            advisor.DeleteDate = DateTime.Now;

            db.SaveChanges();

            return Json(new
            {
                success = true,
                message = "Danışman başarıyla pasif hale getirildi."
            });

        }

        [HttpGet]
        public IHttpActionResult ManageCourses()
        {
            // aktif danışmanlar
            var advisors = db.Users
                .Where(u => u.IsActive && u.UserRoles.Any(r => r.Roles.RoleName == "Advisor"))
                .Select(u => new AdvisorDto
                {
                    UserId = u.UserId,
                    UserName = u.UserName
                })
                .ToList();
            // Departmanlar
            var departments = db.Departments
                .Select(d => new DepartmentDto
                {
                    DepartmentId = d.DepartmentId,
                    DepartmentName = d.DepartmentName
                })
                .ToList();


            // Dersler (çakışma durumları veya danışman boş olabilir o yüzden Advisor null olabilir)
            var courses = db.Courses
               .Include(c => c.Departments)
               .Include(c => c.Users)
               .Select(c => new CourseDto
                {
                    CourseId = c.CourseId,
                    CourseName = c.CourseName,
                    Credits = c.AKTS,
                    Quota = c.Kontenjan,
                    AdvisorId = c.AdvisorId,
                    AdvisorName =  c.Users.UserName,
                    DepartmentId = c.DepartmentId,
                    DepartmentName = c.Departments.DepartmentName,
                    Day = c.DayOfWeek,
                    StartTime = c.StartTime,
                    EndTime = c.EndTime
                })
               .ToList();

            // Ana DTO ile birlikte dön
            var result = new ManageCoursesResponseDto
            {
                Courses = courses,
                Advisors = advisors,
                Departments = departments
            };
            return Ok(result);
        }

        [HttpPost]       
        public IHttpActionResult CreateOrUpdateCourse(CourseDto dto)
        {
            if (dto == null)
                return BadRequest("Gönderilen veri boş.");

            // DTO'dan gelen verileri kontrol et
            if (string.IsNullOrEmpty(dto.CourseName) || dto.Credits <= 0 || dto.Quota <= 0)
                return BadRequest("Zorunlu alanlar eksik veya hatalı.");

            try
            {
                // Güncelleme mi yoksa yeni kayıt mı?
                Courses course;
                if (dto.CourseId != 0)
                {
                    // Güncelleme işlemi
                    course = db.Courses.FirstOrDefault(c => c.CourseId == dto.CourseId);
                    if (course == null)
                        return NotFound();

                    // Alanları güncelle
                    course.CourseName = dto.CourseName;
                    course.AKTS = dto.Credits;
                    course.Kontenjan = dto.Quota;
                    course.DayOfWeek = dto.Day;
                    course.StartTime = dto.StartTime;
                    course.EndTime = dto.EndTime;
                    course.AdvisorId = dto.AdvisorId;
                    course.DepartmentId = dto.DepartmentId;

                    db.SaveChanges();
                    return Ok(new { success = true, message = "Ders başarıyla güncellendi." });
                }
                else
                {
                    // Yeni ders ekleme
                    course = new Courses
                    {
                        CourseName = dto.CourseName,
                        AKTS = dto.Credits,
                        Kontenjan = dto.Quota,
                        DayOfWeek = dto.Day,
                        StartTime = dto.StartTime,
                        EndTime = dto.EndTime,
                        AdvisorId = dto.AdvisorId,
                        DepartmentId = dto.DepartmentId
                    };

                    db.Courses.Add(course);
                    db.SaveChanges();
                    return Ok(new { success = true, message = "Ders başarıyla eklendi." });
                }
            }
            catch (Exception ex)
            {
                return InternalServerError(ex);
            }
        }

        [HttpPost]        
        public IHttpActionResult DeleteCourse(int courseId)
        {
            if (courseId <= 0)
                return BadRequest("Geçersiz ders ID.");

            var course = db.Courses.Find(courseId);
            if (course == null)
                return NotFound();

            db.Courses.Remove(course);
            db.SaveChanges();

            return Ok(new { success = true, message = "Ders başarıyla silindi." });
        }



        [HttpGet]
        public IHttpActionResult GetDepartments()
        {
            var departments = db.Departments               
                .Select(d => new DepartmentDto
                {
                    DepartmentId = d.DepartmentId,
                    DepartmentName = d.DepartmentName
                })
                .ToList();

            return Ok(departments);
        }

        [HttpPost]
        public IHttpActionResult CreateOrUpdateDepartment(DepartmentDto dto)
        {
            if (string.IsNullOrWhiteSpace(dto.DepartmentName))
            {
                return BadRequest("Bölüm adı boş olamaz.");
            }

            if (dto.DepartmentId.HasValue && dto.DepartmentId.Value > 0)
            {
                // Güncelleme
                var dept = db.Departments.Find(dto.DepartmentId.Value);
                if (dept == null)
                {
                    return NotFound();
                }

                dept.DepartmentName = dto.DepartmentName;
                db.SaveChanges();

                return Ok(new { success = true, message = "Bölüm başarıyla güncellendi." });
            }
            else
            {
                // Ekleme
                var newDept = new Departments
                {
                    DepartmentName = dto.DepartmentName
                };

                db.Departments.Add(newDept);
                db.SaveChanges();

                return Ok(new { success = true, message = "Yeni bölüm başarıyla eklendi." });
            }
        }

        [HttpGet]
        public IHttpActionResult ManageTerms()
        {
            var terms = db.Terms
                .Select(s => new TermsDto
                {
                    TermId = s.TermId,
                    TermName = s.TermName,
                    TermStartTime = s.CourseSelectionStart,
                    TermEndTime = s.CourseSelectionEnd,
                    IsActive =s.IsActive,
                })
                .ToList();
        
            return Ok(terms);

        }
        [HttpPost]
        public IHttpActionResult CreateOrUpdateTerm(TermsDto dto)
        {
            if (string.IsNullOrWhiteSpace(dto.TermName))
                return BadRequest("Dönem adı boş olamaz.");

            if (dto.TermId.HasValue && dto.TermId.Value > 0)
            {
                var term = db.Terms.Find(dto.TermId.Value);
                if (term == null) return NotFound();

                term.TermName = dto.TermName;
                term.CourseSelectionStart = dto.TermStartTime;
                term.CourseSelectionEnd = dto.TermEndTime;
                term.IsActive = dto.IsActive;

                db.SaveChanges();
                return Ok(new { success = true, message = "Dönem başarıyla güncellendi." });
            }
            else
            {
                var newTerm = new Terms
                {
                    TermName = dto.TermName,
                    CourseSelectionStart = dto.TermStartTime,
                    CourseSelectionEnd = dto.TermEndTime,
                    IsActive = dto.IsActive
                };

                db.Terms.Add(newTerm);
                db.SaveChanges();

                return Ok(new { success = true, message = "Yeni dönem başarıyla eklendi." });
            }
        }
        [HttpDelete]
        public IHttpActionResult DeleteTerm(int termId)
        {
            var term = db.Terms.Find(termId);
            if (term == null)
            {
                return NotFound();
            }

            db.Terms.Remove(term);
            db.SaveChanges();

            return Ok(new { success = true, message = "Dönem başarıyla silindi." });
        }


    }
}
