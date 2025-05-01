namespace DatingApp.Server.Data.Repositories.User;

public class UserRepository : IUserRepository
{
    private readonly DataContext _db;
    private readonly IMapper _mapper;
    public UserRepository(DataContext db, IMapper mapper)
    {
        _db = db;
        _mapper = mapper;
    }

    public async Task<MemberDto?> GetMemberAsync(string username)
    {
        return await _db.Users.Where(u => u.UserName == username).ProjectTo<MemberDto>(_mapper.ConfigurationProvider).SingleOrDefaultAsync();
    }

    public async Task<PagedList<MemberDto>> GetMembersAsync(UserParams userParams)
    {
        var query = _db.Users.AsQueryable();
        query = query.Where(u => u.UserName != userParams.CurrentUserName);
        query = query.Where(u => u.Gender == userParams.Gender);

        var minDob = DateTime.Today.AddYears(-userParams.MaxAge - 1);
        var maxDob = DateTime.Today.AddYears(-userParams.MinAge);

        query = query.Where(u => u.DateOfBirth >= minDob && u.DateOfBirth <= maxDob);
        switch (userParams.OrderBy)
        {
            case "created": query = query.OrderByDescending(u => u.Created); break;
            default: query = query.OrderByDescending(u => u.LastActive); break;
        }

        var output = query.ProjectTo<MemberDto>(_mapper.ConfigurationProvider).AsNoTracking();
        return await PagedList<MemberDto>.CreateAsync(output, userParams.PageNumber, userParams.PageSize);
    }

    public async Task<UserModel?> GetUserByIdAsync(int id)
    {
        return await _db.Users.FindAsync(id);
    }

    public async Task<UserModel?> GetUserByUsernameAsync(string username)
    {
        return await _db.Users.Include(u => u.Photos).SingleOrDefaultAsync(u => u.UserName == username);
    }

    public async Task<IEnumerable<UserModel>> GetUsersAsync()
    {
        return await _db.Users.Include(u => u.Photos).ToListAsync();
    }

    public void Update(UserModel model)
    {
        _db.Entry(model).State = EntityState.Modified;
    }
}
