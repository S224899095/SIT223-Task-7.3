namespace DatingApp.Server.Models.Entities;

public class ConnectionModel
{
    public ConnectionModel() { }
    public ConnectionModel(string connectionId, string username)
    {
        ConnectionId = connectionId;
        Username = username;
    }

    [Key]
    public string ConnectionId { get; set; }
    public string Username { get; set; }
}
