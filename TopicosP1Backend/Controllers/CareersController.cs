using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using CareerApi.Models;
using static CareerApi.Models.Career;
using TopicosP1Backend.Scripts;
using System.Text.Json;

namespace TopicosP1Backend.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class CareersController : ControllerBase
    {
        private readonly Context _context;
        private readonly APIQueue _queue;

        public CareersController(Context context, APIQueue queue)
        {
            _context = context;
            _queue = queue;
        }

        [HttpGet]
        public object GetCareers()
        {
            return _queue.Request(Function.GetCareers, [], "", $"GetCareers", true);
        }

        [HttpGet("{id}")]
        public object GetCareer(long id)
        {
            return _queue.Request(Function.GetCareer, [$"{id}"], "", $"GetCareer {id}", true);
        }

        [HttpPut("{id}")]
        public object PutCareer(long id, CareerPost c)
        {
            string b = JsonSerializer.Serialize(c);
            return _queue.Request(Function.PutCareer, [$"{id}"], b, $"PutCareer {id} {b}");
        }

        [HttpPost]
        public object PostCareer(CareerPost c)
        {
            string b = JsonSerializer.Serialize(c);
            return _queue.Request(Function.PostCareer, [], b, $"PostCareer {b}");
        }

        [HttpDelete("{id}")]
        public object DeleteCareer(long id)
        {
            return _queue.Request(Function.DeleteCareer, [$"{id}"], "", $"DeleteCareer {id}", true);
        }
    }
}
