using BookApi.Domain.Exceptions;
using BookApi.Domain.ValueObjects.Shared;
using BookApi.Presentation.Services;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace BookApi.Presentation.Abstractions.Controllers;

[ApiController, Route("/api/[controller]"), Authorize]
public abstract class ApiControllerBase(ISender sender, ActorFactoryService actorFactory) : ControllerBase
{
    private readonly ISender Mediator = sender;

    protected async Task<IActionResult> HandleRequest<T>(Func<Actor, T> requestFunc)
        where T : IBaseRequest
        => await HandleActionAsync(async () =>
        {
            var actor =
                await actorFactory.TryGetActorAsync() ?? throw new UnauthorizedException();
            return await Mediator.Send(requestFunc(actor));
        });

    protected async Task<IActionResult> HandleRequestForView<T>(Func<Actor?, T> requestFunc)
        where T : IBaseRequest
        => await HandleActionAsync(async () =>
        {
            var actor = await actorFactory.TryGetActorAsync();
            return await Mediator.Send(requestFunc(actor));
        });

    protected async Task<IActionResult> HandleRequestForAnonymous<T>(T request)
        where T : IBaseRequest
        => await HandleActionAsync(async () => await Mediator.Send(request));

    protected async Task<IActionResult> HandleActionAsync<T>(Func<Task<T>> action)
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
        catch (UnauthorizedException)
        {
            return Unauthorized();
        }
        catch (ConcurrencyException)
        {
            return Conflict();
        }
    }
}
