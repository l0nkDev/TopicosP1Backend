using System.ComponentModel.DataAnnotations;

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
}
