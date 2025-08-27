using System.Diagnostics.CodeAnalysis;

namespace CareerApi.Models
{
    public class Room
    {
        public long Id { get; set; }
        required public Module Module { get; set; }
        required public long NUmber { get; set; }
    }
}
