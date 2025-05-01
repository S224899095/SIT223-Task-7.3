namespace DatingApp.Server.Services;

public interface ITokenService
{
    Task<string> CreateToken(UserModel user);
}
