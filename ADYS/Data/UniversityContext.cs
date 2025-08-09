
using System.Data.Entity; 

namespace ADYS.Data
{
    public class UniversityContext : DbContext
    {
        public UniversityContext() : base("UniversityContext") { }
        

        protected override void OnModelCreating(DbModelBuilder modelBuilder)
        {
           


        }
    }
}
