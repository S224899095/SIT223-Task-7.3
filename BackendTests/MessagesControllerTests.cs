namespace DatingApp.Server.Tests;

public class MessagesControllerTests
{
    private readonly Mock<IUnitOfWork> _mockUow;
    private readonly Mock<IUserRepository> _mockUserRepo;
    private readonly Mock<IMessagesRepository> _mockMessageRepo;
    private readonly Mock<IMapper> _mockMapper;

    public MessagesControllerTests()
    {
        _mockUow = new Mock<IUnitOfWork>();
        _mockUserRepo = new Mock<IUserRepository>();
        _mockMessageRepo = new Mock<IMessagesRepository>();
        _mockMapper = new Mock<IMapper>();

        _mockUow.Setup(u => u.UserRepo).Returns(_mockUserRepo.Object);
        _mockUow.Setup(u => u.MessageRepo).Returns(_mockMessageRepo.Object);
    }

    [Fact]
    public async Task CreateMessage_ReturnsBadRequest_WhenSenderIsRecipient()
    {
        // Arrange
        var controller = new MessagesController(_mockUow.Object, _mockMapper.Object);
        var messageDto = new CreateMessageDto { RecipientUsername = "user1", Content = "Hello" };

        _mockUserRepo.Setup(r => r.GetUserByUsernameAsync("user1")).ReturnsAsync(new UserModel { UserName = "user1" });
        controller.ControllerContext = TestHelpers.FakeUser("user1");

        // Act
        var result = await controller.CreateAsync(messageDto);

        // Assert
        Assert.IsType<BadRequestObjectResult>(result.Result);
    }

    [Fact]
    public async Task CreateMessage_ReturnsNotFound_WhenRecipientDoesNotExist()
    {
        // Arrange
        var controller = new MessagesController(_mockUow.Object, _mockMapper.Object);
        var messageDto = new CreateMessageDto { RecipientUsername = "nonexistent", Content = "Hello" };

        _mockUserRepo.Setup(r => r.GetUserByUsernameAsync("nonexistent")).ReturnsAsync((UserModel?)null);
        controller.ControllerContext = TestHelpers.FakeUser("user1");

        // Act
        var result = await controller.CreateAsync(messageDto);

        // Assert
        Assert.IsType<NotFoundResult>(result.Result);
    }

    [Fact]
    public async Task DeleteMessage_ReturnsNotFound_WhenMessageDoesNotExist()
    {
        // Arrange
        var controller = new MessagesController(_mockUow.Object, _mockMapper.Object);
        var messageId = 1;

        _mockMessageRepo.Setup(r => r.GetMessage(messageId)).ReturnsAsync((MessageModel?)null);
        controller.ControllerContext = TestHelpers.FakeUser("user1");

        // Act
        var result = await controller.DeleteAsync(messageId);

        // Assert
        Assert.IsType<NotFoundObjectResult>(result);
    }

    [Fact]
    public async Task DeleteMessage_ReturnsNoContent_WhenDeleted()
    {
        // Arrange
        var controller = new MessagesController(_mockUow.Object, _mockMapper.Object);
        var messageId = 1;

        var message = new MessageModel { Sender = new UserModel { UserName = "user1" }, Recipient = new UserModel { UserName = "user2" } };
        _mockMessageRepo.Setup(r => r.GetMessage(messageId)).ReturnsAsync(message);
        _mockUow.Setup(u => u.Complete()).ReturnsAsync(true);
        controller.ControllerContext = TestHelpers.FakeUser("user1");

        // Act
        var result = await controller.DeleteAsync(messageId);

        // Assert
        Assert.IsType<NoContentResult>(result);
    }
}
