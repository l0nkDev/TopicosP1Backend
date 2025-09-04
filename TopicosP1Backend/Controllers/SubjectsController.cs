using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using CareerApi.Models;
using TopicosP1Backend.Scripts;
using System.Text.Json;

namespace TopicosP1Backend.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class SubjectsController : ControllerBase
    {
        private readonly Context _context;
        private readonly APIQueue _queue;

        public SubjectsController(Context context, APIQueue queue)
        {
            _context = context;
            _queue = queue;
        }

        // GET: api/Subjects
        [HttpGet]
        public object GetSubjects()
        {
            return _queue.Request(Function.GetSubjects, [], "", "GetSubjects", true);
        }

        // GET: api/Subjects/5
        [HttpGet("{id}")]
        public object GetSubject(string id)
        {
            return _queue.Request(Function.GetSubject, [id], "", $"GetSubject {id}", true);
        }

        // PUT: api/Subjects/5
        // To protect from overposting attacks, see https://go.microsoft.com/fwlink/?linkid=2123754
        [HttpPut("{id}")]
        public async Task<IActionResult> PutSubject(string id, Subject subject)
        {
            if (id != subject.Code)
            {
                return BadRequest();
            }

            _context.Entry(subject).State = EntityState.Modified;

            try
            {
                var oldSubject = await _context.Subjects.FindAsync(id);
                oldSubject.Title = subject.Title;
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!_context.Subjects.Any(e => e.Code == id))
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

        // POST: api/Subjects
        // To protect from overposting attacks, see https://go.microsoft.com/fwlink/?linkid=2123754
        [HttpPost]
        public object PostSubject(Subject.PostSubject s)
        {
            string b = JsonSerializer.Serialize(s);
            return _queue.Request(Function.PostSubject, [], b, $"PostSubject {b}");
        }

        // DELETE: api/Subjects/5
        [HttpDelete("{id}")]
        public object DeleteSubject(string id)
        {
            return _queue.Request(Function.DeleteSubject, [id], "", $"DeleteSubject {id}");
        }

        private bool SubjectExists(string id)
        {
            return _context.Subjects.Any(e => e.Code == id);
        }
    }
}
