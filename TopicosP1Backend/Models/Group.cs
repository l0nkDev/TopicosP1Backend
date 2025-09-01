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
        public IEnumerable<TimeSlot> TimeSlots { get; set; } = new List<TimeSlot>();
        public IEnumerable<Inscription> Inscriptions { get; set; } = new List<Inscription>();
    }
}
