using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using CareerApi.Models;
using static CareerApi.Models.Career;
using TopicosP1Backend.Scripts;

namespace TopicosP1Backend.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class CareersController : ControllerBase
    {
        private readonly Context _context;

        public CareersController(Context context)
        {
            _context = context;
        }

        [HttpGet]
        public async Task<IEnumerable<CareerDTO>> GetCareers()
        {
            var careers = await _context.Careers.ToListAsync();
            return from a in careers select a.SimpleList();
        }

        [HttpGet("{id}")]
        public async Task<ActionResult<CareerDTO>> GetCareer(long id)
        {
            var career = await _context.Careers.FindAsync(id);
            if (career == null) return NotFound();
            return career.SimpleList();
        }

        [HttpPut("{id}")]
        public async Task<IActionResult> PutCareer(long id, PostCareer c)
        {
            if (id != c.Id) return new BadRequestResult();
            Career subject = await _context.Careers.FindAsync(id);
            subject.Name = c.Name;
            _context.Entry(subject).State = EntityState.Modified;

            try { await _context.SaveChangesAsync(); }
            catch (DbUpdateConcurrencyException)
            {
                if (!_context.Careers.Any(e => e.Id == id)) return new NotFoundResult();
                else throw;
            }
            return new NoContentResult();
        }

        // POST: api/Careers
        // To protect from overposting attacks, see https://go.microsoft.com/fwlink/?linkid=2123754
        [HttpPost]
        public async Task<ActionResult<Career>> PostCareer(PostCareer c)
        {
            Career career = new Career { Name = c.Name };
            _context.Careers.Add(career);
            await _context.SaveChangesAsync();
            return CreatedAtAction(nameof(GetCareer), new { id = career.Id }, career);
        }

        // DELETE: api/Careers/5
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteCareer(long id)
        {
            var career = await _context.Careers.FindAsync(id);
            if (career == null) return new NotFoundResult();
            _context.Careers.Remove(career);
            await _context.SaveChangesAsync();
            return new NoContentResult();
        }
    }
}
