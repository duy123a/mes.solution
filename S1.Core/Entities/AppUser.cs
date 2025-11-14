using Microsoft.AspNetCore.Identity;

namespace S1.Core.Entities
{
    public class AppUser : IdentityUser
    {
        public string DisplayName { get; set; } = string.Empty;
        public string ProfileImg { get; set; } = string.Empty;
    }
}
