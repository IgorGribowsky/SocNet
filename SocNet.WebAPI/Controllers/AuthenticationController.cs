using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using SocNet.Core.Entities;
using SocNet.Services.AuthenticationManaging;
using SocNet.WebAPI.Models;
using System.Threading.Tasks;

namespace SocNet.WebAPI.Controllers;

[Route("api/[controller]")]
[ApiController]
public class AuthenticationController : ControllerBase
{
    private readonly ILogger<AuthenticationController> _logger;


    private readonly IJwtManagingService _jwtManager;

    private readonly ICustomAuthenticationService _authService;

    public AuthenticationController(
        ILogger<AuthenticationController> logger,
        IJwtManagingService jwtManager,
        ICustomAuthenticationService userValidator)
    {
        _logger = logger;
        _jwtManager = jwtManager;
        _authService = userValidator;
    }

    [HttpPost("signup")]
    [ProducesResponseType(StatusCodes.Status201Created, Type = typeof(AuthenticationSuccessModel))]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    public async Task<ActionResult<AuthenticationSuccessModel>> SignUpAsync([FromBody] UserSignupModel signupData)
    {
        var isUsernameUnique = await _authService.ChechUsernameUniquenessAsync(signupData.Username);

        if (!isUsernameUnique)
        {
            return BadRequest(new { message = "Username already taken" });
        }

        var user = await _authService.SignUpAsync(
                    new SignupDto(
                        Username: signupData.Username,
                        Password: signupData.Password,
                        FirstName: signupData.FirstName,
                        LastName: signupData.LastName
                    ));

        AddAuthorizationHeader(user);

        // Controller name should be without "Controller" at the end
        var usersControllerName = nameof(UsersController);
        usersControllerName = usersControllerName.Substring(0, usersControllerName.LastIndexOf("Controller"));

        return CreatedAtAction(nameof(UsersController.GetByIdAsync), usersControllerName, new { id = user.UserId },
            new AuthenticationSuccessModel
            {
                UserId = user.UserId,
                Username = user.UserName
            });
    }

    [HttpPost("signin")]
    [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(AuthenticationSuccessModel))]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    public async Task<ActionResult<AuthenticationSuccessModel>> SignInAsync([FromBody] AuthenticationModel credentials)
    {
        var user = await _authService.GetUSerIdentityByCredentialsAsync(userName: credentials.Username, password: credentials.Password);

        if (user is null)
        {
            return BadRequest(new { message = "Incorrect username or password" });
        }
        AddAuthorizationHeader(user);

        return Ok(new AuthenticationSuccessModel
        {
            UserId = user.UserId,
            Username = user.UserName
        });
    }

    [HttpPost("validate")]
    [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(int))]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<ActionResult<int>> ValidateAsync([FromHeader(Name = "Authorization")] string token)
    {
        if (string.IsNullOrEmpty(token))
        {
            return BadRequest(new { message = "Provide Authorization token" });
        }

        var id = await _authService.ValidateJwtAsync(token);

        if (id is null)
        {
            return NotFound();
        }

        return Ok(id);
    }
    private void AddAuthorizationHeader(UserIdentity user)
    {
        var token = "Bearer " + _jwtManager.CreateToken(user);
        HttpContext.Response.Headers.Add("Authorization", token);
    }
}
