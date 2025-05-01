namespace DatingApp.Server.Data;

public class UnitOfWork : IUnitOfWork
{
    private readonly DataContext _db;
    private readonly IMapper _mapper;

    public UnitOfWork(DataContext db, IMapper mapper)
    {
        _db = db;
        _mapper = mapper;
    }

    public IUserRepository UserRepo => new UserRepository(_db, _mapper);

    public IMessagesRepository MessageRepo => new MessageRepository(_db, _mapper);

    public ILikesRepository LikesRepo => new LikesRepository(_db, _mapper);

    public async Task<bool> Complete()
    {
        return await _db.SaveChangesAsync() > 0;
    }

    public bool HasChanges()
    {
        var output = _db.ChangeTracker.HasChanges();
        return output;
    }
}
