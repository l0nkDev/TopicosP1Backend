using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using CareerApi.Models;
using TopicosP1Backend.Scripts;
using static CareerApi.Models.Career;
using System.Text.Json;

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
            return _queue.Request(Function.GetStudyPlan, [id], "", $"GetStudyPlan {id}", true);
        }

        [HttpPut("{id}")]
        public async Task<IActionResult> PutStudyPlan(string id, StudyPlan.StudyPlanPost sp)
        {
            if (id != sp.Code) return new BadRequestResult();
            StudyPlan studyplan = await _context.StudyPlans.FindAsync(id);
            studyplan.Career = await _context.Careers.FindAsync(sp.Career);
            _context.Entry(studyplan).State = EntityState.Modified;

            try { await _context.SaveChangesAsync(); }
            catch (DbUpdateConcurrencyException)
            {
                if (!_context.StudyPlans.Any(e => e.Code == id)) return new NotFoundResult();
                else throw;
            }
            return new NoContentResult();
        }

        // POST: api/Careers
        // To protect from overposting attacks, see https://go.microsoft.com/fwlink/?linkid=2123754
        [HttpPost]
        public async Task<ActionResult<StudyPlan.StudyPlanDTO>> PostStudyPlan(StudyPlan.StudyPlanPost c)
        {
            StudyPlan sp = new StudyPlan { Code = c.Code, Career = await _context.Careers.FindAsync(c.Career) };
            _context.StudyPlans.Add(sp);
            await _context.SaveChangesAsync();
            return CreatedAtAction(nameof(PostStudyPlan), new { id = sp.Code}, sp.Simple());
        }

        // DELETE: api/Careers/5
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteStudyPlan(string id)
        {
            var sp = await _context.StudyPlans.FindAsync(id);
            if (sp == null) return new NotFoundResult();
            _context.StudyPlans.Remove(sp);
            await _context.SaveChangesAsync();
            return new NoContentResult();
        }

        // GET: api/StudyPlans/5
        [HttpGet("{id}/Subjects")]
        public object GetStudyPlanSubjects(string id)
        {
            return _queue.Request(Function.GetStudyPlanSubjects, [id], "", $"GetStudyPlanSubjects {id}", true);
        }

        // GET: api/StudyPlans/5
        [HttpPost("{id}/Subjects")]
        public object PostStudyPlanSubject(string id, StudyPlan.StudyPlanSubjectPost body)
        {
            return _queue.Request(Function.PostStudyPlanSubject, [id], JsonSerializer.Serialize(body), $"PostStudyPlanSubject {id}", true);
        }

        // GET: api/StudyPlans/5
        [HttpPut("{id}/Subjects/{sub}")]
        public object PutStudyPlanSubject(string id, string sub, StudyPlan.StudyPlanSubjectPost body)
        {
            return _queue.Request(Function.PutStudyPlanSubject, [id, sub], JsonSerializer.Serialize(body), $"PutStudyPlanSubject {id}", true);
        }

        // GET: api/StudyPlans/5
        [HttpDelete("{id}/Subjects/{sub}")]
        public object DeleteStudyPlanSubject(string id, string sub)
        {
            return _queue.Request(Function.DeleteStudyPlanSubject, [id, sub], "", $"DeleteStudyPlanSubject {id}", true);
        }

        // GET: api/StudyPlans/5
        [HttpGet("{id}/Subjects/{sub}/Dependencies")]
        public object GetStudyPlanSubjectDeps(string id, string sub)
        {
            return _queue.Request(Function.GetStudyPlanSubjectDeps, [id, sub], "", $"GetStudyPlanSubjectDeps {id} {sub}", true);
        }

        // GET: api/StudyPlans/5
        [HttpPost("{id}/Subjects/{sub}/Dependencies")]
        public object PostStudyPlanSubjectDep(string id, string sub, StudyPlan.SPSDependency body)
        {
            return _queue.Request(Function.PostStudyPlanSubjectDep, [id, sub], JsonSerializer.Serialize(body), $"GetStudyPlanSubjectDep {id} {sub}", true);
        }

        // GET: api/StudyPlans/5
        [HttpDelete("{id}/Subjects/{sub}/Dependencies/{pre}")]
        public object DeleteStudyPlanSubjectDep(string id, string sub, string pre)
        {
            return _queue.Request(Function.DeleteStudyPlanSubjectDep, [id, sub, pre], "", $"DeleteStudyPlanSubjectDep {id} {sub}", true);
        }
    }
}
