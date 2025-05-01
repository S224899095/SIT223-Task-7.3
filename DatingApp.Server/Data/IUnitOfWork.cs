namespace DatingApp.Server.Data;

public interface IUnitOfWork
{
    IUserRepository UserRepo {get; }
    IMessagesRepository MessageRepo {get; }
    ILikesRepository LikesRepo {get; }
    Task<bool> Complete();
    bool HasChanges();
}
