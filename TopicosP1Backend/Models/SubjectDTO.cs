using System.ComponentModel.DataAnnotations;

namespace CareerApi.Models
{
    public class SubjectDTO
    {
        [Key]
        required public string Code { get; set; }
        required public string Title { get; set; }
        public ICollection<SubjectDTO2> Prerequisites { get; set;  } = new List<SubjectDTO2>();
    }
}
