using System.ComponentModel.DataAnnotations;

namespace CareerApi.Models
{
    public class StudyPlanDTO
    {
        [Key]
        required public string Code { get; set; }
        required public string Career { get; set; }
        required public List<SubjectDTO> Subjects { get; set; }
    }
}
