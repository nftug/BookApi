using BookApi.Domain.DTOs.Commands;
using BookApi.Domain.DTOs.Queries;
using BookApi.Presentation.Abstractions.Controllers;
using BookApi.Presentation.Services;
using BookApi.UseCase.Publishers;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace BookApi.Presentation.Controllers;

public class PublishersController(ISender sender, ActorFactoryService actorFactory)
    : ApiControllerBase(sender, actorFactory)
{
    [HttpGet, AllowAnonymous]
    public async Task<IActionResult> GetPublisherList([FromQuery] PublisherQueryDTO queryFields)
        => await HandleRequestForView(actor => new GetPublisherList.Query(actor, queryFields));

    [HttpGet("{publisherId}"), AllowAnonymous]
    public async Task<IActionResult> GetPublisher(int publisherId)
        => await HandleRequestForView(actor => new GetPublisher.Query(actor, publisherId));

    [HttpPost]
    public async Task<IActionResult> CreatePublisher(PublisherCommandDTO command)
        => await HandleRequest(actor => new CreatePublisher.Command(actor, command));

    [HttpPut("{publisherId}")]
    public async Task<IActionResult> UpdatePublisher(int publisherId, PublisherCommandDTO command)
        => await HandleRequest(actor => new UpdatePublisher.Command(actor, publisherId, command));

    [HttpDelete("{publisherId}")]
    public async Task<IActionResult> DeletePublisher(int publisherId)
        => await HandleRequest(actor => new DeletePublisher.Command(actor, publisherId));
}
