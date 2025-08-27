using System.Diagnostics.CodeAnalysis;

namespace CareerApi.Models
{
    public class Period
    {
        public long Id { get; set; }
        required public long Number { get; set; }
        required public Gestion Gestion { get; set; }
    }
}
