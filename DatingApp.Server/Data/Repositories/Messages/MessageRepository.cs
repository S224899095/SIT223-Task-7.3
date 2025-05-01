namespace DatingApp.Server.Data.Repositories.Messages;

public class MessageRepository : IMessagesRepository
{
    private readonly DataContext _db;
    private readonly IMapper _mapper;
    public MessageRepository(DataContext db, IMapper mapper)
    {
        _db = db;
        _mapper = mapper;
    }

    public void AddGroup(GroupModel group)
    {
        _db.Groups.Add(group);
    }

    public void AddMessage(MessageModel message)
    {
        _db.Messages.Add(message);
    }

    public void DeleteMessage(MessageModel message)
    {
        _db.Messages.Remove(message);
    }

    public async Task<ConnectionModel> GetConnection(string connectionId)
    {
        return await _db.Connections.FindAsync(connectionId);
    }

    public async Task<GroupModel> GetGroupForConnection(string connectionId)
    {
        return await _db.Groups.Include(c => c.Connections).Where(c => c.Connections.Any(x => x.ConnectionId == connectionId)).FirstOrDefaultAsync();
    }

    public async Task<MessageModel?> GetMessage(int id)
    {
        var output = await _db.Messages.Where(m => m.Id == id).Include(u => u.Sender).Include(u => u.Recipient).FirstOrDefaultAsync();
        return output;
    }

    public async Task<PagedList<MessageDto>> GetMessageForUser(MessageParams messageParams)
    {
        var query = _db.Messages.OrderByDescending(m => m.MessageSent).ProjectTo<MessageDto>(_mapper.ConfigurationProvider).AsQueryable();

        switch (messageParams.Container)
        {
            case "Inbox": query = query.Where(u => u.RecipientUsername == messageParams.Username && u.RecipientDeleted == false); break;
            case "Outbox": query = query.Where(u => u.SenderUsername == messageParams.Username && u.SenderDeleted == false); break;
            default: query = query.Where(u => u.RecipientUsername == messageParams.Username && u.RecipientDeleted == false && u.DateRead == null); break;
        }

        return await PagedList<MessageDto>.CreateAsync(query, messageParams.PageNumber, messageParams.PageSize);
    }

    public async Task<GroupModel> GetMessageGroup(string groupName)
    {
        return await _db.Groups.Include(x => x.Connections).FirstOrDefaultAsync(x => x.Name == groupName);
    }

    public async Task<IEnumerable<MessageDto>> GetMessageThread(string currentUsername, string recipientUsername)
    {
        var messages = await _db.Messages
            .Include(u => u.Sender).ThenInclude(p => p.Photos)
            .Include(u => u.Recipient).ThenInclude(p => p.Photos)
            .Where(m => (m.Recipient.UserName == currentUsername && m.Sender.UserName == recipientUsername && m.RecipientDeleted == false) ||
                        (m.Recipient.UserName == recipientUsername && m.Sender.UserName == currentUsername && m.SenderDeleted == false))
            .OrderBy(m => m.MessageSent).ToListAsync();

        var unreadMessages = messages.Where(m => m.DateRead == null && m.RecipientUsername == currentUsername).ToList();

        if (unreadMessages.Any())
        {
            foreach (var message in unreadMessages)
            {
                message.DateRead = DateTime.UtcNow;
            }
        }

        return _mapper.Map<IEnumerable<MessageDto>>(messages);
    }

    public void RemoveConnection(ConnectionModel connection)
    {
        _db.Connections.Remove(connection);
    }
}
