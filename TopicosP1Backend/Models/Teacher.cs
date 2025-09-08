using System.ComponentModel.DataAnnotations;
using System.Diagnostics.CodeAnalysis;

namespace CareerApi.Models
{
    public class Teacher
    {
        public long Id { get; set; }
        required public string FirstName { get; set; }
        required public string LastName { get; set; }
        public IEnumerable<Group> Groups { get; set; } = new List<Group>();
        public TeacherDTO Simple() => new(this);

        public class TeacherDTO(Teacher teacher)
        {
            public long Id { get; set; } = teacher.Id;
            public string FirstName { get; set; } = teacher.FirstName;
            public string LastName { get; set; } = teacher.LastName;
        }
    }
}
