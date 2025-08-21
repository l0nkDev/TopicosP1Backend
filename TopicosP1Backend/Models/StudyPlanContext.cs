using Microsoft.EntityFrameworkCore;

namespace CareerApi.Models
{
    public class StudyPlanContext : DbContext
    {
        public StudyPlanContext(DbContextOptions<StudyPlanContext> options)
        : base(options)
        {
        }

        public DbSet<StudyPlan> StudyPlans { get; set; } = default!;
    }
}

