using CareerApi.Models;
using Microsoft.EntityFrameworkCore;
using System.ComponentModel.DataAnnotations;
using System.Diagnostics.CodeAnalysis;

namespace CareerApi.Models
{
    public class Subject
    {
        [Key]
        required public string Code { get; set; }
        required public string Title { get; set; }
        public IEnumerable<SpSubject> SpSubjects { get; set; } = new List<SpSubject>();
        public IEnumerable<StudyPlan> StudyPlans{ get; set; } = new List<StudyPlan>();
        public IEnumerable<SubjectDependency> PreDependencies { get; set; } = new List<SubjectDependency>();
        public IEnumerable<Subject> Prerequisites { get; set; } = new List<Subject>();
        public IEnumerable<SubjectDependency> PostDependencies { get; set; } = new List<SubjectDependency>();
        public IEnumerable<Subject> Postrequisites { get; set; } = new List<Subject>();
    }
    public class SubjectDTO
    {
        [Key]
        required public string Code { get; set; }
        required public string Title { get; set; }
        required public int Credits { get; set; }
        required public int Level { get; set; }
        required public string Type { get; set; }
        public IEnumerable<SubjectDTO2> Prerequisites { get; set; } = new List<SubjectDTO2>();

        [SetsRequiredMembers]
        public SubjectDTO(SpSubject spSubject)
        {
            Code = spSubject.Subject.Code;
            Title = spSubject.Subject.Title;
            Credits = spSubject.Credits;
            Level = spSubject.Level;
            Type = spSubject.Type switch { 0 => "Exclusive", 1 => "Shared", 2 => "Optative", 3 => "Elective", 4 => "Degree", _ => "Undefined" };
            Prerequisites = from a in spSubject.Subject.PreDependencies where a.StudyPlan == spSubject.StudyPlan select new SubjectDTO2(a.Prerequisite);
        }
    }
   
    public class SubjectDTO2
    {
        [Key]
        required public string Code { get; set; }
        required public string Title { get; set; }

        [SetsRequiredMembers]
        public SubjectDTO2(SubjectDTO subject) { Code = subject.Code; Title = subject.Title; }

        [SetsRequiredMembers]
        public SubjectDTO2(Subject subject) { Code = subject.Code; Title = subject.Title; }
    }
}
