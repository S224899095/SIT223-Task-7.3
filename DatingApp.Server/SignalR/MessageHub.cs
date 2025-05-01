namespace DatingApp.Server.SignalR;

public class MessageHub : Hub
{
    private readonly IUnitOfWork _uow;
    private readonly IMapper _mapper;
    private readonly IHubContext<PresenceHub> _presenceHub;
    private readonly PresenceTracker _tracker;

    public MessageHub(IUnitOfWork uow, IMapper mapper, IHubContext<PresenceHub> presenceHub, PresenceTracker tracker)
    {
        _uow = uow;
        _mapper = mapper;
        _presenceHub = presenceHub;
        _tracker = tracker;
    }

    public override async Task OnConnectedAsync()
    {
        var httpContext = Context.GetHttpContext();
        var otherUser = httpContext!.Request.Query["user"].ToString();
        var groupName = GetGroupName(Context.User!.GetUsername(), otherUser);

        // save to db
        await Groups.AddToGroupAsync(Context.ConnectionId, groupName);
        var group = await AddToGroup(groupName);
        await Clients.Group(groupName).SendAsync("UpdatedGroup", group);

        var messages = await _uow.MessageRepo.GetMessageThread(Context.User!.GetUsername(), otherUser);
        if(_uow.HasChanges()) await _uow.Complete();
        await Clients.Caller.SendAsync("ReceivedMessageThread", messages);
    }

    public override async Task OnDisconnectedAsync(Exception? exception)
    {
        // remove from db
        var group = await RemoveFromMessageGroup();
        await Clients.Group(group.Name).SendAsync("UpdatedGroup", group);
        await base.OnDisconnectedAsync(exception);
    }

    private string GetGroupName(string caller, string other)
    {
        var stringCompare = string.CompareOrdinal(caller, other) < 0;
        var output = stringCompare ? $"{caller}-{other}" : $"{other}-{caller}";
        return output;
    }

    public async Task SendMessage(CreateMessageDto createMessageDto)
    {
        var username = Context.User!.GetUsername();
        if (username == createMessageDto.RecipientUsername.ToLower()) throw new HubException("You cannot send messages to yourself");

        var sender = await _uow.UserRepo.GetUserByUsernameAsync(username);
        var recipient = await _uow.UserRepo.GetUserByUsernameAsync(createMessageDto.RecipientUsername);

        if (recipient == null) throw new HubException("Not found recipient");
        if (sender == null) throw new HubException("Not found sender");

        var message = new MessageModel(sender, recipient, sender.UserName, recipient.UserName, createMessageDto.Content);
        var groupName = GetGroupName(sender.UserName, recipient.UserName);
        var group = await _uow.MessageRepo.GetMessageGroup(groupName);

        if (group.Connections.Any(x => x.Username == recipient.UserName))
        {
            message.DateRead = DateTime.UtcNow;
        }
        else
        {
            var connections = _tracker.GetConnectionsForUSer(recipient.UserName);
            if (connections != null)
            {
                var m = new { username = sender.UserName, knownAs = sender.KnownAs };
                await _presenceHub.Clients.Clients(connections).SendAsync("NewMessageReceived", m);
            }
        }

        _uow.MessageRepo.AddMessage(message);
        if (await _uow.Complete()) await Clients.Group(groupName).SendAsync("NewMessage", _mapper.Map<MessageDto>(message));
    }

    private async Task<GroupModel> AddToGroup(string groupName)
    {
        var group = await _uow.MessageRepo.GetMessageGroup(groupName);
        var connection = new ConnectionModel(Context.ConnectionId, Context.User!.GetUsername());

        if (group == null)
        {
            group = new GroupModel(groupName);
            _uow.MessageRepo.AddGroup(group);
        }

        group.Connections.Add(connection);
        if(await _uow.Complete()) return group;
        throw new HubException("Failed to join group...");
    }

    private async Task<GroupModel> RemoveFromMessageGroup()
    {
        var group = await _uow.MessageRepo.GetGroupForConnection(Context.ConnectionId);
        var connection = group.Connections.FirstOrDefault(x => x.ConnectionId == Context.ConnectionId);
        _uow.MessageRepo.RemoveConnection(connection);
        if(await _uow.Complete()) return group;
        throw new HubException("Failed to remove from group...");
    }
}
