namespace DatingApp.Server.Controllers;

[ApiController]
[Route("api/[controller]")]
[Authorize]
public class LikesController : ControllerBase
{
    private readonly IUnitOfWork _uow;
    
    public LikesController(IUnitOfWork uow)
    {
        _uow = uow;
    }

    [HttpPost("{username}")]
    public async Task<ActionResult> AddLike(string username)
    {
        var sourceUserId = User.GetUserId();
        var likedUser = await _uow.UserRepo.GetUserByUsernameAsync(username);
        var sourceUser = await _uow.LikesRepo.GetUserWithLikes(sourceUserId);

        if (likedUser == null) return NotFound();
        if (sourceUser == null) return NotFound();
        if (sourceUser.UserName == username) return BadRequest("You cannot like yourself");

        var userLike = await _uow.LikesRepo.GetUserLike(sourceUserId, likedUser.Id);

        if (userLike != null) return BadRequest("You already liked this user");

        userLike = new UserLikeModel(sourceUserId, likedUser.Id);
        sourceUser.LikedUsers.Add(userLike);

        if(await _uow.Complete()) return Ok();
        return BadRequest("Failed to liked user");
    }

    [HttpGet]
    public async Task<ActionResult<IEnumerable<LikeDto>>> GetUserLikes([FromQuery] LikesParams likesParams){
        likesParams.UserId = User.GetUserId();
        var output = await _uow.LikesRepo.GetUserLikes(likesParams);
        Response.AddPaginationHeader(output.CurrentPage, output.PageSize, output.TotalCount, output.TotalPages);
        return Ok(output);
    }
}
