namespace DatingApp.Server.Data.Repositories.Likes;

public class LikesRepository : ILikesRepository
{
    private readonly DataContext _db;
    private readonly IMapper _mapper;
    public LikesRepository(DataContext db, IMapper mapper)
    {
        _db = db;
        _mapper = mapper;
    }

    public async Task<UserLikeModel?> GetUserLike(int sourceUserId, int likedUserId)
    {
        var output = await _db.Likes.FindAsync(sourceUserId, likedUserId);
        return output;
    }

    public async Task<UserModel?> GetUserWithLikes(int userId)
    {
        var output = await _db.Users.Include(x => x.LikedUsers).FirstOrDefaultAsync(x => x.Id == userId);
        return output;
    }

    public async Task<PagedList<LikeDto>> GetUserLikes(LikesParams likesParams)
    {
        var users = _db.Users.OrderBy(u => u.UserName).AsQueryable();
        var likes = _db.Likes.AsQueryable();

        if (likesParams.Predicate == "liked")
        {
            likes = likes.Where(l => l.SourceUserId == likesParams.UserId);
            users = likes.Select(l => l.LikedUser);
        }
        if (likesParams.Predicate == "likedBy")
        {
            likes = likes.Where(l => l.LikedUserId == likesParams.UserId);
            users = likes.Select(l => l.SourceUser);
        }

        var likedUsers = users.ProjectTo<LikeDto>(_mapper.ConfigurationProvider);
        var output = await PagedList<LikeDto>.CreateAsync(likedUsers, likesParams.PageNumber, likesParams.PageSize);
        return output;
    }
}
