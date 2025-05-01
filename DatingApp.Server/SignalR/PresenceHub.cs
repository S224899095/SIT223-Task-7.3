namespace DatingApp.Server.SignalR;

public class PresenceHub : Hub
{
    private readonly PresenceTracker _tracker;

    public PresenceHub(PresenceTracker tracker)
    {
        _tracker = tracker;

    }

    [Authorize]
    public override async Task OnConnectedAsync()
    {
        var isOnline = _tracker.UserConnected(Context.User!.GetUsername(), Context.ConnectionId);
        if(isOnline) await Clients.Others.SendAsync("UserIsOnline", Context.User!.GetUsername());

        var currentUsers = _tracker.GetOnlineUsers();
        await Clients.Caller.SendAsync("GetOnlineUsers", currentUsers);
    }

    public override async Task OnDisconnectedAsync(Exception? exception)
    {
        var isOffline = _tracker.UserDictonnected(Context.User!.GetUsername(), Context.ConnectionId);
        if(isOffline) await Clients.Others.SendAsync("UserIsOffline", Context.User!.GetUsername());
        await base.OnDisconnectedAsync(exception);
    }
}
