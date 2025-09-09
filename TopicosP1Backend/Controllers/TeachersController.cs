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
using System.Security.Policy;
using System.Text.Json;

namespace TopicosP1Backend.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class TeachersController : ControllerBase
    {
        private readonly Context _context;
        private readonly APIQueue _queue;

        public TeachersController(Context context, APIQueue queue)
        {
            _context = context;
            _queue = queue;
        }

        [HttpGet]
        public object GetTeachers()
        {
            return _queue.Request(Function.GetTeachers, [], "", "GetTeachers", true);
        }

        [HttpGet("{id}")]
        public object GetTeacher(long id)
        {
            return _queue.Request(Function.GetTeacher, [$"{id}"], "", $"GetTeacher {id}", true);
        }

        [HttpPut("{id}")]
        public object PutTeacher(long id, Teacher.TeacherPost student)
        {
            string b = JsonSerializer.Serialize(student);
            return _queue.Request(Function.PutTeacher, [$"{id}"], b, $"PutTeacher {id} {b}");
        }

        [HttpPost]
        public object PostTeacher(Teacher.TeacherPost student)
        {
            string b = JsonSerializer.Serialize(student);
            return _queue.Request(Function.PostTeacher, [], b, $"PostTeacher {b}");
        }

        [HttpDelete("{id}")]
        public object DeleteTeacher(long id)
        {
            return _queue.Request(Function.DeleteTeacher, [$"{id}"], "", $"DeleteTeacher {id}");
        }
    }
}
