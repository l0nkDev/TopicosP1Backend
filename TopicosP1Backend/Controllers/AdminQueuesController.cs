using Microsoft.AspNetCore.Mvc;
using Microsoft.OpenApi.Extensions;
using System.Collections.Concurrent;
using System.Runtime.InteropServices;
using TopicosP1Backend.Scripts;

namespace TopicosP1Backend.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class AdminQueuesController(APIQueue queue, WorkerManager manager) : ControllerBase
    {
        private readonly APIQueue _queue = queue;
        private readonly WorkerManager _manager = manager;

        [HttpGet("")]
        public async Task<ActionResult<List<Dictionary<string, object>>>> GetQueues()
        {
            List<CustomQueue> queues = _queue.GetQueues();
            List<Dictionary<string, object>> res = [];
            int i = 0;
            while (i < queues.Count)
            {
                Dictionary<string, object> tmp = [];
                tmp.Add("Id", i+1);
                tmp.Add("Count", queues[i].Count);
                tmp.Add("Endpoints", from q in queues[i].Endpoints select ((Function)q).GetDisplayName());
                tmp.Add("Items", queues[i]);
                res.Add(tmp);
                i++;
            }
            return res;
        }

        [HttpGet("{id}")]
        public async Task<ActionResult<Dictionary<string, object>>> GetQueue(int id)
        {
            CustomQueue queue = _queue.GetQueue(id);
            Dictionary<string, object> tmp = [];
            tmp.Add("Id", id);
            tmp.Add("Count", queue.Count);
            tmp.Add("Endpoints", from q in queue.Endpoints select ((Function)q).GetDisplayName());
            tmp.Add("Items", queue);
            return tmp;

        }

        [HttpPost("")]
        public async Task<ActionResult<Dictionary<string, object>>> AddQueue(List<int> Endpoints = null)
        {
            _queue.AddQueue(Endpoints);
            return new OkResult();

        }

    }
}
