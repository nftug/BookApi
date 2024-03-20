using BookApi.Domain.DTOs.Commands;
using BookApi.Domain.DTOs.Queries;
using BookApi.Presentation.Abstractions.Controllers;
using BookApi.Presentation.Services;
using BookApi.UseCase.Users;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace BookApi.Presentation.Controllers;

public class UsersController(ISender sender, ActorFactoryService actorFactory)
    : ApiControllerBase(sender, actorFactory)
{
    [AllowAnonymous]
    [HttpPost("login")]
    public async Task<IActionResult> LoginAsync(LoginCommandDTO command)
        => await HandleRequestForAnonymous(new Login.Command(command));

    [AllowAnonymous]
    [HttpPost("signup")]
    public async Task<IActionResult> SignUp(SignUpCommandDTO command)
        => await HandleRequestForAnonymous(new SignUp.Command(command));

    [HttpGet]
    public async Task<IActionResult> GetUserList([FromQuery] UserQueryDTO queryFields)
        => await HandleRequest(actor => new GetUserList.Query(actor, queryFields));

    [HttpGet("me")]
    public async Task<IActionResult> GetMyUserInfo()
        => await HandleRequest(actor => new GetUser.Query(actor, actor.UserId));

    [HttpGet("{userId}")]
    public async Task<IActionResult> GetUserInfo(string userId)
        => await HandleRequest(actor => new GetUser.Query(actor, userId));

    [HttpPut("me/username")]
    public async Task<IActionResult> ChangeMyUserName(UserNameCommandDTO command)
        => await HandleRequest(actor => new ChangeUserName.Command(actor, command));

    [HttpPut("me/password")]
    public async Task<IActionResult> ChangeMyPassword(UserPasswordCommandDTO command)
        => await HandleRequest(actor => new ChangeUserPassword.Command(actor, command));
}
