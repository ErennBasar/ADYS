using ADYS.Models;
using System.Data.Entity; 

namespace ADYS.Data
{
    public class UniversityContext : DbContext
    {
        public UniversityContext() : base("UniversityContext") { }
        public DbSet<Course> Courses { get; set; }
        public DbSet<Term> Terms { get; set; }
        public DbSet<User> Users { get; set; }
        public DbSet<Role> Roles { get; set; }
        public DbSet<UserRole> UserRoles { get; set; }
        public DbSet<AdvisorAssignment> AdvisorAssignments { get; set; }

        public DbSet<CourseSelection> CourseSelections { get; set; }
        public DbSet<Department> Departments { get; set; } 

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
                .WithMany(d => d.Courses)
                .HasForeignKey(c => c.DepartmentId)
                .WillCascadeOnDelete(false);

            modelBuilder.Entity<CourseSelection>()
                .HasRequired(cs => cs.Student)
                .WithMany()
                .HasForeignKey(cs => cs.StudentId)
                .WillCascadeOnDelete(false);

            modelBuilder.Entity<UserRole>()
                .HasRequired(ur => ur.User)
                .WithMany(u => u.UserRoles)
                .HasForeignKey(ur => ur.UserId)
                .WillCascadeOnDelete(false);

            modelBuilder.Entity<UserRole>() 
                 .HasRequired(ur => ur.Role) // bu taraf çok
                 .WithMany(r => r.UserRoles) // bu tafaf tek
                 .HasForeignKey(ur => ur.RoleId) // FK bu alan
                 .WillCascadeOnDelete(false);

            modelBuilder.Entity<User>()
                 .HasOptional(u => u.Advisor)
                 .WithMany(a => a.Students)
                 .HasForeignKey(u => u.AdvisorId)
                 .WillCascadeOnDelete(false);

            modelBuilder.Entity<AdvisorAssignment>()
            .HasRequired(aa => aa.Student)
            .WithMany(u => u.AdvisorAssignments)
            .HasForeignKey(aa => aa.StudentId)
            .WillCascadeOnDelete(false);

            modelBuilder.Entity<AdvisorAssignment>()
                .HasRequired(aa => aa.Advisor)
                .WithMany(u => u.AdvisorStudents)
                .HasForeignKey(aa => aa.AdvisorId)
                .WillCascadeOnDelete(false);


        }
    }
}
