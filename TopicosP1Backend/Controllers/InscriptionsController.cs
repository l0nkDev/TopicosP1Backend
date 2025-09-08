using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using CareerApi.Models;
using TopicosP1Backend.Scripts;

namespace TopicosP1Backend.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class InscriptionsController : ControllerBase
    {
        private readonly Context _context;

        public InscriptionsController(Context context)
        {
            _context = context;
        }

        // GET: api/Inscriptions
        [HttpGet]
        public async Task<ActionResult<IEnumerable<Inscription.InscriptionDTO>>> GetInscriptions()
        {
            List<Inscription> l = await _context.Inscriptions.IgnoreAutoIncludes().Include(_ => _.Student).Include(_ => _.Period).ThenInclude(_ => _.Gestion).Include(_ => _.Groups).ThenInclude(_ => _.Subject)
                .Include(_ => _.Student).Include(_ => _.Period).ThenInclude(_ => _.Gestion).Include(_ => _.Groups).ThenInclude(_ => _.Teacher)
                .ToListAsync();
            return (from i in l select i.Simple()).ToList();
        }

        // GET: api/Inscriptions/5
        [HttpGet("{id}")]
        public async Task<ActionResult<Inscription.InscriptionDTO>> GetInscription(long id)
        {
            var inscription = await _context.Inscriptions.IgnoreAutoIncludes().Include(_ => _.Student).Include(_ => _.Period).ThenInclude(_ => _.Gestion).Include(_ => _.Groups).ThenInclude(_ => _.Subject)
                .Include(_ => _.Student).Include(_ => _.Period).ThenInclude(_ => _.Gestion).Include(_ => _.Groups).ThenInclude(_ => _.Teacher)
                .Include(_ => _.Student).Include(_ => _.Period).ThenInclude(_ => _.Gestion).Include(_ => _.Groups).ThenInclude(_ => _.Period).ThenInclude(_ => _.Gestion)
                .FirstOrDefaultAsync(_=>_.Id == id);

            if (inscription == null)
            {
                return NotFound();
            }

            return inscription.Simple();
        }

        // PUT: api/Inscriptions/5
        // To protect from overposting attacks, see https://go.microsoft.com/fwlink/?linkid=2123754
        [HttpPut("{id}")]
        public async Task<ActionResult<Inscription.InscriptionDTO>> PutInscription(long id, Inscription.InscriptionPost i)
        {
            var inscription = await _context.Inscriptions.FindAsync(id);
            var student = await _context.Students.IgnoreAutoIncludes().FirstOrDefaultAsync(_=>_.Id == i.Student);
            var period = await _context.Periods.FirstOrDefaultAsync(_=>_.Number == i.Period && _.Gestion.Year == i.Gestion);
            if (inscription == null || student == null || period == null) return new NotFoundResult();
            if (id != inscription.Id) return new BadRequestResult();
            inscription.Student = student;
            inscription.Period = period;
            inscription.Type = i.Type;
            _context.Entry(inscription).State = EntityState.Modified;
            _context.SaveChanges();
            return inscription.Simple();
        }

        // POST: api/Inscriptions
        // To protect from overposting attacks, see https://go.microsoft.com/fwlink/?linkid=2123754
        [HttpPost]
        public async Task<ActionResult<Inscription>> PostInscription(Inscription.InscriptionPost i)
        {
            var student = await _context.Students.IgnoreAutoIncludes().FirstOrDefaultAsync(_ => _.Id == i.Student);
            var period = await _context.Periods.FirstOrDefaultAsync(_ => _.Number == i.Period && _.Gestion.Year == i.Gestion);
            Inscription n = new() { Student = student, Period = period, DateTime = DateTime.Now, Type = i.Type };
            _context.Inscriptions.Add(n);
            await _context.SaveChangesAsync();

            return CreatedAtAction("GetInscription", new { id = n.Id }, n.Simple());
        }

        // DELETE: api/Inscriptions/5
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteInscription(long id)
        {
            var inscription = await _context.Inscriptions.FindAsync(id);
            if (inscription == null)
            {
                return NotFound();
            }

            _context.Inscriptions.Remove(inscription);
            await _context.SaveChangesAsync();

            return NoContent();
        }

        // GET: api/Inscriptions/5
        [HttpGet("{id}/Groups")]
        public async Task<ActionResult<List<Group.GroupDTO>>> GetGroups(long id)
        {
            var inscription = await _context.Inscriptions.IgnoreAutoIncludes().Include(_ => _.Student).Include(_ => _.Period).ThenInclude(_ => _.Gestion).Include(_ => _.Groups).ThenInclude(_ => _.Subject)
                .Include(_ => _.Student).Include(_ => _.Period).ThenInclude(_ => _.Gestion).Include(_ => _.Groups).ThenInclude(_ => _.Teacher)
                .Include(_ => _.Student).Include(_ => _.Period).ThenInclude(_ => _.Gestion).Include(_ => _.Groups).ThenInclude(_ => _.Period).ThenInclude(_ => _.Gestion)
                .FirstOrDefaultAsync(_ => _.Id == id);
            if (inscription == null) return NotFound();
            return inscription.Simple().Groups.ToList();
        }

        // GET: api/Inscriptions/5
        [HttpPost("{id}/Groups")]
        public async Task<ActionResult<List<Group.GroupDTO>>> PostGroups(long id, Inscription.GIPost body)
        {
            var inscription = await _context.Inscriptions.FirstOrDefaultAsync(_ => _.Id == id);
            Group group = await _context.Groups.FindAsync(body.Group);
            if (inscription == null || group == null) return new NotFoundResult();
            GroupInscription gi = new() { Group = group, Inscription = inscription };
            await _context.GroupInscriptions.AddAsync(gi);
            await _context.SaveChangesAsync();
            return new OkResult();
        }

        // GET: api/Inscriptions/5
        [HttpDelete("{id}/Groups/{group}")]
        public async Task<ActionResult<List<Group.GroupDTO>>> DeleteGroups(long id, long group)
        {
            GroupInscription gi = await _context.GroupInscriptions.FirstOrDefaultAsync(_ => _.Group.Id == group && _.Inscription.Id == id);
            if (gi == null) return new NotFoundResult();
            _context.GroupInscriptions.Remove(gi);
            await _context.SaveChangesAsync();
            return new OkResult();
        }
    }
}
