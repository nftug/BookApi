using BookApi.Domain.DTOs.Commands;
using BookApi.Presentation.Abstractions.Controllers;
using BookApi.Presentation.Services;
using BookApi.UseCase.Books;
using MediatR;
using Microsoft.AspNetCore.Mvc;

namespace BookApi.Presentation.Controllers;

public class BooksController(ISender sender, ActorFactoryService actorFactory)
    : ApiControllerBase(sender, actorFactory)
{
    [HttpGet("{isbn}")]
    public async Task<IActionResult> GetBook(string isbn)
        => await HandleRequest(actor => new GetBook.Query(actor, isbn));

    [HttpPost]
    public async Task<IActionResult> CreateBook(BookCommandDTO command)
        => await HandleRequest(actor => new CreateBook.Command(actor, command));

    [HttpPut("{isbn}")]
    public async Task<IActionResult> UpdateBook(string isbn, BookCommandDTO command)
        => await HandleRequest(actor => new UpdateBook.Command(actor, isbn, command));

    [HttpDelete("{isbn}")]
    public async Task<IActionResult> DeleteBook(string isbn)
        => await HandleRequest(actor => new DeleteBook.Command(actor, isbn));

    [HttpPost("{isbn}/likes")]
    public async Task<IActionResult> ToggleLike(string isbn)
        => await HandleRequest(actor => new ToggleBookLike.Command(actor, isbn));

    [HttpPost("{isbn}/likes/users/{userId}")]
    public async Task<IActionResult> EditLike(string isbn, string userId, BookLikeEditCommandDTO command)
        => await HandleRequest(actor => new EditBookLike.Command(actor, isbn, userId, command));
}
