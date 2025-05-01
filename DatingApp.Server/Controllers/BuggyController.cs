namespace DatingApp.Server.Controllers;

[ApiController]
[Route("api/[controller]")]
public class BuggyController : Controller
{
    private readonly DataContext _data;

    public BuggyController(DataContext data)
    {
        _data = data;
    }

    [Authorize]
    [HttpGet("auth")]
    public async Task<ActionResult<String>> GetSecret()
    {
        return "secret";
    }

    [HttpGet("not-found")]
    public ActionResult<UserModel> GetNotFound()
    {
        var thing = _data.Users.Find(-1);
        if (thing == null) return NotFound();
        return Ok(thing);
    }

    [HttpGet("server-error")]
    public ActionResult<UserModel> GetServerError()
    {
        var thing = _data.Users.Find(-1);
        var output = thing.ToString();
        return Ok(output);
    }

    [HttpGet("bad-request")]
    public ActionResult<string> GetBadRequest()
    {
        return BadRequest("This was not a good request.");
    }
}
