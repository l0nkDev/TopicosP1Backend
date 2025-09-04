using System.ComponentModel.DataAnnotations.Schema;
using System.Diagnostics.CodeAnalysis;
using System.Security.Cryptography;
using System.Text.Json.Serialization;

namespace CareerApi.Models
{
    public class Period
    {
        public long Id { get; set; }
        required public long Number { get; set; }
        required public Gestion Gestion { get; set; }
        public PeriodDTO Simple() => new(this);
        public YearlessPeriod Yearless() => new(this);

        public class PeriodDTO(Period period)
        {
            public long Id { get; set; } = period.Id;
            public long Number { get; set; } = period.Number;
            public Gestion.YearOnly Gestion { get; set; } = period.Gestion.YearItem();
        }

        public class PostDTO
        {
            public long Id { get; set; }
            required public long Number { get; set; }
            required public long Gestion { get; set; }
        }

        public class YearlessPeriod(Period period)
        {
            public long Id { get; set; } = period.Id;
            public long Number { get; set; } = period.Number;
        }
    }
}
