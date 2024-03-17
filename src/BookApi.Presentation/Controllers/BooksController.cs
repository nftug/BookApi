using BookApi.Domain.DTOs.Commands;
using BookApi.Presentation.Abstractions.Controllers;
using BookApi.UseCase.Books;
using MediatR;
using Microsoft.AspNetCore.Mvc;

namespace BookApi.Presentation.Controllers;

public class BooksController(ISender sender) : ApiControllerBase(sender)
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
}
