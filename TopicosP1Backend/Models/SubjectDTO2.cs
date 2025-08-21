using System.ComponentModel.DataAnnotations;

namespace CareerApi.Models
{
    public class SubjectDTO2
    {
        [Key]
        required public string Code { get; set; }
        required public string Title { get; set; }
    }
}
