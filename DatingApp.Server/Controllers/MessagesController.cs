namespace DatingApp.Server.Controllers;

[ApiController]
[Route("api/[controller]")]
[Authorize]
public class MessagesController : ControllerBase
{
    private readonly IUnitOfWork _uow;
    private readonly IMapper _mapper;

    public MessagesController(IUnitOfWork uow, IMapper mapper)
    {
        _uow = uow;
        _mapper = mapper;
    }

    [HttpPost]
    public async Task<ActionResult<MessageDto>> CreateAsync(CreateMessageDto createMessageDto)
    {
        var username = User.GetUsername();

        if (username == createMessageDto.RecipientUsername.ToLower()) return BadRequest("You cannot send messages to yourself");
        var sender = await _uow.UserRepo.GetUserByUsernameAsync(username);
        var recipient = await _uow.UserRepo.GetUserByUsernameAsync(createMessageDto.RecipientUsername);

        if (recipient == null) return NotFound();
        if (sender == null) return BadRequest();

        var message = new MessageModel(sender, recipient, sender.UserName, recipient.UserName, createMessageDto.Content);
        _uow.MessageRepo.AddMessage(message);

        if (await _uow.Complete()) return Ok(_mapper.Map<MessageDto>(message));
        return BadRequest("Failed to save message");
    }

    [HttpGet]
    public async Task<ActionResult<IEnumerable<MessageDto>>> GetAllAsync([FromQuery] MessageParams messageParams)
    {
        messageParams.Username = User.GetUsername();
        var messages = await _uow.MessageRepo.GetMessageForUser(messageParams);
        Response.AddPaginationHeader(messages.CurrentPage, messages.PageSize, messages.TotalCount, messages.TotalPages);
        return Ok(messages);
    }

    [HttpDelete("{id}")]
    public async Task<ActionResult> DeleteAsync(int id)
    {
        var username = User.GetUsername();
        var message = await _uow.MessageRepo.GetMessage(id);
        if(message == null) return NotFound("message does not exist...");
        if(message.Sender!.UserName != username && message.Recipient!.UserName != username) return Unauthorized();
        if(message.Sender.UserName == username) message.SenderDeleted = true;
        if(message.Recipient!.UserName == username) message.RecipientDeleted = true;
        if(message.SenderDeleted && message.RecipientDeleted) _uow.MessageRepo.DeleteMessage(message);
        if(await _uow.Complete()) return NoContent();
        return BadRequest("Message could not be deleted...");
    }
}
