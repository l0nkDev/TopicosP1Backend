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
            List<Inscription> l = await _context.Inscriptions.IgnoreAutoIncludes().Include(_=>_.Student).Include(_=>_.Period).ThenInclude(_=>_.Gestion).Include(_=>_.Groups).ThenInclude(_=>_.Subject).ToListAsync();
            return (from i in l select i.Simple()).ToList();
        }

        // GET: api/Inscriptions/5
        [HttpGet("{id}")]
        public async Task<ActionResult<Inscription>> GetInscription(long id)
        {
            var inscription = await _context.Inscriptions.FindAsync(id);

            if (inscription == null)
            {
                return NotFound();
            }

            return inscription;
        }

        // PUT: api/Inscriptions/5
        // To protect from overposting attacks, see https://go.microsoft.com/fwlink/?linkid=2123754
        [HttpPut("{id}")]
        public async Task<IActionResult> PutInscription(long id, Inscription inscription)
        {
            if (id != inscription.Id)
            {
                return BadRequest();
            }

            _context.Entry(inscription).State = EntityState.Modified;

            try
            {
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!InscriptionExists(id))
                {
                    return NotFound();
                }
                else
                {
                    throw;
                }
            }

            return NoContent();
        }

        // POST: api/Inscriptions
        // To protect from overposting attacks, see https://go.microsoft.com/fwlink/?linkid=2123754
        [HttpPost]
        public async Task<ActionResult<Inscription>> PostInscription(Inscription inscription)
        {
            _context.Inscriptions.Add(inscription);
            await _context.SaveChangesAsync();

            return CreatedAtAction("GetInscription", new { id = inscription.Id }, inscription);
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

        private bool InscriptionExists(long id)
        {
            return _context.Inscriptions.Any(e => e.Id == id);
        }
    }
}
