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
        public class StudentPost
        {
            required public long Id { get; set; }
            required public string FirstName { get; set; }
            required public string LastName { get; set; }
        }
        public static async Task<ActionResult<IEnumerable<StudentDTO>>> GetStudents(Context _context)
        {
            var l = await _context.Students.IgnoreAutoIncludes().ToListAsync();
            return (from i in l select i.Simple()).ToList();
        }

        public static async Task<ActionResult<StudentDTO>> GetStudent(Context _context, long id)
        {
            var student = await _context.Students.IgnoreAutoIncludes().FirstOrDefaultAsync(_ => _.Id == id);
            if (student == null) return new NotFoundResult();
            return student.Simple();
        }

        [HttpPut("{id}")]
        public static async Task<ActionResult<StudentDTO>> PutStudent(Context _context, long id, StudentPost student)
        {
            Student s = await _context.Students.IgnoreAutoIncludes().FirstOrDefaultAsync(_ => _.Id == id);
            if (s == null) return new NotFoundResult();
            if (s.Id != student.Id) return new BadRequestResult();
            s.FirstName = student.FirstName;
            s.LastName = student.LastName;
            _context.Entry(s).State = EntityState.Modified;
            await _context.SaveChangesAsync();
            return s.Simple();
        }

        [HttpPost]
        public static async Task<ActionResult<StudentDTO>> PostStudent(Context _context, StudentPost student)
        {
            Student s = new() { FirstName = student.FirstName, LastName = student.LastName };
            _context.Students.Add(s);
            await _context.SaveChangesAsync();

            return s.Simple();
        }

        [HttpDelete("{id}")]
        public static async Task<IActionResult> DeleteStudent(Context _context, long id)
        {
            var student = await _context.Students.IgnoreAutoIncludes().FirstOrDefaultAsync(_ => _.Id == id);
            if (student == null) return new NotFoundResult();
            _context.Students.Remove(student);
            await _context.SaveChangesAsync();
            return new NoContentResult();
        }

        public static async Task<ActionResult<List<StudentGroups.HistoryEntry>>> History(Context context, long id)
        {
            var student = await context.Students.IgnoreAutoIncludes().AsSplitQuery()
                .Include(_ => _.StudentGroups).ThenInclude(_ => _.Group).ThenInclude(_ => _.Subject)
                .Include(_ => _.StudyPlans).ThenInclude(_ => _.SpSubjects).ThenInclude(_ => _.Subject).ThenInclude(_ => _.Prerequisites)
                .FirstOrDefaultAsync(_ => _.Id == id);
            if (student == null) { return new NotFoundResult(); }
            var history = await context.StudentGroups.Where(_ => _.Student.Id == id && _.Status != 2).ToListAsync();
            List<StudentGroups.HistoryEntry> res = [.. (from a in history select a.Simple())];
            return res;
        }

        public static async Task<ActionResult<List<Subject.SubjectWithGroups>>> Available(Context context, long id)
        {
            var student = await context.Students.IgnoreAutoIncludes().AsSplitQuery()
                .Include(_ => _.StudentGroups).ThenInclude(_ => _.Group).ThenInclude(_ => _.Subject)
                .Include(_ => _.StudyPlans).ThenInclude(_ => _.SpSubjects).ThenInclude(_ => _.Subject).ThenInclude(_ => _.Prerequisites)
                .Include(_ => _.StudyPlans).ThenInclude(_ => _.SpSubjects).ThenInclude(_ => _.Subject).ThenInclude(_ => _.Groups).ThenInclude(_ => _.Period).ThenInclude(_ => _.Gestion)
                .Include(_ => _.StudyPlans).ThenInclude(_ => _.SpSubjects).ThenInclude(_ => _.Subject).ThenInclude(_ => _.Groups).ThenInclude(_ => _.Teacher)
                .FirstOrDefaultAsync(_ => _.Id == id);
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
