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
    public class GroupsController : ControllerBase
    {
        private readonly Context _context;

        public GroupsController(Context context)
        {
            _context = context;
        }

        // GET: api/Groups
        [HttpGet]
        public async Task<ActionResult<IEnumerable<Group.GroupDTO>>> GetGroups()
        {
            List<Group> l = await _context.Groups.ToListAsync();
            return (from i in l select i.Simple()).ToList();
        }

        // GET: api/Groups/5
        [HttpGet("{id}")]
        public async Task<ActionResult<Group.GroupDTO>> GetGroup(long id)
        {
            var @group = await _context.Groups.FindAsync(id);

            if (@group == null)
            {
                return NotFound();
            }

            return @group.Simple();
        }

        // PUT: api/Groups/5
        // To protect from overposting attacks, see https://go.microsoft.com/fwlink/?linkid=2123754
        [HttpPut("{id}")]
        public async Task<ActionResult<Group.GroupDTO>> PutGroup(long id, Group.PostGroup g)
        {
            Group group = await _context.Groups.FindAsync(id);
            if (id != g.Id) return BadRequest();
            group.Period = await _context.Periods.FirstOrDefaultAsync(_ => _.Number == g.Period && _.Gestion.Year == g.Gestion);
            group.Code = g.Code;
            group.Mode = g.Mode;
            group.Subject = await _context.Subjects.FindAsync(g.Subject);
            group.Teacher = await _context.Teachers.FindAsync(g.Teacher);
            group.Quota = g.Quota;
            _context.Entry(group).State = EntityState.Modified;
            try { await _context.SaveChangesAsync(); }
            catch (DbUpdateConcurrencyException)
            { if (!_context.Groups.Any(e => e.Id == id)) return NotFound(); else throw; }
            return group.Simple();
        }

        // POST: api/Groups
        // To protect from overposting attacks, see https://go.microsoft.com/fwlink/?linkid=2123754
        [HttpPost]
        public async Task<ActionResult<Group.GroupDTO>> PostGroup(Group.PostGroup g)
        {
            Group @group = new()
            {
                Period = await _context.Periods.FirstOrDefaultAsync(_ => _.Number == g.Period && _.Gestion.Year == g.Gestion),
                Code = g.Code,
                Mode = g.Mode,
                Subject = await _context.Subjects.FindAsync(g.Subject),
                Teacher = await _context.Teachers.FindAsync(g.Teacher),
                Quota = g.Quota,
            };
            _context.Groups.Add(@group);
            await _context.SaveChangesAsync();

            return CreatedAtAction("GetGroup", new { id = @group.Id }, @group.Simple());
        }

        // DELETE: api/Groups/5
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteGroup(long id)
        {
            var @group = await _context.Groups.FindAsync(id);
            if (@group == null)
            {
                return NotFound();
            }

            _context.Groups.Remove(@group);
            await _context.SaveChangesAsync();

            return NoContent();
        }

        [HttpGet("{id}/Timeslots")]
        public async Task<ActionResult<List<TimeSlot.TimeSlotDTO>>> GetTimeSlots(long id)
        {
            Group g = await _context.Groups.FindAsync(id);
            return (from i in g.TimeSlots select i.Simple()).ToList();
        }

        [HttpPost("{id}/Timeslots")]
        public async Task<ActionResult<TimeSlot.TimeSlotDTO>> PostTimeSlot(long id, TimeSlot.TimeSlotPost body)
        {
            Room room = await _context.Rooms.FirstOrDefaultAsync(_ => _.Number == body.Room && _.Module.Number == body.Module);
            Group group = await _context.Groups.FindAsync(id);
            if (room == null || group == null) return new NotFoundResult();
            TimeSlot g = new()
            {
                Day = body.Day,
                StartTime = body.StartTime,
                EndTime = body.EndTime,
                Room = room,
                Group = group
            };
            _context.TimeSlots.Add(g);
            _context.SaveChangesAsync();
            return g.Simple();
        }

        [HttpPut("{id}/Timeslots/{ts}")]
        public async Task<ActionResult<TimeSlot.TimeSlotDTO>> PutTimeSlot(long id, long ts, TimeSlot.TimeSlotPost body)
        {
            TimeSlot timeslot = await _context.TimeSlots.FindAsync(ts);
            Room room = await _context.Rooms.FirstOrDefaultAsync(_ => _.Number == body.Room && _.Module.Number == body.Module);
            Group group = await _context.Groups.FindAsync(id);
            if (room == null || group == null || timeslot == null) return new NotFoundResult();
            timeslot.Day = body.Day;
            timeslot.StartTime = body.StartTime;
            timeslot.EndTime = body.EndTime;
            timeslot.Room = room;
            timeslot.Group = group;
            _context.Entry(timeslot).State = EntityState.Modified;
            _context.SaveChangesAsync();
            return timeslot.Simple();
        }

        [HttpDelete("{id}/Timeslots/{ts}")]
        public async Task<ActionResult<TimeSlot.TimeSlotDTO>> DeleteTimeSlot(long id, long ts)
        {
            TimeSlot timeslot = await _context.TimeSlots.FindAsync(ts);
            if (timeslot == null) return new NotFoundResult();
            _context.TimeSlots.Remove(timeslot);
            _context.SaveChangesAsync();
            return new OkResult();
        }
    }
}
