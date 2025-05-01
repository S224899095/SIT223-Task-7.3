namespace DatingApp.Server.SignalR;

public class PresenceTracker
{
    private static readonly Dictionary<string, List<string>> OnlineUsers = new();

    public bool UserConnected(string username, string connectionId)
    {
        bool isOnline = false;
        lock (OnlineUsers)
        {
            if (OnlineUsers.ContainsKey(username))
            {
                OnlineUsers[username].Add(connectionId);
            }
            else
            {
                OnlineUsers.Add(username, new List<string> { connectionId });
                isOnline = true;
            }
        }

        return isOnline;
    }

    public bool UserDictonnected(string username, string connectionId)
    {
        bool isOffline = false;
        lock (OnlineUsers)
        {
            if (!OnlineUsers.ContainsKey(username)) return isOffline;
            OnlineUsers[username].Remove(connectionId);
            if (OnlineUsers[username].Count == 0)
            {
                OnlineUsers.Remove(username);
                isOffline = true;
            }
        }

        return isOffline;
    }

    public string[] GetOnlineUsers()
    {
        string[] onlineUsers;
        lock (OnlineUsers)
        {
            onlineUsers = OnlineUsers.OrderBy(k => k.Key).Select(k => k.Key).ToArray();
        }

        return onlineUsers;
    }

    public List<string> GetConnectionsForUSer(string username)
    {
        List<string> connectionIds;
        lock (OnlineUsers)
        {
            connectionIds = OnlineUsers.GetValueOrDefault(username);
        }

        return connectionIds;
    }
}
