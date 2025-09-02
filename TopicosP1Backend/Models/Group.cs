using System.Diagnostics.CodeAnalysis;

namespace CareerApi.Models
{
    public class Group
    {
        public long Id { get; set; }
        required public string Code { get; set; }
        required public string Mode { get; set; }
        required public Period Periodo { get; set; }
        required public Subject Subject { get; set; }
        required public Teacher Teacher { get; set; }
        public long Quota { get; set; }
        public IEnumerable<TimeSlot> TimeSlots { get; set; } = new List<TimeSlot>();
        public IEnumerable<GroupInscription> GroupInscriptions { get; set; } = new List<GroupInscription>();
        public IEnumerable<Inscription> Inscriptions { get; set; } = new List<Inscription>();
        public IEnumerable<StudentGroups> StudentGroups { get; set; } = new List<StudentGroups>();
        public IEnumerable<Student> Students { get; set; } = new List<Student>();
    }
}
