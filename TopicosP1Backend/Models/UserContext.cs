using Microsoft.EntityFrameworkCore;
using Microsoft.Net.Http.Headers;

namespace CareerApi.Models
{
    public class UserContext : DbContext
    {
        public UserContext(DbContextOptions<UserContext> options)
        : base(options)
        {
        }

        public DbSet<User> Users { get; set; } = default!;

        public char GetRequestUserRole(HttpRequest request)
        {
            string token = request.Headers[HeaderNames.Authorization].ToString().Replace("Bearer ", "");
            Console.WriteLine(token);
            var user = Users.FirstOrDefault(x => x.Token == token);
            if (user == null) { Console.WriteLine("User not found."); return 'n';}
            return user.Role;
        }
    }
}

