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
    public class GroupInscriptionsController : ControllerBase
    {
        private readonly Context _context;

        public GroupInscriptionsController(Context context)
        {
            _context = context;
        }

        // GET: api/GroupInscriptions
        [HttpGet]
        public async Task<ActionResult<IEnumerable<GroupInscription>>> GetGroupInscriptions()
        {
            return await _context.GroupInscriptions.ToListAsync();
        }

        // GET: api/GroupInscriptions/5
        [HttpGet("{id}")]
        public async Task<ActionResult<GroupInscription>> GetGroupInscription(long id)
        {
            var groupInscription = await _context.GroupInscriptions.FindAsync(id);

            if (groupInscription == null)
            {
                return NotFound();
            }

            return groupInscription;
        }

        // PUT: api/GroupInscriptions/5
        // To protect from overposting attacks, see https://go.microsoft.com/fwlink/?linkid=2123754
        [HttpPut("{id}")]
        public async Task<IActionResult> PutGroupInscription(long id, GroupInscription groupInscription)
        {
            if (id != groupInscription.Id)
            {
                return BadRequest();
            }

            _context.Entry(groupInscription).State = EntityState.Modified;

            try
            {
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!GroupInscriptionExists(id))
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

        // POST: api/GroupInscriptions
        // To protect from overposting attacks, see https://go.microsoft.com/fwlink/?linkid=2123754
        [HttpPost]
        public async Task<ActionResult<GroupInscription>> PostGroupInscription(GroupInscription groupInscription)
        {
            _context.GroupInscriptions.Add(groupInscription);
            await _context.SaveChangesAsync();

            return CreatedAtAction("GetGroupInscription", new { id = groupInscription.Id }, groupInscription);
        }

        // DELETE: api/GroupInscriptions/5
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteGroupInscription(long id)
        {
            var groupInscription = await _context.GroupInscriptions.FindAsync(id);
            if (groupInscription == null)
            {
                return NotFound();
            }

            _context.GroupInscriptions.Remove(groupInscription);
            await _context.SaveChangesAsync();

            return NoContent();
        }

        private bool GroupInscriptionExists(long id)
        {
            return _context.GroupInscriptions.Any(e => e.Id == id);
        }
    }
}
