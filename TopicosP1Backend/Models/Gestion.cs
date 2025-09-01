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
        
        public class GestionDTO(Gestion gestion) 
        {
            public long Year { get; set; } = gestion.Year;
            public IEnumerable<Period.PeriodDTO> Periods { get; } =
                from a in gestion.Periods select a.Simple();
        }
    }

}
