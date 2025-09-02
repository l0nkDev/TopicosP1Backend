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
        public IEnumerable<Group> Groups { get; set; } = new List<Group>();
        public SubjectDTO SimpleList() => new(this);
        public SubjectSimple Simple() => new(this);
        public SubjectWithGroups WithGroups() => new(this);
        public class SubjectDTO(Subject subject)
        {
            public string Code { get; set; } = subject.Code;
            public string Title { get; set; } = subject.Title;
            public IEnumerable<SubjectSimple> Prerequisites { get; set; } = from a in subject.PreDependencies select a.Prerequisite.Simple();
            public IEnumerable<SubjectSimple> Postrequisites { get; set; } = from a in subject.PostDependencies select a.Prerequisite.Simple();
        }

        public class SubjectSimple(Subject subject)
        {
            public string Code { get; set; } = subject.Code;
            public string Title { get; set; } = subject.Title;
        }

        public class SubjectWithGroups(Subject subject)
        {
            public string Code { get; set; } = subject.Code;
            public string Title { get; set; } = subject.Title;
            public IEnumerable<Group.GroupDTO> Groups { get; set; } = from a in subject.Groups where a.Quota > 0 && a.Period.Id == 42 select a.Simple();

        }
    }
}
