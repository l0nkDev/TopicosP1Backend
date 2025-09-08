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

        [HttpGet]
        public object GetSubjects()
        {
            return _queue.Request(Function.GetSubjects, [], "", "GetSubjects", true);
        }

        [HttpGet("{id}")]
        public object GetSubject(string id)
        {
            return _queue.Request(Function.GetSubject, [id], "", $"GetSubject {id}", true);
        }

        [HttpPut("{id}")]
        public object PutSubject(string id, Subject.PostSubject s)
        {
            string b = JsonSerializer.Serialize(s);
            return _queue.Request(Function.PutSubject, [id], b, $"PutSubject {id}", true);
        }

        [HttpPost]
        public object PostSubject(Subject.PostSubject s)
        {
            string b = JsonSerializer.Serialize(s);
            return _queue.Request(Function.PostSubject, [], b, $"PostSubject {b}");
        }

        [HttpDelete("{id}")]
        public object DeleteSubject(string id)
        {
            return _queue.Request(Function.DeleteSubject, [id], "", $"DeleteSubject {id}");
        }
    }
}
