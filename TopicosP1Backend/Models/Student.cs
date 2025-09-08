using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.ComponentModel.DataAnnotations;
using System.Diagnostics.CodeAnalysis;
using TopicosP1Backend.Scripts;

namespace CareerApi.Models
{
    public class Student
    {
        public long Id { get; set; }
        required public string FirstName { get; set; }
        required public string LastName { get; set; }
        public IEnumerable<StudyPlan> StudyPlans { get; set; } = new List<StudyPlan>();
        public IEnumerable<StudentGroups> StudentGroups { get; set; } = new List<StudentGroups>();
        public IEnumerable<Group> Groups { get; set; } = new List<Group>();
        public StudentDTO Simple() => new(this);
        public class StudentDTO(Student s)
        {
            public long Id { get; set; } = s.Id;
            public string FirstName { get; set; } = s.FirstName;
            public string LastName { get; set; } = s.LastName;
        }

        public static async Task<ActionResult<List<StudentGroups.HistoryEntry>>> History(Context context, long id)
        {
            var student = await context.Students.FindAsync(id);
            if (student == null) { return new NotFoundResult(); }
            var history = await context.StudentGroups.Where(_ => _.Student.Id == id && _.Status != 2).ToListAsync();
            List<StudentGroups.HistoryEntry> res = [.. (from a in history select a.Simple())];
            return res;
        }

        public static async Task<ActionResult<List<Subject.SubjectWithGroups>>> Available(Context context, long id)
        {
            var student = await context.Students.FirstAsync(_ => _.Id == id);
            if (student == null) { return new NotFoundResult(); }
            var spsubjects = (from sp in student.StudyPlans select sp.SpSubjects).SelectMany(_ => _).ToList();
            var passed = (from sub in student.StudentGroups where sub.Grade >= 51 && sub.Status == 1 select sub.Group.Subject).ToList();
            List<Subject> available = new List<Subject>();
            foreach (var sp in spsubjects)
                if (sp.Subject.Prerequisites.All(_ => passed.Contains(_))) available.Add(sp.Subject);
            var res = (from a in available.Except(passed).Distinct().ToList() select a.WithGroups()).ToList();
            return res;
        }
    }
}
