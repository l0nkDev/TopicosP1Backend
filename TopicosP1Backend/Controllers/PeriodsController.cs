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
    public class PeriodsController : ControllerBase
    {
        private readonly Context _context;

        public PeriodsController(Context context)
        {
            _context = context;
        }

        // GET: api/Periods
        [HttpGet]
        public async Task<ActionResult<IEnumerable<Period>>> GetPeriods()
        {
            return await _context.Periods.ToListAsync();
        }

        // GET: api/Periods/5
        [HttpGet("{id}")]
        public async Task<ActionResult<Period>> GetPeriod(long id)
        {
            var period = await _context.Periods.FindAsync(id);

            if (period == null)
            {
                return NotFound();
            }

            return period;
        }

        // PUT: api/Periods/5
        // To protect from overposting attacks, see https://go.microsoft.com/fwlink/?linkid=2123754
        [HttpPut("{id}")]
        public async Task<IActionResult> PutPeriod(long id, Period period)
        {
            if (id != period.Id)
            {
                return BadRequest();
            }

            _context.Entry(period).State = EntityState.Modified;

            try
            {
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!PeriodExists(id))
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

        // POST: api/Periods
        // To protect from overposting attacks, see https://go.microsoft.com/fwlink/?linkid=2123754
        [HttpPost]
        public async Task<ActionResult<Period>> PostPeriod(Period period)
        {
            _context.Periods.Add(period);
            await _context.SaveChangesAsync();

            return CreatedAtAction("GetPeriod", new { id = period.Id }, period);
        }

        // DELETE: api/Periods/5
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeletePeriod(long id)
        {
            var period = await _context.Periods.FindAsync(id);
            if (period == null)
            {
                return NotFound();
            }

            _context.Periods.Remove(period);
            await _context.SaveChangesAsync();

            return NoContent();
        }

        private bool PeriodExists(long id)
        {
            return _context.Periods.Any(e => e.Id == id);
        }
    }
}
