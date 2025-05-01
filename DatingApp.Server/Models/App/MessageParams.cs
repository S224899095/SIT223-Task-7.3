namespace DatingApp.Server.Models.App;

public class MessageParams : PaginationParams
{
    public string? Username { get; set; }
    public string Container { get; set; }
}
