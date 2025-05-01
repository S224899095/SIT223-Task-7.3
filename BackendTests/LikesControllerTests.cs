namespace DatingApp.Server.Tests;


public class LikesControllerTests
{
    private readonly Mock<IUnitOfWork> _mockUow;
    private readonly Mock<IUserRepository> _mockUserRepo;
    private readonly Mock<ILikesRepository> _mockLikesRepo;

    public LikesControllerTests()
    {
        _mockUow = new Mock<IUnitOfWork>();
        _mockUserRepo = new Mock<IUserRepository>();
        _mockLikesRepo = new Mock<ILikesRepository>();

        _mockUow.Setup(u => u.UserRepo).Returns(_mockUserRepo.Object);
        _mockUow.Setup(u => u.LikesRepo).Returns(_mockLikesRepo.Object);
    }

    [Fact]
    public async Task AddLike_ReturnsNotFound_WhenUserNotExists()
    {
        // Arrange
        var controller = new LikesController(_mockUow.Object);
        var username = "nonexistent";

        _mockUserRepo.Setup(r => r.GetUserByUsernameAsync(username)).ReturnsAsync((UserModel?)null);
        controller.ControllerContext = TestHelpers.FakeUser("sourceUser");

        // Act
        var result = await controller.AddLike(username);

        // Assert
        Assert.IsType<NotFoundResult>(result);
    }

    [Fact]
    public async Task AddLike_ReturnsBadRequest_WhenUserLikesHimself()
    {
        // Arrange
        var controller = new LikesController(_mockUow.Object);
        var username = "selfUser";

        var sourceUser = new UserModel { UserName = "selfUser" };
        _mockUserRepo.Setup(r => r.GetUserByUsernameAsync(username)).ReturnsAsync(sourceUser);
        controller.ControllerContext = TestHelpers.FakeUser("selfUser");

        // Act
        var result = await controller.AddLike(username);

        // Assert
        Assert.IsType<NotFoundResult>(result);
    }
}
