using System.ComponentModel.DataAnnotations;
using System.Diagnostics.CodeAnalysis;

namespace CareerApi.Models
{
    public class Gestion
    {
        [Key]
        public long Year { get; set; }
        public IEnumerable<Period> Periods { get; } = new List<Period>();
        public GestionDTO Simple() => new(this);
        public YearOnly YearItem() => new(this);

        public class GestionDTO(Gestion gestion) 
        {
            public long Year { get; set; } = gestion.Year;
            public IEnumerable<Period.YearlessPeriod> Periods { get; } =
                from a in gestion.Periods select a.Yearless();
        }
        public class YearOnly(Gestion gestion)
        {
            public long Year { get; set; } = gestion.Year;
        }
    }

}
