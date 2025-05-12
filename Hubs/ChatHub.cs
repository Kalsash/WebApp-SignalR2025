using Microsoft.AspNetCore.SignalR;
using System.Security.Claims;

namespace WebApp_SignalR2025.Hubs
{
    public class ChatHub : Hub
    {

        public override async Task OnConnectedAsync()
        {
            var httpContext = Context.GetHttpContext();
            await base.OnConnectedAsync();
        }

        public async Task SendMessage(string user, string message)
        {
            await Clients.Others.SendAsync("ReceiveMessage", user, message);
        }

        public async Task TypingNotification(string user)
        {
            await Clients.Others.SendAsync("UserTyping", user);
        }
    }
}
