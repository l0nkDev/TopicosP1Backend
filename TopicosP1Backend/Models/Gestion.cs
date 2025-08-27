using System.ComponentModel.DataAnnotations;
using System.Diagnostics.CodeAnalysis;

namespace CareerApi.Models
{
    public class Gestion
    {
        [Key]
        public long Year { get; set; }
        public IEnumerable<Period> Periods { get; } = new List<Period>();
    }
}
