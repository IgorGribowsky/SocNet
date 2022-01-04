using Microsoft.AspNetCore.Http;
using System.Linq;
using System.Threading.Tasks;
using SocNet.Services.AuthenticationManaging;

namespace SocNet.WebAPI.Middleware;

public class JwtMiddleware
{
    private readonly RequestDelegate _next;

    public JwtMiddleware(RequestDelegate next)
    {
        _next = next;
    }

    public async Task Invoke(HttpContext context, IJwtManagingService jwtManager, ICustomAuthenticationService validationService)
    {
        var token = context.Request.Headers["Authorization"].FirstOrDefault()?.Split(" ").Last();
        var userId = await validationService.ValidateJwtAsync(token);
        if (userId != null)
        {
            context.Items["User"] = validationService.GetUSerIdentityById(userId.Value);
        }

        await _next(context);
    }
}
