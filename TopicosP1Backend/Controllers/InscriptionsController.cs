using Microsoft.AspNetCore.Mvc;
using CareerApi.Models;
using TopicosP1Backend.Scripts;
using System.Text.Json;

namespace TopicosP1Backend.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class InscriptionsController : ControllerBase
    {
        private readonly Context _context;
        private readonly APIQueue _queue;

        public InscriptionsController(Context context, APIQueue queue)
        {
            _context = context;
            _queue = queue;
        }

        [HttpGet]
        public object GetInscriptions()
        {
            return _queue.Request(Function.GetInscriptions, [], "", "GetInscriptions", true);
        }

        [HttpGet("{id}")]
        public object GetInscription(long id)
        {
            return _queue.Request(Function.GetInscription, [$"{id}"], "", $"GetInscription {id}", true);
        }

        [HttpPut("{id}")]
        public object PutInscription(long id, Inscription.InscriptionPost i)
        {
            string b = JsonSerializer.Serialize(i);
            return _queue.Request(Function.PutInscription, [$"{id}"], b, $"PutInscription {id} {b}");
        }

        [HttpPost]
        public object PostInscription(Inscription.InscriptionPost i)
        {
            string b = JsonSerializer.Serialize(i);
            return _queue.Request(Function.PostInscription, [], b, $"PostInscription {b}");
        }

        [HttpDelete("{id}")]
        public object DeleteInscription(long id)
        {
            return _queue.Request(Function.DeleteInscription, [$"{id}"], "", $"DeleteInscription {id}");
        }

        [HttpGet("{id}/Groups")]
        public object GetInsGroups(long id)
        {
            return _queue.Request(Function.GetInsGroups, [$"{id}"], "", $"GetInsGroups {id}", true);
        }

        [HttpPost("{id}/Groups")]
        public object PostInsGroup(long id, Inscription.GIPost body)
        {
            string b = JsonSerializer.Serialize(body);
            return _queue.Request(Function.PostInsGroup, [$"{id}"], b, $"PostInsGroup {id} {b}");
        }

        [HttpDelete("{id}/Groups/{group}")]
        public object DeleteInsGroup(long id, long group)
        {
            return _queue.Request(Function.DeleteInsGroup, [$"{id}", $"{group}"], "", $"DeleteInsGroup {id} {group}");
        }
    }
}
