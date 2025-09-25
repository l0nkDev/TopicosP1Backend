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
    public class StudentsController : ControllerBase
    {
        private readonly Context _context;
        private readonly APIQueue _queue;

        public StudentsController(Context context, APIQueue queue)
        {
            _context = context;
            _queue = queue;
        }

        [HttpGet]
        public object GetStudents()
        {
            return _queue.Request(Function.GetStudents, [], "", "GetStudents", true);
        }

        [HttpGet("{id}")]
        public object GetStudent(long id)
        {
            return _queue.Request(Function.GetStudent, [$"{id}"], "", $"GetStudent {id}", true);
        }

        [HttpPut("{id}")]
        public object PutStudent(long id, Student.StudentPost student)
        {
            string b = JsonSerializer.Serialize(student);
            return _queue.Request(Function.PutStudent, [$"{id}"], b, $"PutStudent {id} {b}");
        }

        [HttpPost]
        public object PostStudent(Student.StudentPost student)
        {
            string b = JsonSerializer.Serialize(student);
            return _queue.Request(Function.PostStudent, [], b, $"PostStudent {b}");
        }

        [HttpDelete("{id}")]
        public object DeleteStudent(long id)
        {
            return _queue.Request(Function.DeleteStudent, [$"{id}"], "", $"DeleteStudent {id}");
        }

        [HttpGet("{id}/Groups/{stId}")]
        public object GetStudentGroup(long id, long stId)
        {
            return _queue.Request(Function.GetStudentGroup, [$"{id}", $"{stId}"], "", $"GetStudentGroup {id} {stId}");
        }

        [HttpPut("{id}/Groups/{stId}")]
        public object PutStudentGroup(long id, long stId, StudentGroups.SgDTO stDTO)
        {
            string b = JsonSerializer.Serialize(stDTO);
            return _queue.Request(Function.PutStudentGroup, [$"{id}", $"{stId}"], b, $"PutStudentGroup {id} {stId}");
        }

        [HttpGet("{id}/history")]
        public object GetStudentHistory(long id)
        {
            return _queue.Request(Function.GetStudentHistory, [id.ToString()], "", $"GetStudentHistory {id}");
        }

        [HttpGet("{id}/available")]
        public object GetStudentAvailable(long id)
        {
            return _queue.Request(Function.GetStudentAvaliables, [id.ToString()], "", $"GetStudentAvailables {id}");
        }
    }
}
