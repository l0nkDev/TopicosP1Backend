using Microsoft.AspNetCore.Mvc;
using CareerApi.Models;
using TopicosP1Backend.Scripts;
using System.Text.Json;

namespace TopicosP1Backend.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class PeriodsController : ControllerBase
    {
        private readonly Context _context;
        private readonly APIQueue _queue;

        public PeriodsController(Context context, APIQueue queue)
        {
            _context = context;
            _queue = queue;
        }

        [HttpGet]
        public object GetPeriods()
        {
            return _queue.Request(Function.GetPeriods, [], "", "GetPeriods", true);
        }

        [HttpGet("{id}")]
        public object GetPeriod(long id)
        {
            return _queue.Request(Function.GetPeriod, [$"{id}"], "", $"GetPeriod {id}", true);
        }

        [HttpPost]
        public object PostPeriod(Period.PostDTO period)
        {
            string b = JsonSerializer.Serialize(period);
            return _queue.Request(Function.PostPeriod, [], b, $"PostPeriod {b}");
        }

        [HttpDelete("{id}")]
        public object DeletePeriod(long id)
        {
            return _queue.Request(Function.DeletePeriod, [$"{id}"], "", $"DeletePeriod {id}");
        }
    }
}
