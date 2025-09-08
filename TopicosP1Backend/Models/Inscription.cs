using System.ComponentModel.DataAnnotations;
using System.Diagnostics.CodeAnalysis;

namespace CareerApi.Models
{
    public class Inscription
    {
        public long Id { get; set; }
        public IEnumerable<GroupInscription> GroupInscriptions { get; set; } = new List<GroupInscription>();
        public IEnumerable<Group> Groups { get; set; } = new List<Group>();
        required public Student Student { get; set; }
        required public Period Period { get; set; }
        public DateTime DateTime { get; set; } = DateTime.Now;
        required public int Type { get; set; } //0 = Inscripcion, 1 = Adicion, 2 = Retiro
        public InscriptionDTO Simple() => new(this);
        public class InscriptionDTO(Inscription ins)
        {
            public long Id { get; set; } = ins.Id;
            public Student.StudentDTO Student { get; set; } = ins.Student.Simple();
            public Period.PeriodDTO Period { get; set; } = ins.Period.Simple();
            public DateTime DateTime { get; set; } = ins.DateTime;
            public int Type { get; set; } = ins.Type;
            public List<Group.GroupDTO> Groups { get; set; } = (from i in ins.Groups select i.Simple()).ToList();
        }

        public class InscriptionPost
        {
            public long Id { get; set; }
            public long Student { get; set; }
            public long Period { get; set; }
            public long Gestion { get; set; }
            public int Type { get; set; }

        }
        public class GIPost
        {
            required public long Group { get; set; }
        }
    }
}
