using BookApi.Domain.DTOs.Commands;
using BookApi.Domain.DTOs.Queries;
using BookApi.Presentation.Abstractions.Controllers;
using BookApi.Presentation.Services;
using BookApi.UseCase.Authors;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace BookApi.Presentation.Controllers;

public class AuthorsController(ISender sender, ActorFactoryService actorFactory)
    : ApiControllerBase(sender, actorFactory)
{
    [HttpGet, AllowAnonymous]
    public async Task<IActionResult> GetAuthorList([FromQuery] AuthorQueryDTO queryFields)
        => await HandleRequestForView(actor => new GetAuthorList.Query(actor, queryFields));

    [HttpGet("{authorId}"), AllowAnonymous]
    public async Task<IActionResult> GetAuthor(int authorId)
        => await HandleRequestForView(actor => new GetAuthor.Query(actor, authorId));

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
