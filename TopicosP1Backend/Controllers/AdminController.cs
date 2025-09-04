using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using CareerApi.Models;
using TopicosP1Backend.Scripts;

namespace TopicosP1Backend.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class AdminController(IQueueWorkerStopper stopper) : ControllerBase
    {
        private readonly IQueueWorkerStopper _stopper = stopper;

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
    }
}
