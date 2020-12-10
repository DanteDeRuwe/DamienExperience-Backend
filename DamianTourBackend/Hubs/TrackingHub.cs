using System.Text.RegularExpressions;
using System.Threading.Tasks;
using Microsoft.AspNetCore.SignalR;

namespace DamianTourBackend.Api.Hubs
{
    public class TrackingHub : Hub
    {
        public Task JoinGroup(string groupName) =>
            Groups.AddToGroupAsync(Context.ConnectionId, groupName);
    }
}