using Microsoft.AspNetCore.Mvc;
using TopicosP1Backend.Scripts;

namespace TopicosP1Backend.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class AdminController(IQueueWorkerStopper stopper, APIQueue queue) : ControllerBase
    {
        private readonly IQueueWorkerStopper _stopper = stopper;
        private readonly APIQueue _queue = queue;

        [HttpGet("Stop")]
        public async Task<IActionResult> Stop()
        {
            _stopper.StopAsync();
            return Ok();
        }

        [HttpGet("Start")]
        public async Task<IActionResult> Start()
        {
            _stopper.StartAsync();
            return Ok();
        }

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

        [HttpGet("Transaction/{id}")]
        public async Task<ActionResult<object>> GetTransaction(string id)
        {
            return _queue.getTranStatus(id);
        }
    }
}
