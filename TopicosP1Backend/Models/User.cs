using Microsoft.AspNetCore.Identity;

namespace CareerApi.Models
{
    public class User: IdentityUser
    {
        required public string FirstName { get; set; }
        required public string LastName { get; set; }
    }
}
