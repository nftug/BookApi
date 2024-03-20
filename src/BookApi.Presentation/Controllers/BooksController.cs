using BookApi.Domain.Abstractions.DTOs;
using BookApi.Domain.DTOs.Commands;
using BookApi.Domain.DTOs.Queries;
using BookApi.Domain.DTOs.Responses;
using BookApi.Presentation.Abstractions.Controllers;
using BookApi.Presentation.Services;
using BookApi.UseCase.Books;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace BookApi.Presentation.Controllers;

public class BooksController(ISender sender, ActorFactoryService actorFactory)
    : ApiControllerBase(sender, actorFactory)
{
    [HttpGet, AllowAnonymous]
    [ProducesResponseType(typeof(PaginationResponseDTO<BookSummaryResponseDTO>), 200)]
    public async Task<IActionResult> GetBookList([FromQuery] BookQueryDTO queryFields)
        => await HandleRequestForView(actor => new GetBookList.Query(actor, queryFields));

    [HttpGet("{isbn}"), AllowAnonymous]
    [ProducesResponseType(typeof(BookResponseDTO), 200)]
    public async Task<IActionResult> GetBook(string isbn)
        => await HandleRequestForView(actor => new GetBook.Query(actor, isbn));

    [HttpPost]
    [ProducesResponseType(typeof(BookCreationResponseDTO), 200)]
    public async Task<IActionResult> CreateBook(BookCommandDTO command)
        => await HandleRequest(actor => new CreateBook.Command(actor, command));

    [HttpPut("{isbn}")]
    [ProducesResponseType(204)]
    public async Task<IActionResult> UpdateBook(string isbn, BookCommandDTO command)
        => await HandleRequest(actor => new UpdateBook.Command(actor, isbn, command));

    [HttpDelete("{isbn}")]
    [ProducesResponseType(204)]
    public async Task<IActionResult> DeleteBook(string isbn)
        => await HandleRequest(actor => new DeleteBook.Command(actor, isbn));

    // BookLike
    [HttpGet("{isbn}/likes"), AllowAnonymous]
    [ProducesResponseType(typeof(PaginationResponseDTO<BookLikeListItemResponseDTO>), 200)]
    public async Task<IActionResult> GetLikes(string isbn, [FromQuery] BookLikeQueryDTO queryFields)
        => await HandleRequestForView(actor => new GetBookLikeList.Query(actor, isbn, queryFields));

    [HttpPost("{isbn}/likes")]
    [ProducesResponseType(typeof(BookLikeResponseDTO), 200)]
    public async Task<IActionResult> ToggleLike(string isbn)
        => await HandleRequest(actor => new ToggleBookLike.Command(actor, isbn));
}
