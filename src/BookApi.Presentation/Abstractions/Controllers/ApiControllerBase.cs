using BookApi.Domain.Exceptions;
using BookApi.Domain.ValueObjects.Shared;
using MediatR;
using Microsoft.AspNetCore.Mvc;

namespace BookApi.Presentation.Abstractions.Controllers;

[ApiController, Route("/api/[controller]")]
public abstract class ApiControllerBase(ISender sender) : ControllerBase
{
    private readonly ISender Mediator = sender;

    protected async Task<IActionResult> HandleRequest<T>(Func<ActorForPermission, T> requestFunc)
        where T : IBaseRequest
    {
        var dummyActor = new ActorForPermission(ItemId.Reconstruct(1), "Admin", true);
        return await HandleActionAsync(() => Mediator.Send(requestFunc(dummyActor)));
    }

    protected async Task<IActionResult> HandleRequestForAnonymous<T>(T request)
        where T : IBaseRequest
        => await HandleActionAsync(() => Mediator.Send(request));

    private async Task<IActionResult> HandleActionAsync<T>(Func<Task<T>> action)
    {
        try
        {
            var result = await action();

            return result switch
            {
                T content => Ok(content),
                _ => NoContent()
            };
        }
        catch (ValidationErrorException validationErrorException)
        {
            ModelState.AddModelError("error", validationErrorException.Message);
            return BadRequest(ModelState);
        }
        catch (ItemNotFoundException)
        {
            return NotFound();
        }
        catch (ForbiddenException)
        {
            return Forbid();
        }
        catch (ConcurrencyException)
        {
            return Conflict();
        }
    }
}
