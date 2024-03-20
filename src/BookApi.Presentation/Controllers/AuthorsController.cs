using BookApi.Domain.Abstractions.DTOs;
using BookApi.Domain.DTOs.Commands;
using BookApi.Domain.DTOs.Queries;
using BookApi.Domain.DTOs.Responses;
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
    [ProducesResponseType(typeof(PaginationResponseDTO<AuthorSummaryResponseDTO>), 200)]
    public async Task<IActionResult> GetAuthorList([FromQuery] AuthorQueryDTO queryFields)
        => await HandleRequestForView(actor => new GetAuthorList.Query(actor, queryFields));

    [HttpGet("{authorId}"), AllowAnonymous]
    [ProducesResponseType(typeof(AuthorResponseDTO), 200)]
    public async Task<IActionResult> GetAuthor(int authorId)
        => await HandleRequestForView(actor => new GetAuthor.Query(actor, authorId));

    [HttpPost]
    [ProducesResponseType(typeof(ItemCreationResponseDTO), 200)]
    public async Task<IActionResult> CreateAuthor(AuthorCommandDTO command)
        => await HandleRequest(actor => new CreateAuthor.Command(actor, command));

    [HttpPut("{authorId}")]
    [ProducesResponseType(204)]
    public async Task<IActionResult> UpdateAuthor(int authorId, AuthorCommandDTO command)
        => await HandleRequest(actor => new UpdateAuthor.Command(actor, authorId, command));

    [HttpDelete("{authorId}")]
    [ProducesResponseType(204)]
    public async Task<IActionResult> DeleteAuthor(int authorId)
        => await HandleRequest(actor => new DeleteAuthor.Command(actor, authorId));
}
