using Microsoft.AspNetCore.Identity;

namespace WebApp_SignalR2025.Models
{
    public class User : IdentityUser
    {
        public string IconPath { get; set; }
    }
}
