using Microsoft.EntityFrameworkCore;

namespace CareerApi.Models
{
    public class SubjectContext : DbContext
    {
        public SubjectContext(DbContextOptions<StudyPlanContext> options)
        : base(options)
        {
        }

        public DbSet<Subject> Subjects { get; set; } = default!;
    }
}

