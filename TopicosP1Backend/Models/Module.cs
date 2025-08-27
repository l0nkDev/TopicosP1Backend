using System.ComponentModel.DataAnnotations;
using System.Diagnostics.CodeAnalysis;

namespace CareerApi.Models
{
    public class Module
    {
        [Key]
        public long Number { get; set; }
    }
}
