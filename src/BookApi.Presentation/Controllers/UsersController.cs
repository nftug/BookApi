using BookApi.Domain.DTOs.Commands;
using BookApi.Domain.DTOs.Queries;
using BookApi.Presentation.Abstractions.Controllers;
using BookApi.Presentation.Services;
using BookApi.UseCase.Books;
using BookApi.UseCase.Users;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace BookApi.Presentation.Controllers;

public class UsersController(ISender sender, ActorFactoryService actorFactory)
    : ApiControllerBase(sender, actorFactory)
{
    [HttpPost("login"), AllowAnonymous]
    public async Task<IActionResult> LoginAsync(LoginCommandDTO command)
        => await HandleRequestForAnonymous(new Login.Command(command));

    [HttpPost("signup"), AllowAnonymous]
    public async Task<IActionResult> SignUp(SignUpCommandDTO command)
        => await HandleRequestForAnonymous(new SignUp.Command(command));

    [HttpGet, AllowAnonymous]
    public async Task<IActionResult> GetUserList([FromQuery] UserQueryDTO queryFields)
        => await HandleRequestForView(actor => new GetUserList.Query(actor, queryFields));

    [HttpGet("me")]
    public async Task<IActionResult> GetMyUserInfo()
        => await HandleRequest(actor => new GetUser.Query(actor, actor.UserId));

    [HttpGet("{userId}"), AllowAnonymous]
    public async Task<IActionResult> GetUserInfo(string userId)
        => await HandleRequestForView(actor => new GetUser.Query(actor, userId));

    [HttpPut("me/username")]
    public async Task<IActionResult> ChangeMyUserName(UserNameCommandDTO command)
        => await HandleRequest(actor => new ChangeUserName.Command(actor, command));

    [HttpPut("me/password")]
    public async Task<IActionResult> ChangeMyPassword(UserPasswordCommandDTO command)
        => await HandleRequest(actor => new ChangeUserPassword.Command(actor, command));

    // Liked books
    [HttpGet("me/likes/books")]
    public async Task<IActionResult> GetMyLikedBooks([FromQuery] BookQueryDTO queryFields)
        => await HandleRequest(actor => new GetLikedBookList.Query(actor, actor.UserId, queryFields));

    [HttpGet("{userId}/likes/books"), AllowAnonymous]
    public async Task<IActionResult> GetUsersLikedBooks(string userId, [FromQuery] BookQueryDTO queryFields)
        => await HandleRequestForView(actor => new GetLikedBookList.Query(actor, userId, queryFields));

    [HttpPost("{userId}/likes/books/{isbn}")]
    public async Task<IActionResult> EditUsersBookLike(string userId, string isbn, BookLikeEditCommandDTO command)
        => await HandleRequest(actor => new EditBookLike.Command(actor, userId, isbn, command));
}
