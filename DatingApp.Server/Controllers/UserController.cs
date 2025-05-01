namespace DatingApp.Server.Controllers;

[ServiceFilter(typeof(LogUserActivity))]
[ApiController]
[Route("api/[controller]")]
[Authorize]
public class UserController : ControllerBase
{
    private readonly IUnitOfWork _uow;
    private readonly IMapper _mapper;
    private readonly IPhotoService _photoService;

    public UserController(IUnitOfWork uow, IPhotoService photoService, IMapper mapper)
    {
        _photoService = photoService;
        _uow = uow;
        _mapper = mapper;
    }

    [HttpGet]
    public async Task<ActionResult<IEnumerable<MemberDto>>> GetAllAsync([FromQuery] UserParams? userParams) 
    {
        if (userParams == null) userParams = new();
        var user = await _uow.UserRepo.GetUserByUsernameAsync(User.GetUsername()!);

        if(user == null) return BadRequest();
        userParams.CurrentUserName = user.UserName;

        if (string.IsNullOrEmpty(userParams.Gender))
            userParams.Gender = user.Gender == "male" ? "female" : "male";

        var users = await _uow.UserRepo.GetMembersAsync(userParams ?? new());
        if (users.Any() == false)
            return NotFound();
            
        Response.AddPaginationHeader(users.CurrentPage, users.PageSize, users.TotalCount, users.TotalPages);
        return Ok(users);
    }

    [HttpGet("{username}", Name = "GetUser")]
    public async Task<ActionResult<MemberDto>> GetAsync(string username)
    {
        var user = await _uow.UserRepo.GetMemberAsync(username);
        if (user == null)
            return NotFound();
        return Ok(user);
    }

    [HttpPut]
    public async Task<ActionResult> UpdateAsync(MemberUpdateDto member)
    {
        var user = await _uow.UserRepo.GetUserByUsernameAsync(User.GetUsername());
        _mapper.Map(member, user);
        _uow.UserRepo.Update(user);

        if (await _uow.Complete())
            return NoContent();
        return BadRequest();
    }

    [HttpPut("set-main-photo/{photoId}")]
    public async Task<ActionResult> SetMainPhoto(int photoId)
    {
        var user = await _uow.UserRepo.GetUserByUsernameAsync(User.GetUsername());
        var photo = user.Photos.FirstOrDefault(x => x.Id == photoId);

        if (photo.IsMain) return BadRequest("Already main photo");
        var currentMain = user.Photos.FirstOrDefault(x => x.IsMain);
        if (currentMain != null) currentMain.IsMain = false;
        photo.IsMain = true;

        if (await _uow.Complete()) return NoContent();
        return BadRequest("Failed to set main photo");
    }

    [HttpPost("add-photo")]
    public async Task<ActionResult<PhotoDto>> AddPhoto(IFormFile file)
    {
        var user = await _uow.UserRepo.GetUserByUsernameAsync(User.GetUsername());
        var result = await _photoService.AddPhotoAsync(file);

        if (result.Error != null)
            return BadRequest(result.Error.Message);

        var photo = new PhotoModel()
        {
            Url = result.SecureUrl.AbsoluteUri,
            PublicId = result.PublicId
        };

        if (user.Photos.Count == 0)
            photo.IsMain = true;

        user.Photos.Add(photo);

        if (await _uow.Complete())
            return CreatedAtRoute("GetUser", new { Username = user.UserName }, _mapper.Map<PhotoDto>(photo));

        return BadRequest("Problem upoloading photo");
    }

    [HttpDelete("delete-photo/{photoId}")]
    public async Task<ActionResult> DeletePhoto(int photoId)
    {
        var user = await _uow.UserRepo.GetUserByUsernameAsync(User.GetUsername());
        var photo = user.Photos.FirstOrDefault(x => x.Id == photoId);

        if (photo == null) return NotFound();
        if (photo.IsMain) return BadRequest("cannot delete main photo.");
        if (photo.PublicId != null)
        {
            var result = await _photoService.DeletePhotoAsync(photo.PublicId);
            if (result.Error != null) return BadRequest(result.Error.Message);
        }
        user.Photos.Remove(photo);
        if (await _uow.Complete()) return Ok();
        return BadRequest();
    }
}
