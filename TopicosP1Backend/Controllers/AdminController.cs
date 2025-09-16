using Microsoft.AspNetCore.Mvc;
using System.Collections.Concurrent;
using TopicosP1Backend.Scripts;

namespace TopicosP1Backend.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class AdminController(APIQueue queue, WorkerManager manager) : ControllerBase
    {
        private readonly APIQueue _queue = queue;
        private readonly WorkerManager _manager = manager;

        [HttpGet("Queues")]
        public async Task<ActionResult<List<ConcurrentQueue<QueuedFunction>>>> GetQueues()
        {
            return _queue.GetQueues();
        }

        [HttpGet("Queues/{id}")]
        public async Task<ActionResult<ConcurrentQueue<QueuedFunction>>> GetQueue(int id)
        {
            return _queue.GetQueue(id);
        }

        [HttpGet("Queues/SetCountTo/{id}")]
        public async void SetCountTo(int id)
        {
            _queue.SetQueuesCount(id);
        }

        [HttpGet("Workers")]
        public async Task<object> GetWorkers()
        {
            return _manager.GetWorkers();
        }

        [HttpGet("Workers/{id}")]
        public async Task<object> GetWorker(int id)
        {
            return _manager.GetWorker(id);
        }

        [HttpGet("Workers/{id}/Start")]
        public async Task<object> StartWorker(int id)
        {
            _manager.Start(id);
            return new OkResult();
        }

        [HttpGet("Workers/{id}/Stop")]
        public async Task<object> StopWorker(int id)
        {
            _manager.Stop(id);
            return new OkResult();
        }

        [HttpGet("Workers/SetCountTo/{id}")]
        public async void WSetCountTo(int id)
        {
            _manager.SetCountTo(id);
        }

        [HttpGet("Transaction/{id}")]
        public async Task<ActionResult<object>> GetTransaction(string id)
        {
            return _queue.getTranStatus(id);
        }

        [HttpGet("MakeAsync")]
        public async Task<ActionResult<object>> MakeAsync()
        {
            return _queue.isasync = true;
        }

        [HttpGet("MakeSync")]
        public async Task<ActionResult<object>> MakeSync()
        {
            return _queue.isasync = false;
        }

        [HttpGet("ThingsReceived")]
        public async Task<ActionResult<object>> GetThingsReceived()
        {
            int total = _queue.thingsreceived["Total"];
            _queue.thingsreceived["Total"] = _queue.thingsreceived.Values.Sum() - total;
            return _queue.thingsreceived;
        }

        [HttpGet("ThingsDone")]
        public async Task<ActionResult<object>> GetThingsDone()
        {
            int total = _queue.thingsdone["Total"];
            _queue.thingsdone["Total"] = _queue.thingsdone.Values.Sum() - total;
            return _queue.thingsdone;
        }
    }
}
