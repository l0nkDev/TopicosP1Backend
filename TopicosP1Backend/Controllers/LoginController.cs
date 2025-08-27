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
    [Route("api/auth")]
    [ApiController]
    public class LoginController : ControllerBase
    {
        private readonly UserContext _context;

        public LoginController(UserContext context)
        {
            _context = context;
        }
        // POST: api/Careers
        // To protect from overposting attacks, see https://go.microsoft.com/fwlink/?linkid=2123754
        [HttpPost]
        public async Task<ActionResult<LoginResponse>> PostLogin(LoginRequest request)
        {
            if (request == null) { return BadRequest(); }

            int hash = Util.Hash(request.Password);
            Console.WriteLine(request.Login);
            Console.WriteLine(hash);

            var user = await _context.Users.FirstOrDefaultAsync(i => i.Login == request.Login && i.PasswordHash == hash);
            if (user == null) { return NotFound(); }
            return Ok(new LoginResponse { Login = user.Login, Token = user.Token });
            
        }

        [HttpGet("{id}")]
        public async Task<ActionResult<User>> GetUser(long id)
        {
            var user = await _context.Users.FindAsync(id);

            if (_context.GetRequestUserRole(Request) != 'C') { return Unauthorized(); }
            if (user == null)
            {
                return NotFound();
            }

            return user;
        }

        private bool UserExists(long id)
        {
            return _context.Users.Any(e => e.Id == id);
        }

    }
}
