using System.ComponentModel.DataAnnotations;

namespace CareerApi.Models
{
    public class Subject
    {
        [Key]
        required public string Code { get; set; }
        required public string Title { get; set; }
        public ICollection<Subject> Prerequisites { get; } = new List<Subject>();
        public ICollection<Subject> Postrequisites { get; } = new List<Subject>();
    }
}
