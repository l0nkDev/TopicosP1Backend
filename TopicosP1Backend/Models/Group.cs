using System.Diagnostics.CodeAnalysis;

namespace CareerApi.Models
{
    public class Group
    {
        public long Id { get; set; }
        required public string Code { get; set; }
        required public string Mode { get; set; }
        required public Period Period { get; set; }
        required public Subject Subject { get; set; }
        required public Teacher Teacher { get; set; }
        public long Quota { get; set; }
        public IEnumerable<TimeSlot> TimeSlots { get; set; } = new List<TimeSlot>();
        public IEnumerable<GroupInscription> GroupInscriptions { get; set; } = new List<GroupInscription>();
        public IEnumerable<Inscription> Inscriptions { get; set; } = new List<Inscription>();
        public IEnumerable<StudentGroups> StudentGroups { get; set; } = new List<StudentGroups>();
        public IEnumerable<Student> Students { get; set; } = new List<Student>();
        public GroupDTO Simple() => new(this);

        public class GroupDTO(Group group)
        {
            public long Id { get; set; } = group.Id;
            public string Code { get; set; } = group.Code;
            public string Mode { get; set; } = group.Mode;
            public long Quota { get; set; } = group.Quota;
            public string Period { get; set; } = group.Period.Number.ToString() + "-" + group.Period.Gestion.Year.ToString();
            public string Teacher { get; set; } = group.Teacher.FirstName + " " + group.Teacher.LastName;
        }
    }
}
