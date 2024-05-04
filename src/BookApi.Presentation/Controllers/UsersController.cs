using BookApi.Domain.Abstractions.DTOs;
using BookApi.Domain.DTOs.Commands;
using BookApi.Domain.DTOs.Queries;
using BookApi.Domain.DTOs.Responses;
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
    [ProducesResponseType(typeof(LoginResponseDTO), 200)]
    public async Task<IActionResult> LoginAsync(LoginCommandDTO command)
        => await HandleRequestForAnonymous(new Login.Command(command));

    [HttpPost("signup"), AllowAnonymous]
    [ProducesResponseType(typeof(LoginResponseDTO), 200)]
    public async Task<IActionResult> SignUp(SignUpCommandDTO command)
        => await HandleRequestForAnonymous(new SignUp.Command(command));

    [HttpGet, AllowAnonymous]
    [ProducesResponseType(typeof(PaginationResponseDTO<UserSummaryResponseDTO>), 200)]
    public async Task<IActionResult> GetUserList([FromQuery] UserQueryDTO queryFields)
        => await HandleRequestForView(actor => new GetUserList.Query(actor, queryFields));

    [HttpGet("me")]
    [ProducesResponseType(typeof(UserResponseDTO), 200)]
    public async Task<IActionResult> GetMyUserInfo()
        => await HandleRequest(actor => new GetUser.Query(actor, actor.UserId.Value));

    [HttpGet("{userId}"), AllowAnonymous]
    [ProducesResponseType(typeof(UserResponseDTO), 200)]
    public async Task<IActionResult> GetUserInfo(string userId)
        => await HandleRequestForView(actor => new GetUser.Query(actor, userId));

    [HttpPut("me/username")]
    [ProducesResponseType(204)]
    public async Task<IActionResult> ChangeMyUserName(UserNameCommandDTO command)
        => await HandleRequest(actor => new ChangeUserName.Command(actor, command));

    [HttpPut("me/password")]
    [ProducesResponseType(204)]
    public async Task<IActionResult> ChangeMyPassword(UserPasswordCommandDTO command)
        => await HandleRequest(actor => new ChangeUserPassword.Command(actor, command));

    [HttpDelete("me")]
    [ProducesResponseType(204)]
    public async Task<IActionResult> DeleteMyAccount()
        => await HandleRequest(actor => new DeleteUser.Command(actor, actor.UserId.Value));

    [HttpDelete("{userId}")]
    [ProducesResponseType(204)]
    public async Task<IActionResult> DeleteUserAccount(string userId)
        => await HandleRequest(actor => new DeleteUser.Command(actor, userId));

    // Liked books
    [HttpGet("me/likes/books")]
    [ProducesResponseType(typeof(PaginationResponseDTO<BookListItemResponseDTO>), 200)]
    public async Task<IActionResult> GetMyLikedBooks([FromQuery] BookQueryDTO queryFields)
        => await HandleRequest(actor => new GetLikedBookList.Query(actor, actor.UserId.Value, queryFields));

    [HttpGet("{userId}/likes/books"), AllowAnonymous]
    [ProducesResponseType(typeof(PaginationResponseDTO<BookListItemResponseDTO>), 200)]
    public async Task<IActionResult> GetUsersLikedBooks(string userId, [FromQuery] BookQueryDTO queryFields)
        => await HandleRequestForView(actor => new GetLikedBookList.Query(actor, userId, queryFields));

    [HttpPost("{userId}/likes/books/{isbn}")]
    [ProducesResponseType(204)]
    public async Task<IActionResult> EditUsersBookLike(string userId, string isbn, BookLikeEditCommandDTO command)
        => await HandleRequest(actor => new EditBookLike.Command(actor, userId, isbn, command));
}
