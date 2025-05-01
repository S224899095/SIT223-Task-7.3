namespace DatingApp.Server.Data.Repositories.Messages;

public interface IMessagesRepository
{
    void AddGroup(GroupModel group);
    void RemoveConnection(ConnectionModel connection);
    Task<ConnectionModel> GetConnection(string connectionId);
    Task<GroupModel> GetMessageGroup(string groupName);
    Task<GroupModel> GetGroupForConnection(string connectionId);
    void AddMessage(MessageModel message);
    void DeleteMessage(MessageModel message);
    Task<MessageModel?> GetMessage(int id);
    Task<PagedList<MessageDto>> GetMessageForUser(MessageParams messageParams);
    Task<IEnumerable<MessageDto>> GetMessageThread(string currentUsername, string recipientUsername);
}
