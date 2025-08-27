using Microsoft.EntityFrameworkCore;
using System.ComponentModel.DataAnnotations;

namespace CareerApi.Models
{
    public class Subject
    {
        [Key]
        required public string Code { get; set; }
        required public string Title { get; set; }
        public IEnumerable<SpSubject> SpSubjects { get; set; } = new List<SpSubject>();
        public IEnumerable<StudyPlan> StudyPlans{ get; set; } = new List<StudyPlan>();
    }
}
