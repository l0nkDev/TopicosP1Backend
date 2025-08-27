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
        public IEnumerable<SubjectDTO2> Prerequisites { get; set; } = new List<SubjectDTO2>();

        [SetsRequiredMembers]
        public SubjectDTO(Subject subject)
        {
            Code = subject.Code;
            Title = subject.Title;
            Prerequisites = from a in subject.Prerequisites select new SubjectDTO2(a);
        }

        [SetsRequiredMembers]
        public SubjectDTO(Subject subject, StudyPlan studyPlan)
        {
            Code = subject.Code;
            Title = subject.Title;
            Prerequisites = from a in subject.PostDependencies where a.StudyPlan == studyPlan select new SubjectDTO2(a.Prerequisite);
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
