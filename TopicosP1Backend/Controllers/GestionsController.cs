using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using CareerApi.Models;

namespace TopicosP1Backend.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class GestionsController : ControllerBase
    {
        private readonly Context _context;

        public GestionsController(Context context)
        {
            _context = context;
        }

        // GET: api/Gestions
        [HttpGet]
        public async Task<ActionResult<IEnumerable<Gestion>>> GetGestions()
        {
            return await _context.Gestions.ToListAsync();
        }

        // GET: api/Gestions/5
        [HttpGet("{id}")]
        public async Task<ActionResult<Gestion>> GetGestion(long id)
        {
            var gestion = await _context.Gestions.FindAsync(id);

            if (gestion == null)
            {
                return NotFound();
            }

            return gestion;
        }

        // PUT: api/Gestions/5
        // To protect from overposting attacks, see https://go.microsoft.com/fwlink/?linkid=2123754
        [HttpPut("{id}")]
        public async Task<IActionResult> PutGestion(long id, Gestion gestion)
        {
            if (id != gestion.Year)
            {
                return BadRequest();
            }

            _context.Entry(gestion).State = EntityState.Modified;

            try
            {
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!GestionExists(id))
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

        // POST: api/Gestions
        // To protect from overposting attacks, see https://go.microsoft.com/fwlink/?linkid=2123754
        [HttpPost]
        public async Task<ActionResult<Gestion>> PostGestion(Gestion gestion)
        {
            _context.Gestions.Add(gestion);
            await _context.SaveChangesAsync();

            return CreatedAtAction("GetGestion", new { id = gestion.Year }, gestion);
        }

        // DELETE: api/Gestions/5
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteGestion(long id)
        {
            var gestion = await _context.Gestions.FindAsync(id);
            if (gestion == null)
            {
                return NotFound();
            }

            _context.Gestions.Remove(gestion);
            await _context.SaveChangesAsync();

            return NoContent();
        }

        private bool GestionExists(long id)
        {
            return _context.Gestions.Any(e => e.Year == id);
        }
    }
}
