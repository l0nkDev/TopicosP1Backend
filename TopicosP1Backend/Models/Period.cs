using System.Diagnostics.CodeAnalysis;

namespace CareerApi.Models
{
    public class Period
    {
        public long Id { get; set; }
        required public long Number { get; set; }
        required public Gestion Gestion { get; set; }
        public PeriodDTO Simple() => new(this);

        public class PeriodDTO(Period period)
        {
            public long Id { get; set; } = period.Id;
            public long Number { get; set; } = period.Number;
        }
    }
}
