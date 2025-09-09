using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using CareerApi.Models;
using TopicosP1Backend.Scripts;
using System.Text.Json;

namespace TopicosP1Backend.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class ModulesController : ControllerBase
    {
        private readonly Context _context;
        private readonly APIQueue _queue;

        public ModulesController(Context context, APIQueue queue)
        {
            _context = context;
            _queue = queue;
        }

        [HttpGet]
        public object GetModules()
        {
            return _queue.Request(Function.GetModules, [], "", "GetModules", true);
        }

        [HttpGet("{id}")]
        public object GetModule(long id)
        {
            return _queue.Request(Function.GetModule, [$"{id}"], "", $"GetModule {id}", true);
        }


        [HttpPost]
        public object PostModule(Module.ModulePost m)
        {
            string b = JsonSerializer.Serialize(m);
            return _queue.Request(Function.PostModule, [], b, $"PostModule {b}");
        }

        [HttpDelete("{id}")]
        public object DeleteModule(long id)
        {
            return _queue.Request(Function.DeleteModule, [$"{id}"], "", $"DeleteModule {id}");
        }

        [HttpGet("{id}/Rooms")]
        public object GetModRooms(long id)
        {
            return _queue.Request(Function.GetModRooms, [$"{id}"], "", $"getModRooms {id}", true);
        }

        [HttpPost("{id}/Rooms")]
        public object PostModRoom(long id, Module.ModulePost room)
        {
            string b = JsonSerializer.Serialize(room);
            return _queue.Request(Function.PostModRoom, [$"{id}"], b, $"PostModRoom {id} {b}");
        }

        [HttpDelete("{id}/Rooms{room}")]
        public object DeleteModRoom(long id, long room)
        {
            return _queue.Request(Function.DeleteModRoom, [$"{id}", $"{room}"], "", $"DeleteModRoom {id} {room}");
        }
    }
}
