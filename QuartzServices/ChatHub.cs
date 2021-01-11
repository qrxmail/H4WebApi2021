using Microsoft.AspNetCore.SignalR;
using System.Threading.Tasks;

namespace H4WebApi.Services
{
    public class ChatHub : Hub
    {
        // 这个方法是前端调用的发送消息的方法
        public async Task SendMessage(string user, string message)
        {
            await Clients.All.SendAsync("ReceiveMessage", user, message);
        }
    }
}