using Microsoft.EntityFrameworkCore;

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
    }

    public static class Extensions
    {
        public static void CreateDbIfNotExists(this IHost host)
        {
            using var scope = host.Services.CreateScope();

            var services = scope.ServiceProvider;
            var context = services.GetRequiredService<CareerContext>();
            context.Database.EnsureCreated();
            DbInitializer.Initialize(context);
        }
    }

    public static class DbInitializer
    {
        public static void Initialize(CareerContext context)
        {
            Console.WriteLine("triggered!");
            if (context.Careers.Any())
                return;

            var careers = new List<Career>
            {
                new() { Name = "INGENIERIA INFORMATICA" },
                new() { Name = "INGENIERIA EN SISTEMAS" },
                new() { Name = "INGENIERIA EN REDES Y TELECOMUNICACIONES" },
            };
            context.Careers.AddRange(careers);
            context.SaveChanges();

            var studyplans = new List<StudyPlan>
            {
                new() { Code = "187-3", Career = careers[0] },
                new() { Code = "187-4", Career = careers[1] },
                new() { Code = "187-5", Career = careers[2] },
            };
            context.StudyPlans.AddRange(studyplans);
            context.SaveChanges();

            var subjects = new List<Subject>
            {
                new() { Code = "MAT101", Title = "Cálculo I" },                     //0
                new() { Code = "INF119", Title = "Estructuras Discretas" },         //1
                new() { Code = "INF110", Title = "Introducción a la Informática" }, //2
                new() { Code = "FIS100", Title = "Física I" },                      //3
                new() { Code = "LIN100", Title = "Inglés Técnico I" },              //4
                new() { Code = "MAT102", Title = "Cálculo II" },                    //5
                new() { Code = "MAT103", Title = "Algebra Lineal" },                //6
                new() { Code = "INF120", Title = "Programación I" },                //7
                new() { Code = "FIS102", Title = "Física II" },                     //8
                new() { Code = "LIN101", Title = "Inglés Técnico II" },             //9
            };
            subjects[0].Postrequisites.Add(subjects[5]);
            subjects[1].Postrequisites.Add(subjects[6]);
            subjects[2].Postrequisites.Add(subjects[7]);
            subjects[3].Postrequisites.Add(subjects[8]);
            subjects[4].Postrequisites.Add(subjects[9]);
            context.Subjects.AddRange(subjects);
            context.SaveChanges();

            var spsubjects = new List<SpSubject>()
            {
                new() { StudyPlan = studyplans[0], Subject = subjects[0], Credits = 5, Level = 1, Type = 1},
                new() { StudyPlan = studyplans[0], Subject = subjects[1], Credits = 5, Level = 1, Type = 1},
                new() { StudyPlan = studyplans[0], Subject = subjects[2], Credits = 5, Level = 1, Type = 1},
                new() { StudyPlan = studyplans[0], Subject = subjects[3], Credits = 5, Level = 1, Type = 1},
                new() { StudyPlan = studyplans[0], Subject = subjects[4], Credits = 5, Level = 1, Type = 1},
                new() { StudyPlan = studyplans[0], Subject = subjects[5], Credits = 5, Level = 2, Type = 1},
                new() { StudyPlan = studyplans[0], Subject = subjects[6], Credits = 5, Level = 2, Type = 1},
                new() { StudyPlan = studyplans[0], Subject = subjects[7], Credits = 5, Level = 2, Type = 1},
                new() { StudyPlan = studyplans[0], Subject = subjects[8], Credits = 5, Level = 2, Type = 1},
                new() { StudyPlan = studyplans[0], Subject = subjects[9], Credits = 5, Level = 2, Type = 1},
            };
            context.SpSubjects.AddRange(spsubjects);
            context.SaveChanges();

        }
    }
}
