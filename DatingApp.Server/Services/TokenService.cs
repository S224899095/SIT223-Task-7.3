namespace DatingApp.Server.Services;

public class TokenService : ITokenService
{
    private readonly SymmetricSecurityKey _key;
    private readonly UserManager<UserModel>  _userManager;

    public TokenService(IConfiguration config, UserManager<UserModel> userManager)
    {
        string key = config.GetValue<string>("TokenKey");
        _key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(key));
        _userManager = userManager;
    }

    public async Task<string> CreateToken(UserModel user)
    {
        var claims = new List<Claim>()
        {
            new Claim(JwtRegisteredClaimNames.NameId, user.Id.ToString()),
            new Claim(JwtRegisteredClaimNames.UniqueName, user.UserName.ToString())
        };

        var roles = await _userManager.GetRolesAsync(user);
        claims.AddRange(roles.Select(role => new Claim(ClaimTypes.Role, role)));

        var creds = new SigningCredentials(_key, SecurityAlgorithms.HmacSha512Signature);
        var tokenDescrp = new SecurityTokenDescriptor()
        {
            Subject = new ClaimsIdentity(claims),
            Expires = DateTime.Now.AddDays(7),
            SigningCredentials = creds
        };

        var tokenHandler = new JwtSecurityTokenHandler();
        var token = tokenHandler.CreateToken(tokenDescrp);
        return tokenHandler.WriteToken(token);
    }
}
