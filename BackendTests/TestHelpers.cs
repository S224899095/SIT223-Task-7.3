namespace DatingApp.Server.Tests;

public static class TestHelpers
{
    public static ControllerContext FakeUser(string username, int userId = 1)
    {
        var user = new ClaimsPrincipal(new ClaimsIdentity(new[]
        {
            new Claim(ClaimTypes.Name, username),
            new Claim("nameid", userId.ToString())
        }, "mock"));

        return new ControllerContext
        {
            HttpContext = new DefaultHttpContext { User = user }
        };
    }
}

