using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using CareerApi.Models;
using TopicosP1Backend.Scripts;

namespace TopicosP1Backend.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class StudyPlansController : ControllerBase
    {
        private readonly Context _context;
        private readonly APIQueue _queue;

        public StudyPlansController(Context context, APIQueue queue)
        {
            _context = context;
            _queue = queue;
        }

        // GET: api/StudyPlans
        [HttpGet]
        public object GetStudyPlans()
        {
            return _queue.Request(Function.GetStudyPlans, [], "", "GetStudyPlans", true);
        }

        // GET: api/StudyPlans/5
        [HttpGet("{id}")]
        public object GetStudyPlan(string id)
        {
            return _queue.Request(Function.GetStudyPlan, [$"{id}"], "", $"GetStudyPlan {id}", true);
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
