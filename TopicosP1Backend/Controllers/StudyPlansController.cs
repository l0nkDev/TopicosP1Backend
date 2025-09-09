using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using CareerApi.Models;
using TopicosP1Backend.Scripts;
using static CareerApi.Models.Career;
using System.Text.Json;

namespace TopicosP1Backend.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class StudyPlansController : ControllerBase
    {
        private readonly Context _context;
        private readonly APIQueue _queue;

        public StudyPlansController(Context context, APIQueue queue)
        {
            _context = context;
            _queue = queue;
        }

        [HttpGet]
        public object GetStudyPlans()
        {
            return _queue.Request(Function.GetStudyPlans, [], "", "GetStudyPlans", true);
        }

        [HttpGet("{id}")]
        public object GetStudyPlan(string id)
        {
            return _queue.Request(Function.GetStudyPlan, [id], "", $"GetStudyPlan {id}", true);
        }

        [HttpPut("{id}")]
        public object PutStudyPlan(string id, StudyPlan.StudyPlanPost sp)
        {
            string b = JsonSerializer.Serialize(sp);
            return _queue.Request(Function.PutStudyPlan, [id], b, $"PutStudyPlan {id} {b}", true);
        }

        [HttpPost]
        public object PostStudyPlan(StudyPlan.StudyPlanPost c)
        {
            string b = JsonSerializer.Serialize(c);
            return _queue.Request(Function.PostStudyPlan, [], b, $"PostStudyPlan {b}", true);
        }

        [HttpDelete("{id}")]
        public object DeleteStudyPlan(string id)
        {
            return _queue.Request(Function.DeleteStudyPlan, [id], "", $"DeleteStudyPlan {id}", true);
        }

        [HttpGet("{id}/Subjects")]
        public object GetSpSubjects(string id)
        {
            return _queue.Request(Function.GetSpSubjects, [id], "", $"GetSpSubjects {id}", true);
        }

        [HttpPost("{id}/Subjects")]
        public object PostSpSubject(string id, StudyPlan.StudyPlanSubjectPost body)
        {
            return _queue.Request(Function.PostSpSubject, [id], JsonSerializer.Serialize(body), $"PostSpSubject {id}", true);
        }

        [HttpPut("{id}/Subjects/{sub}")]
        public object PutSpSubject(string id, string sub, StudyPlan.StudyPlanSubjectPost body)
        {
            return _queue.Request(Function.PutSpSubject, [id, sub], JsonSerializer.Serialize(body), $"PutSpSubject {id}", true);
        }

        [HttpDelete("{id}/Subjects/{sub}")]
        public object DeleteSpSubject(string id, string sub)
        {
            return _queue.Request(Function.DeleteSpSubject, [id, sub], "", $"DeleteSpSubject {id}", true);
        }

        [HttpGet("{id}/Subjects/{sub}/Dependencies")]
        public object GetSpSubDependencies(string id, string sub)
        {
            return _queue.Request(Function.GetSpSubDependencies, [id, sub], "", $"GetSpSubDependencies {id} {sub}", true);
        }

        [HttpPost("{id}/Subjects/{sub}/Dependencies")]
        public object PostSpSubDependency(string id, string sub, StudyPlan.SPSDependency body)
        {
            return _queue.Request(Function.PostSpSubDependency, [id, sub], JsonSerializer.Serialize(body), $"PostSpSubDependency {id} {sub}", true);
        }

        [HttpDelete("{id}/Subjects/{sub}/Dependencies/{pre}")]
        public object DeleteSpSubDependency(string id, string sub, string pre)
        {
            return _queue.Request(Function.DeleteSpSubDependency, [id, sub, pre], "", $"DeleteSpSubDependency {id} {sub}", true);
        }
    }
}
