namespace DatingApp.Server.Models.App;

public class LikesParams : PaginationParams
{
    public int UserId { get; set; }
    public string Predicate { get; set; } = string.Empty;
}
