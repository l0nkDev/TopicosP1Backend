using System.ComponentModel.DataAnnotations;
using System.Diagnostics.CodeAnalysis;

namespace CareerApi.Models
{
    public class StudyPlan
    {
        [Key]
        required public string Code { get; set; }
        public Career Career { get; set; } = null!;
        public IEnumerable<SpSubject> SpSubjects { get; set; } = new List<SpSubject>();
        public IEnumerable<Subject> Subjects { get; set; } = new List<Subject>();
    }
    public class StudyPlanDTO
    {
        [Key]
        required public string Code { get; set; }
        required public string Career { get; set; }
        required public IEnumerable<SubjectDTO> Subjects { get; set; }

        [SetsRequiredMembers]
        public StudyPlanDTO(StudyPlan studyPlan)
        {
            Code = studyPlan.Code;
            Career = studyPlan.Career.Name;
            Subjects = from a in studyPlan.SpSubjects select new SubjectDTO(a);
        }
    }
}
