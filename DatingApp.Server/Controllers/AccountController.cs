namespace DatingApp.Server.Controllers;

[ApiController]
[Route("api/[controller]")]
public class AccountController : ControllerBase
{
    private readonly UserManager<UserModel> _userManager;
    private readonly SignInManager<UserModel> _signInManager;
    private readonly ITokenService _tokenService;
    private readonly IMapper _mapper;

    public AccountController(UserManager<UserModel> userManager, SignInManager<UserModel> signInManager, ITokenService tokenService, IMapper mapper)
    {
        _mapper = mapper;
        _userManager = userManager;
        _signInManager = signInManager;
        _tokenService = tokenService;
    }

    [HttpPost("register")]
    public async Task<ActionResult<UserDto>> Register(RegisterDto registerDto)
    {
        if (!ModelState.IsValid) return BadRequest(ModelState);
        if (await UserExists(registerDto.Username)) return BadRequest("User already exists");

        var user = _mapper.Map<UserModel>(registerDto);
        user.UserName = registerDto.Username.ToLower();
        var result = await _userManager.CreateAsync(user);

        if (!result.Succeeded) return BadRequest(result.Errors);

        var roleResult = await _userManager.AddToRoleAsync(user, "Member");

        if(!roleResult.Succeeded) return BadRequest(roleResult.Errors);

        var output = new UserDto()
        {
            Username = user.UserName,
            Token = await _tokenService.CreateToken(user),
            KnownAs = user.KnownAs,
            Gender = user.Gender
        };

        return Ok(output);
    }

    [HttpPost("login")]
    public async Task<ActionResult<UserDto>> Login(LoginDto loginDto)
    {
        var user = await _userManager.Users.Include(p => p.Photos).SingleOrDefaultAsync(u => u.UserName == loginDto.Username.ToLower());

        if (user == null) return Unauthorized("Invalid username");

        var result = await _signInManager.CheckPasswordSignInAsync(user, loginDto.Password, false);

        if (!result.Succeeded) return Unauthorized();

        var output = new UserDto()
        {
            Username = user.UserName,
            Token = await _tokenService.CreateToken(user),
            PhotoUrl = user.Photos.FirstOrDefault(x => x.IsMain)?.Url,
            KnownAs = user.KnownAs,
            Gender = user.Gender
        };

        return Ok(output);
    }

    private async Task<bool> UserExists(string username)
    {
        return await _userManager.Users.AnyAsync(u => u.UserName == username.ToLower());
    }
}

