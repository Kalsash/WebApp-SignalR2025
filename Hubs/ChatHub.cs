using Microsoft.AspNetCore.SignalR;
using Microsoft.AspNetCore.Identity;
using WebApp_SignalR2025.Models;

namespace WebApp_SignalR2025.Hubs
{
    public class ChatHub : Hub
    {
        private readonly UserManager<User> _userManager;
        private readonly Dictionary<string, HashSet<string>> _groupMembers = new();

        public ChatHub(UserManager<User> userManager)
        {
            _userManager = userManager;
        }

        public override async Task OnConnectedAsync()
        {
            if (Context.User?.Identity?.IsAuthenticated == true)
            {
                var user = await _userManager.GetUserAsync(Context.User);
                await Groups.AddToGroupAsync(Context.ConnectionId, $"user_{user.Id}");
            }
            await base.OnConnectedAsync();
        }

        // Group chat methods
        public async Task JoinGroupChat(string groupName)
        {
            if (Context.User?.Identity?.IsAuthenticated == true)
            {
                var user = await _userManager.GetUserAsync(Context.User);
                await Groups.AddToGroupAsync(Context.ConnectionId, groupName);

                if (!_groupMembers.ContainsKey(groupName))
                {
                    _groupMembers[groupName] = new HashSet<string>();
                }
                _groupMembers[groupName].Add(user.Id);

                var userNameWithoutEmail = user.UserName?.Split('@')[0]; // Убираем почту из имени
                await Clients.Group(groupName).SendAsync("UserJoined", userNameWithoutEmail);
            }
        }

        public async Task LeaveGroupChat(string groupName)
        {
            if (Context.User?.Identity?.IsAuthenticated == true)
            {
                var user = await _userManager.GetUserAsync(Context.User);
                await Groups.RemoveFromGroupAsync(Context.ConnectionId, groupName);

                if (_groupMembers.ContainsKey(groupName))
                {
                    _groupMembers[groupName].Remove(user.Id);
                    if (_groupMembers[groupName].Count == 0)
                    {
                        _groupMembers.Remove(groupName);
                    }
                }

                var userNameWithoutEmail = user.UserName?.Split('@')[0]; // Убираем почту из имени
                await Clients.Group(groupName).SendAsync("UserLeft", userNameWithoutEmail);
            }
        }

        public async Task SendGroupMessage(string groupName, string message)
        {
            if (Context.User?.Identity?.IsAuthenticated == true)
            {
                var sender = await _userManager.GetUserAsync(Context.User);
                var userNameWithoutEmail = sender.UserName?.Split('@')[0]; // Убираем почту из имени

                await Clients.Group(groupName).SendAsync("ReceiveGroupMessage",
                    new
                    {
                        SenderId = sender.Id,
                        SenderName = userNameWithoutEmail,
                        SenderIcon = sender.IconPath ?? "https://i.pravatar.cc/100",
                        GroupName = groupName,
                        Message = message,
                        Timestamp = DateTime.Now.ToString("o")
                    });
            }
        }

        public async Task SendMessage(string receiverId, string message)
        {
            if (Context.User?.Identity?.IsAuthenticated == true)
            {
                var sender = await _userManager.GetUserAsync(Context.User);
                var receiver = await _userManager.FindByIdAsync(receiverId);
                var userNameWithoutEmail = sender.UserName?.Split('@')[0]; // Убираем почту из имени

                if (sender != null && receiver != null)
                {
                    await Clients.Group($"user_{receiver.Id}").SendAsync("ReceiveMessage",
                        new
                        {
                            SenderId = sender.Id,
                            SenderName = userNameWithoutEmail,
                            SenderIcon = sender.IconPath ?? "https://i.pravatar.cc/100",
                            Message = message,
                            Timestamp = DateTime.Now.ToString("o")
                        });
                }
            }
        }

        public async Task TypingNotification(string receiverId)
        {
            if (Context.User?.Identity?.IsAuthenticated == true)
            {
                var sender = await _userManager.GetUserAsync(Context.User);
                var receiver = await _userManager.FindByIdAsync(receiverId);
                var userNameWithoutEmail = sender.UserName?.Split('@')[0]; // Убираем почту из имени

                if (sender != null && receiver != null)
                {
                    await Clients.Group($"user_{receiver.Id}").SendAsync("UserTyping",
                        new
                        {
                            SenderId = sender.Id,
                            SenderName = userNameWithoutEmail
                        });
                }
            }
        }
    }
}