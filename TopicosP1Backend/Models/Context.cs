using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Hosting;
using Microsoft.Net.Http.Headers;
using TopicosP1Backend.Scripts;

namespace CareerApi.Models
{
    public class Context: DbContext
    {
        public Context(DbContextOptions<Context> options)
        : base(options)
        {
        }

        public DbSet<Career> Careers { get; set; } = default!;
        public DbSet<StudyPlan> StudyPlans { get; set; } = default!;
        public DbSet<Subject> Subjects { get; set; } = default!;
        public DbSet<SpSubject> SpSubjects { get; set; } = default!;
        public DbSet<SubjectDependency> Prerequisites { get; set; } = default!;
        public DbSet<User> Users { get; set; } = default!;
        public char GetRequestUserRole(HttpRequest request)
        {
            string token = request.Headers[HeaderNames.Authorization].ToString().Replace("Bearer ", "");
            Console.WriteLine(token);
            var user = Users.FirstOrDefault(x => x.Token == token);
            if (user == null) { Console.WriteLine("User not found."); return 'n'; }
            return user.Role;
        }
        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<Subject>()
                .HasMany(e => e.StudyPlans)
                .WithMany(e => e.Subjects)
                .UsingEntity<SpSubject>();

            modelBuilder.Entity<StudyPlan>()
                .HasMany(e => e.Subjects)
                .WithMany(e => e.StudyPlans)
                .UsingEntity<SpSubject>();

            modelBuilder.Entity<Subject>()
                .HasMany(e => e.Prerequisites)
                .WithMany(e => e.Postrequisites)
                .UsingEntity<SubjectDependency>(
                    r => r.HasOne<Subject>(e => e.Prerequisite).WithMany(e => e.PreDependencies),
                    s => s.HasOne<Subject>(e => e.Postrequisite).WithMany(e => e.PostDependencies)
                );
        }
    }

    public static class Extensions
    {
        public static void CreateDbIfNotExists(this IHost host)
        {
            using var scope = host.Services.CreateScope();

            var services = scope.ServiceProvider;
            var context = services.GetRequiredService<Context>();
            context.Database.EnsureCreated();
            DatabaseInitialization.Populate(context);
        }
    }
}
