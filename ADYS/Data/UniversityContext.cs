using ADYS.Models;
using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Web;

namespace ADYS.Data
{
    public class UniversityContext : DbContext
    {
        public UniversityContext() : base("UniversityContext") { }

        public DbSet<Student> Students { get; set; }
        public DbSet<Advisor> Advisors { get; set; }
        public DbSet<Course> Courses { get; set; }
        public DbSet<CourseSelection> CourseSelections { get; set; }

    }
}