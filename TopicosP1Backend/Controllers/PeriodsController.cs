using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using CareerApi.Models;
using TopicosP1Backend.Scripts;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using System.Text.Json;

namespace TopicosP1Backend.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class PeriodsController : ControllerBase
    {
        private readonly Context _context;
        private readonly APIQueue _queue;

        public PeriodsController(Context context, APIQueue queue)
        {
            _context = context;
            _queue = queue;
        }

        // GET: api/Periods
        [HttpGet]
        public object GetPeriods()
        {
            return _queue.Request(Function.GetPeriods, [], "", "GetPeriods", true);
        }

        // GET: api/Periods/5
        [HttpGet("{id}")]
        public object GetPeriod(long id)
        {
            return _queue.Request(Function.GetPeriod, [$"{id}"], "", $"GetPeriod {id}", true);
        }

        // PUT: api/Periods/5
        // To protect from overposting attacks, see https://go.microsoft.com/fwlink/?linkid=2123754
        [HttpPut("{id}")]
        public async Task<IActionResult> PutPeriod(long id, Period.PeriodDTO period)
        {
            Period? p = await _context.Periods.FindAsync(id);
            if (p == null) return BadRequest();
            p.Number = period.Number; p.Gestion = await _context.Gestions.FindAsync(period.Gestion);

            if (id != p.Id)
            {
                return BadRequest();
            }
            _context.Entry(p).State = EntityState.Modified;

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
        public object PostPeriod(Period.PostDTO period)
        {
            string b = JsonSerializer.Serialize(period);
            return _queue.Request(Function.PostPeriod, [], b, $"PostPeriod {b}");
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
