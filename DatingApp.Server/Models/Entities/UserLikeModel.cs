namespace DatingApp.Server.Models.Entities;

[Table("Likes")]
public class UserLikeModel
{
    public UserLikeModel(){}
    public UserLikeModel(int sourceUserId, int likedUserId)
    {
        SourceUserId = sourceUserId;
        LikedUserId = likedUserId;
    }

    public int SourceUserId { get; set; }
    public UserModel? SourceUser { get; set; }
    public int LikedUserId { get; set; }
    public UserModel? LikedUser { get; set; }
}
