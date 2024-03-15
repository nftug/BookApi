using BookApi.Domain.DTOs.Commands;
using BookApi.Domain.DTOs.Responses;
using BookApi.Domain.Services;
using BookApi.Domain.ValueObjects.Shared;
using MediatR;

namespace BookApi.UseCase.Authors;

public class CreateAuthor
{
    public record Command(ActorForPermission Actor, AuthorCommandDTO CommandForm)
        : IRequest<ItemCreationResponseDTO>;

    public class Handler(AuthorSaveService authorSaveService) : IRequestHandler<Command, ItemCreationResponseDTO>
    {
        public async Task<ItemCreationResponseDTO> Handle(Command request, CancellationToken cancellationToken)
        {
            var permission = new AdminOnlyPermission(request.Actor);
            var newAuthor = await authorSaveService.CreateAsync(permission, request.CommandForm.Name);
            return new(newAuthor.ID);
        }
    }
}
