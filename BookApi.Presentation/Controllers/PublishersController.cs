using BookApi.Domain.DTOs.Commands;
using BookApi.Presentation.Abstractions.Controllers;
using BookApi.UseCase.Publishers;
using MediatR;
using Microsoft.AspNetCore.Mvc;

namespace BookApi.Presentation.Controllers;

public class PublishersController(ISender sender) : ApiControllerBase(sender)
{
    [HttpGet("{publisherID}")]
    public async Task<IActionResult> GetPublisher(int publisherID)
        => await HandleRequest(actor => new GetPublisher.Query(actor, publisherID));

    [HttpPost]
    public async Task<IActionResult> CreatePublisher(PublisherCommandDTO command)
        => await HandleRequest(actor => new CreatePublisher.Command(actor, command));

    [HttpPut("{publisherID}")]
    public async Task<IActionResult> UpdatePublisher(int publisherID, PublisherCommandDTO command)
        => await HandleRequest(actor => new UpdatePublisher.Command(actor, publisherID, command));

    [HttpDelete("{publisherID}")]
    public async Task<IActionResult> DeletePublisher(int publisherID)
        => await HandleRequest(actor => new DeletePublisher.Command(actor, publisherID));
}
