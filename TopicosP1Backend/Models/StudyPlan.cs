using System.ComponentModel.DataAnnotations;
using System.Diagnostics.CodeAnalysis;
using static CareerApi.Models.Subject;

namespace CareerApi.Models
{
    public class StudyPlan
    {
        [Key]
        required public string Code { get; set; }
        public Career Career { get; set; } = null!;
        public IEnumerable<SpSubject> SpSubjects { get; set; } = new List<SpSubject>();
        public IEnumerable<Subject> Subjects { get; set; } = new List<Subject>();
        public IEnumerable<Student> Students { get; set; } = new List<Student>();
        public StudyPlanDTO Simple() => new(this);

        public class StudyPlanDTO(StudyPlan studyPlan)
        {
            public string Code { get; set; } = studyPlan.Code;
            public string Career { get; set; } = studyPlan.Career.Name;
            public IEnumerable<SpSubject.SpSubjectDTO> Subjects { get; set; } = 
                from a in studyPlan.SpSubjects select a.SimpleList();
        }
    }
}
