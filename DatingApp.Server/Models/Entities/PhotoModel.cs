namespace DatingApp.Server.Models.Entities;

[Table("Photos")]
public class PhotoModel
{
    public int Id { get; set; }
    public string Url { get; set; }
    public bool IsMain { get; set; }
    public string? PublicId { get; set; }
    public int UserId { get; set; }
    public UserModel User { get; set; }
}
