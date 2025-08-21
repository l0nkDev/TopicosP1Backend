using System.ComponentModel.DataAnnotations;

namespace CareerApi.Models
{
    public class StudyPlan
    {
        [Key]
        required public string Code { get; set; }
        public Career Career { get; set; } = null!;
    }
}
