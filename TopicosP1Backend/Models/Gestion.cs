using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.ComponentModel.DataAnnotations;
using System.Diagnostics.CodeAnalysis;
using TopicosP1Backend.Scripts;

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
        public static async Task<IEnumerable<GestionDTO>> GetAll(Context context)
        {
            var l = await context.Gestions.ToListAsync();
            return from a in l select a.Simple();
        }

        public static async Task<ActionResult<Gestion.GestionDTO>> Get(Context context, long id)
        {
            var gestion = await context.Gestions.FirstOrDefaultAsync(_ => _.Year == id);
            if (gestion == null) return new NotFoundResult();
            return gestion.Simple();
        }
        public static async Task<ActionResult<Gestion>> Post(Context _context, Gestion gestion)
        {
            _context.Gestions.Add(gestion);
            await _context.SaveChangesAsync();
            return gestion;
        }
    }
}
