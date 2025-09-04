using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using CareerApi.Models;
using TopicosP1Backend.Scripts;
using System.Collections;
using System.Text.Json;

namespace TopicosP1Backend.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class GestionsController : ControllerBase
    {
        private readonly Context _context;
        private readonly APIQueue _queue;

        public GestionsController(Context context, APIQueue queue)
        {
            _context = context;
            _queue = queue;
        }

        // GET: api/Gestions
        [HttpGet]
        public async Task<object> GetGestions()
        {
            return _queue.Request(Function.GetGestions, [], "", $"GetGestions", true);
        }

        // GET: api/Gestions/5
        [HttpGet("{id}")]
        public async Task<object> GetGestion(long id)
        {
            return _queue.Request(Function.GetGestion, [id.ToString()], "", $"GetGestion {id}", true);
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
        public object PostGestion(Gestion gestion)
        {
            string b = JsonSerializer.Serialize(gestion);
            return _queue.Request(Function.PostGestion, [], b, $"PostGestion {b}");
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
