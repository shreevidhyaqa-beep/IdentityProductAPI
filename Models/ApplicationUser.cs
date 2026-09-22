using Microsoft.AspNetCore.Identity;

namespace ProductAPIIdentity.Models
{
    public class ApplicationUser:IdentityUser
    {
        public string? FullName { get; set; }


    }
}
