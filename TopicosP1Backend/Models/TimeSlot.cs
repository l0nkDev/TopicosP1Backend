using System.Diagnostics.CodeAnalysis;

namespace CareerApi.Models
{
    public class TimeSlot
    {
        public long Id { get; set; }
        required public string Day { get; set; }
        required public TimeOnly StartTime { get; set; }
        required public TimeOnly EndTime { get; set; }
        required public Room Room { get; set; }
        required public Gestion Gestion { get; set; }
    }
}
