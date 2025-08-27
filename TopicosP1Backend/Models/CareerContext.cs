using Microsoft.EntityFrameworkCore;
using TopicosP1Backend.Scripts;

namespace CareerApi.Models
{
    public class CareerContext: DbContext
    {
        public CareerContext(DbContextOptions<CareerContext> options)
        : base(options)
        {
        }

        public DbSet<Career> Careers { get; set; } = default!;
        public DbSet<StudyPlan> StudyPlans { get; set; } = default!;
        public DbSet<Subject> Subjects { get; set; } = default!;
        public DbSet<SpSubject> SpSubjects { get; set; } = default!;
        public DbSet<Prerequisites> Prerequisites { get; set; } = default!;
        public DbSet<User> Users { get; set; } = default!;
    }

    public static class Extensions
    {
        public static void CreateDbIfNotExists(this IHost host)
        {
            using var scope = host.Services.CreateScope();

            var services = scope.ServiceProvider;
            var context = services.GetRequiredService<CareerContext>();
            context.Database.EnsureCreated();
            DatabaseInitialization.Populate(context);
        }
    }
}
