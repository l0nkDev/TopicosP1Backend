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
using System.Text.Json;

namespace TopicosP1Backend.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class GestionsController : ControllerBase
    {
        private readonly Context _context;
        private readonly APIQueue _queue;

        public GestionsController(Context context, APIQueue queue)
        {
            _context = context;
            _queue = queue;
        }

        [HttpGet]
        public async Task<object> GetGestions()
        {
            return _queue.Request(Function.GetGestions, [], "", $"GetGestions", true);
        }

        [HttpGet("{id}")]
        public async Task<object> GetGestion(long id)
        {
            return _queue.Request(Function.GetGestion, [id.ToString()], "", $"GetGestion {id}", true);
        }

        [HttpPost]
        public object PostGestion(Gestion gestion)
        {
            string b = JsonSerializer.Serialize(gestion);
            return _queue.Request(Function.PostGestion, [], b, $"PostGestion {b}");
        }

        [HttpDelete("{id}")]
        public object DeleteGestion(long id)
        {
            return _queue.Request(Function.DeleteGestion, [$"{id}"], "", $"DeleteGestion {id}");
        }
    }
}
