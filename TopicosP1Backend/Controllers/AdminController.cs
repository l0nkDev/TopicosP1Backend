using Microsoft.AspNetCore.Mvc;
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
        public async Task<ActionResult<List<Queue<QueuedFunction>>>> GetQueues()
        {
            return _queue.GetQueues();
        }

        [HttpGet("Queues/{id}")]
        public async Task<ActionResult<Queue<QueuedFunction>>> GetQueue(int id)
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
    }
}
