using BookApi.Domain.DTOs.Commands;
using BookApi.Presentation.Abstractions.Controllers;
using BookApi.UseCase.Authors;
using MediatR;
using Microsoft.AspNetCore.Mvc;

namespace AuthorApi.Presentation.Controllers;

public class AuthorsController(ISender sender) : ApiControllerBase(sender)
{
    [HttpGet("{authorId}")]
    public async Task<IActionResult> GetAuthor(int authorId)
        => await HandleRequest(actor => new GetAuthor.Query(actor, authorId));

    [HttpPost]
    public async Task<IActionResult> CreateAuthor(AuthorCommandDTO command)
        => await HandleRequest(actor => new CreateAuthor.Command(actor, command));

    [HttpPut("{authorId}")]
    public async Task<IActionResult> UpdateAuthor(int authorId, AuthorCommandDTO command)
        => await HandleRequest(actor => new UpdateAuthor.Command(actor, authorId, command));

    [HttpDelete("{authorId}")]
    public async Task<IActionResult> DeleteAuthor(int authorId)
        => await HandleRequest(actor => new DeleteAuthor.Command(actor, authorId));
}
