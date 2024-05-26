using Microsoft.AspNetCore.SignalR;
using System.Collections.Concurrent;

namespace WebSocket.Hubs
{
    public class ChatHub : Hub
    {
        private static readonly ConcurrentDictionary<string, string> onlineUsers = new ConcurrentDictionary<string, string>();

        public override Task OnConnectedAsync()
        {
            // Lógica ao conectar
            return base.OnConnectedAsync();
        }

        public override Task OnDisconnectedAsync(System.Exception exception)
        {
            // Remover usuário ao desconectar
            var user = onlineUsers.FirstOrDefault(u => u.Value == Context.ConnectionId);
            if (user.Key != null)
            {
                onlineUsers.TryRemove(user.Key, out _);
            }

            return base.OnDisconnectedAsync(exception);
        }

        public Task AddUser(string userId)
        {
            onlineUsers[userId] = Context.ConnectionId;
            return Task.CompletedTask;
        }

        public async Task SendMessage(string userId, string message)
        {
            if (onlineUsers.TryGetValue(userId, out var connectionId))
            {
                await Clients.Client(connectionId).SendAsync("ReceiveMessage", message);
            }
        }
    }
}
