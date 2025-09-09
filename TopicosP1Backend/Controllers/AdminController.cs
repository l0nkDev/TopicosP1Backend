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
        public async Task<IActionResult> GetQueues()
        {
            _stopper.StartAsync();
            return Ok();
        }
    }
}
