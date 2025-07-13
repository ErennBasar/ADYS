using ADYS.Models;
using System.Data.Entity; 

namespace ADYS.Data
{
    public class UniversityContext : DbContext
    {
        public UniversityContext() : base("UniversityContext") { }

        public DbSet<Student> Students { get; set; }
        public DbSet<Advisor> Advisors { get; set; }
        public DbSet<Course> Courses { get; set; }
        public DbSet<CourseSelection> CourseSelections { get; set; }
        public DbSet<Department> Departments { get; set; } // department tanımını unutma

        protected override void OnModelCreating(DbModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            modelBuilder.Entity<Course>()
                .HasRequired(c => c.Advisor)
                .WithMany()
                .HasForeignKey(c => c.AdvisorId)
                .WillCascadeOnDelete(false);

            modelBuilder.Entity<Course>()
                .HasRequired(c => c.Department)
                .WithMany()
                .HasForeignKey(c => c.DepartmentId)
                .WillCascadeOnDelete(false);
        }
    }
}
