namespace DatingApp.Server.Helpers;

public class LogUserActivity : IAsyncActionFilter
{
    public async Task OnActionExecutionAsync(ActionExecutingContext context, ActionExecutionDelegate next)
    {
        var resultContext = await next();
        if(resultContext.HttpContext.User.Identity!.IsAuthenticated == false) return;

        var userId = resultContext.HttpContext.User.GetUserId();
        var uow = resultContext.HttpContext.RequestServices.GetService<IUnitOfWork>()!;
        var user = await uow.UserRepo.GetUserByIdAsync(userId);
        
        user!.LastActive = DateTime.UtcNow;
        await uow.Complete();
    }
}
