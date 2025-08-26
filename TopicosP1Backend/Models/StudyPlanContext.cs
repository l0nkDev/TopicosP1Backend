using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;

namespace CareerApi.Models
{
    public class StudyPlanContext : IdentityDbContext<User>
    {
        public StudyPlanContext(DbContextOptions<StudyPlanContext> options)
        : base(options)
        {
        }

        public DbSet<StudyPlan> StudyPlans { get; set; } = default!;
    }
}

