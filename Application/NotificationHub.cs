using Microsoft.AspNetCore.SignalR;

namespace Application
{
    public class NotificationHub : Hub
    {
        public override async Task OnConnectedAsync()
        {
            var userId = Context.UserIdentifier;

            await Groups.AddToGroupAsync(Context.ConnectionId, $"User_{userId}");
            await base.OnConnectedAsync();
        }
    }

}
