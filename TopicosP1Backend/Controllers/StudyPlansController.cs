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
            int tranid = "GetStudyPlans".GetHashCode();
            if (_queue.IsQueued(tranid) != null) return tranid;
            try { return _queue.Get(tranid, true); } catch { Console.WriteLine("Failed!"); }
            _queue.Add(new QueuedFunction()
            { Function = Function.GetStudyPlans, Args = [], Hash = tranid });
            return tranid;
        }

        // GET: api/StudyPlans/5
        [HttpGet("{id}")]
        public object GetStudyPlan(string id)
        {
            int tranid = ("GetStudyPlan " + id.ToString()).GetHashCode();
            if (_queue.IsQueued(tranid) != null) return tranid;
            try { return _queue.Get(tranid, true); } catch { Console.WriteLine("Failed!"); }
            _queue.Add(new QueuedFunction()
            { Function = Function.GetStudyPlan, Args = [id], Hash = tranid });
            return tranid;
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
