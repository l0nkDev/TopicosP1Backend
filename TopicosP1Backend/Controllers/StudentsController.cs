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

namespace TopicosP1Backend.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class StudentsController : ControllerBase
    {
        private readonly Context _context;
        private readonly APIQueue _queue;

        public StudentsController(Context context, APIQueue queue)
        {
            _context = context;
            _queue = queue;
        }

        // GET: api/Students
        [HttpGet]
        public async Task<ActionResult<IEnumerable<Student>>> GetStudents()
        {
            return await _context.Students.ToListAsync();
        }

        // GET: api/Students/5
        [HttpGet("{id}")]
        public async Task<ActionResult<Student>> GetStudent(long id)
        {
            var student = await _context.Students.FindAsync(id);

            if (student == null)
            {
                return NotFound();
            }

            return student;
        }

        [HttpGet("{id}/history")]
        public object GetStudentHistory(long id)
        {
            int tranid = ("GetStudentHistory " + id.ToString()).GetHashCode();
            if (_queue.IsQueued(tranid) != null) return tranid;
            try { return _queue.Get(tranid, false); } catch { Console.WriteLine("Failed!"); }
            _queue.Add(() => _queue.GetStudentHistory(id, tranid));
            return tranid;
        }

        [HttpGet("{id}/available")]
        public object GetStudentAvailable(long id)
        {
            int tranid = ("GetStudentAvailable " + id.ToString()).GetHashCode();
            if (_queue.IsQueued(tranid) != null) return tranid;
            try { return _queue.Get(tranid, false); } catch { Console.WriteLine("Failed!"); }
            _queue.Add(() => _queue.GetStudentAvailable(id, tranid));
            return tranid;
        }

        // PUT: api/Students/5
        // To protect from overposting attacks, see https://go.microsoft.com/fwlink/?linkid=2123754
        [HttpPut("{id}")]
        public async Task<IActionResult> PutStudent(long id, Student student)
        {
            if (id != student.Id)
            {
                return BadRequest();
            }

            _context.Entry(student).State = EntityState.Modified;

            try
            {
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!StudentExists(id))
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

        // POST: api/Students
        // To protect from overposting attacks, see https://go.microsoft.com/fwlink/?linkid=2123754
        [HttpPost]
        public async Task<ActionResult<Student>> PostStudent(Student student)
        {
            _context.Students.Add(student);
            await _context.SaveChangesAsync();

            return CreatedAtAction("GetStudent", new { id = student.Id }, student);
        }

        // DELETE: api/Students/5
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteStudent(long id)
        {
            var student = await _context.Students.FindAsync(id);
            if (student == null)
            {
                return NotFound();
            }

            _context.Students.Remove(student);
            await _context.SaveChangesAsync();

            return NoContent();
        }

        private bool StudentExists(long id)
        {
            return _context.Students.Any(e => e.Id == id);
        }
    }
}
