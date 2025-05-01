namespace DatingApp.Server.Data.Repositories.Likes;

public interface ILikesRepository
{
    Task<UserLikeModel?> GetUserLike(int sourceUserId, int likedUserId);
    Task<UserModel?> GetUserWithLikes(int userId);
    Task<PagedList<LikeDto>> GetUserLikes(LikesParams likesParams);
}
