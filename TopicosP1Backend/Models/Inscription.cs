using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.ComponentModel.DataAnnotations;
using System.Diagnostics.CodeAnalysis;
using TopicosP1Backend.Scripts;

namespace CareerApi.Models
{
    public class Inscription
    {
        public long Id { get; set; }
        public IEnumerable<GroupInscription> GroupInscriptions { get; set; } = new List<GroupInscription>();
        public IEnumerable<Group> Groups { get; set; } = new List<Group>();
        required public Student Student { get; set; }
        required public Period Period { get; set; }
        public DateTime DateTime { get; set; } = DateTime.Now;
        required public int Type { get; set; } //0 = Inscripcion, 1 = Adicion, 2 = Retiro
        public InscriptionDTO Simple() => new(this);
        public class InscriptionDTO(Inscription ins)
        {
            public long Id { get; set; } = ins.Id;
            public Student.StudentDTO Student { get; set; } = ins.Student.Simple();
            public Period.PeriodDTO Period { get; set; } = ins.Period.Simple();
            public DateTime DateTime { get; set; } = ins.DateTime;
            public int Type { get; set; } = ins.Type;
            public List<Group.GroupDTO> Groups { get; set; } = (from i in ins.Groups select i.Simple()).ToList();
        }

        public class InscriptionPost
        {
            public long Id { get; set; }
            public long Student { get; set; }
            public long Period { get; set; }
            public long Gestion { get; set; }
            public int Type { get; set; }

        }
        public class GIPost
        {
            required public long Group { get; set; }
        }

        public static async Task<ActionResult<IEnumerable<InscriptionDTO>>> GetInscriptions(Context _context)
        {
            List<Inscription> l = await _context.Inscriptions.IgnoreAutoIncludes().Include(_ => _.Student).Include(_ => _.Period).ThenInclude(_ => _.Gestion).Include(_ => _.Groups).ThenInclude(_ => _.Subject)
                .Include(_ => _.Student).Include(_ => _.Period).ThenInclude(_ => _.Gestion).Include(_ => _.Groups).ThenInclude(_ => _.Teacher)
                .ToListAsync();
            return (from i in l select i.Simple()).ToList();
        }

        public static async Task<ActionResult<InscriptionDTO>> GetInscription(Context _context, long id)
        {
            var inscription = await _context.Inscriptions.IgnoreAutoIncludes().Include(_ => _.Student).Include(_ => _.Period).ThenInclude(_ => _.Gestion).Include(_ => _.Groups).ThenInclude(_ => _.Subject)
                .Include(_ => _.Student).Include(_ => _.Period).ThenInclude(_ => _.Gestion).Include(_ => _.Groups).ThenInclude(_ => _.Teacher)
                .Include(_ => _.Student).Include(_ => _.Period).ThenInclude(_ => _.Gestion).Include(_ => _.Groups).ThenInclude(_ => _.Period).ThenInclude(_ => _.Gestion)
                .FirstOrDefaultAsync(_ => _.Id == id);

            if (inscription == null)
            {
                return new NotFoundResult();
            }

            return inscription.Simple();
        }

        public static async Task<ActionResult<InscriptionDTO>> PutInscription(Context _context, long id, InscriptionPost i)
        {
            var inscription = await _context.Inscriptions.FindAsync(id);
            var student = await _context.Students.IgnoreAutoIncludes().FirstOrDefaultAsync(_ => _.Id == i.Student);
            var period = await _context.Periods.FirstOrDefaultAsync(_ => _.Number == i.Period && _.Gestion.Year == i.Gestion);
            if (inscription == null || student == null || period == null) return new NotFoundResult();
            if (id != inscription.Id) return new BadRequestResult();
            inscription.Student = student;
            inscription.Period = period;
            inscription.Type = i.Type;
            _context.Entry(inscription).State = EntityState.Modified;
            _context.SaveChanges();
            return inscription.Simple();
        }

        public static async Task<ActionResult<InscriptionDTO>> PostInscription(Context _context, InscriptionPost i)
        {
            var student = await _context.Students.IgnoreAutoIncludes().FirstOrDefaultAsync(_ => _.Id == i.Student);
            var period = await _context.Periods.FirstOrDefaultAsync(_ => _.Number == i.Period && _.Gestion.Year == i.Gestion);
            Inscription n = new() { Student = student, Period = period, DateTime = DateTime.Now, Type = i.Type };
            _context.Inscriptions.Add(n);
            await _context.SaveChangesAsync();

            return n.Simple();
        }

        public static async Task<IActionResult> DeleteInscription(Context _context, long id)
        {
            var inscription = await _context.Inscriptions.FindAsync(id);
            if (inscription == null)
            {
                return new NotFoundResult();
            }

            _context.Inscriptions.Remove(inscription);
            await _context.SaveChangesAsync();

            return new NoContentResult();
        }

        public static async Task<ActionResult<List<Group.GroupDTO>>> GetInsGroups(Context _context, long id)
        {
            var inscription = await _context.Inscriptions.IgnoreAutoIncludes().Include(_ => _.Student).Include(_ => _.Period).ThenInclude(_ => _.Gestion).Include(_ => _.Groups).ThenInclude(_ => _.Subject)
                .Include(_ => _.Student).Include(_ => _.Period).ThenInclude(_ => _.Gestion).Include(_ => _.Groups).ThenInclude(_ => _.Teacher)
                .Include(_ => _.Student).Include(_ => _.Period).ThenInclude(_ => _.Gestion).Include(_ => _.Groups).ThenInclude(_ => _.Period).ThenInclude(_ => _.Gestion)
                .FirstOrDefaultAsync(_ => _.Id == id);
            if (inscription == null) return new NotFoundResult();
            return inscription.Simple().Groups.ToList();
        }

        public static async Task<ActionResult<List<Group.GroupDTO>>> PostInsGroups(Context _context, long id, GIPost body)
        {
            var inscription = await _context.Inscriptions.FirstOrDefaultAsync(_ => _.Id == id);
            Group group = await _context.Groups.FindAsync(body.Group);
            if (inscription == null || group == null) return new NotFoundResult();
            GroupInscription gi = new() { Group = group, Inscription = inscription };
            await _context.GroupInscriptions.AddAsync(gi);
            await _context.SaveChangesAsync();
            return new OkResult();
        }

        public static async Task<ActionResult<List<Group.GroupDTO>>> DeleteInsGroups(Context _context, long id, long group)
        {
            GroupInscription gi = await _context.GroupInscriptions.FirstOrDefaultAsync(_ => _.Group.Id == group && _.Inscription.Id == id);
            if (gi == null) return new NotFoundResult();
            _context.GroupInscriptions.Remove(gi);
            await _context.SaveChangesAsync();
            return new OkResult();
        }
    }
}
