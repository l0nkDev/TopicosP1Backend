using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using CareerApi.Models;
using TopicosP1Backend.Scripts;
using System.Text.Json;

namespace TopicosP1Backend.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class GroupsController : ControllerBase
    {
        private readonly Context _context;
        private readonly APIQueue _queue;

        public GroupsController(Context context, APIQueue queue)
        {
            _context = context;
            _queue = queue;
        }

        [HttpGet]
        public object GetGroups()
        {
            return _queue.Request(Function.GetGroups, [], "", "GetGroups", true);
        }

        [HttpGet("{id}")]
        public object GetGroup(long id)
        {
            return _queue.Request(Function.GetGroup, [$"{id}"], "", $"GetGroup {id}", true);
        }

        [HttpPut("{id}")]
        public object PutGroup(long id, Group.GroupPost g)
        {
            string b = JsonSerializer.Serialize(g);
            return _queue.Request(Function.PutGroup, [$"{id}"], b, $"PutGroup {id} {b}");
        }

        [HttpPost]
        public object PostGroup(Group.GroupPost g)
        {
            string b = JsonSerializer.Serialize(g);
            return _queue.Request(Function.PostGroup, [], b, $"PostGroup {b}");
        }

        [HttpDelete("{id}")]
        public object DeleteGroup(long id)
        {
            return _queue.Request(Function.DeleteGroup, [$"{id}"], "", $"DeleteGroup {id}");
        }

        [HttpGet("{id}/Timeslots")]
        public object GetTimeSlots(long id)
        {
            return _queue.Request(Function.GetTimeSlots, [$"{id}"], "", $"GetTimeSlots {id}", true);
        }

        [HttpPost("{id}/Timeslots")]
        public object PostTimeSlot(long id, TimeSlot.TimeSlotPost body)
        {
            string b = JsonSerializer.Serialize(body);
            return _queue.Request(Function.PostTimeSlot, [$"{id}"], b, $"PostTimeSlot {id} {b}");
        }

        [HttpPut("{id}/Timeslots/{ts}")]
        public object PutTimeSlot(long id, long ts, TimeSlot.TimeSlotPost body)
        {
            string b = JsonSerializer.Serialize(body);
            return _queue.Request(Function.PutTimeSlot, [$"{id}", $"{ts}"], b, $"PutTimeSlot {id} {ts} {b}");
        }

        [HttpDelete("{id}/Timeslots/{ts}")]
        public object DeleteTimeSlot(long id, long ts)
        {
            return _queue.Request(Function.DeleteTimeSlot, [$"{id}", $"{ts}"], "", $"DeleteTimeSlot {id} {ts}");
        }
    }
}
