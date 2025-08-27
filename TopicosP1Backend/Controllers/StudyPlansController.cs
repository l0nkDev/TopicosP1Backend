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
    public class StudyPlansController : ControllerBase
    {
        private readonly Context _context;

        public StudyPlansController(Context context)
        {
            _context = context;
        }

        // GET: api/StudyPlans
        [HttpGet]
        public async Task<IEnumerable<StudyPlanDTO>> GetStudyPlans()
        {
            var db = await _context.StudyPlans.ToListAsync();
            var studyplans = from sp in db select GetStudyPlan(sp.Code).Result.Value;
            return studyplans;
        }

        // GET: api/StudyPlans/5
        [HttpGet("{id}")]
        public async Task<ActionResult<StudyPlanDTO>> GetStudyPlan(string id)
        {
            var studyPlan = await _context.StudyPlans.Include(_ => _.Career).Include(_ => _.Subjects).ThenInclude(_ => _.Prerequisites).FirstOrDefaultAsync(i => i.Code == id);
            if (studyPlan == null) return NotFound();
            return new StudyPlanDTO(studyPlan);
        }

        // PUT: api/StudyPlans/5
        // To protect from overposting attacks, see https://go.microsoft.com/fwlink/?linkid=2123754
        [HttpPut("{id}")]
        public async Task<IActionResult> PutStudyPlan(string id, StudyPlan studyPlan)
        {
            if (id != studyPlan.Code){ return BadRequest(); }
            _context.Entry(studyPlan).State = EntityState.Modified;

            try
            {
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!StudyPlanExists(id))
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

        // POST: api/StudyPlans
        // To protect from overposting attacks, see https://go.microsoft.com/fwlink/?linkid=2123754
        [HttpPost]
        public async Task<ActionResult<StudyPlan>> PostStudyPlan(StudyPlan studyPlan)
        {
            _context.StudyPlans.Add(studyPlan);
            try
            {
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateException)
            {
                if (StudyPlanExists(studyPlan.Code))
                {
                    return Conflict();
                }
                else
                {
                    throw;
                }
            }

            return CreatedAtAction("GetStudyPlan", new { id = studyPlan.Code }, studyPlan);
        }

        // DELETE: api/StudyPlans/5
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteStudyPlan(string id)
        {
            var studyPlan = await _context.StudyPlans.FindAsync(id);
            if (studyPlan == null)
            {
                return NotFound();
            }

            _context.StudyPlans.Remove(studyPlan);
            await _context.SaveChangesAsync();

            return NoContent();
        }

        private bool StudyPlanExists(string id)
        {
            return _context.StudyPlans.Any(e => e.Code == id);
        }
    }
}
