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
            public Subject.SubjectSimple Subject { get; set; } = group.Subject.Simple();
            public string Code { get; set; } = group.Code;
            public string Mode { get; set; } = group.Mode;
            public long Quota { get; set; } = group.Quota;
            public Period.PeriodDTO Period { get; set; } = group.Period.Simple();
            public Teacher.TeacherDTO Teacher { get; set; } = group.Teacher.Simple();
        }
        public class PostGroup
        {
            required public long Id { get; set; }
            required public string Subject { get; set; }
            required public string Code { get; set; }
            required public string Mode { get; set; }
            required public long Quota { get; set; }
            required public long Period { get; set; }
            required public long Gestion { get; set; }
            required public long Teacher { get; set; }
        }
    }
}
