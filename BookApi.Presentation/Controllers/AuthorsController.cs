using BookApi.Domain.DTOs.Commands;
using BookApi.Presentation.Abstractions.Controllers;
using BookApi.UseCase.Authors;
using MediatR;
using Microsoft.AspNetCore.Mvc;

namespace AuthorApi.Presentation.Controllers;

public class AuthorsController(ISender sender) : ApiControllerBase(sender)
{
    [HttpGet("{authorID}")]
    public async Task<IActionResult> GetAuthor(int authorID)
        => await HandleRequest(actor => new GetAuthor.Query(actor, authorID));

    [HttpPost]
    public async Task<IActionResult> CreateAuthor(AuthorCommandDTO command)
        => await HandleRequest(actor => new CreateAuthor.Command(actor, command));

    [HttpPut("{authorID}")]
    public async Task<IActionResult> UpdateAuthor(int authorID, AuthorCommandDTO command)
        => await HandleRequest(actor => new UpdateAuthor.Command(actor, authorID, command));

    [HttpDelete("{authorID}")]
    public async Task<IActionResult> DeleteAuthor(int authorID)
        => await HandleRequest(actor => new DeleteAuthor.Command(actor, authorID));
}
