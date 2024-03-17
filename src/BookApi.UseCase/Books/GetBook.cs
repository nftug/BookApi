using BookApi.Domain.DTOs.Responses;
using BookApi.Domain.Exceptions;
using BookApi.Domain.Interfaces;
using BookApi.Domain.ValueObjects.Books;
using BookApi.Domain.ValueObjects.Shared;
using MediatR;

namespace BookApi.UseCase.Books;

public class GetBook
{
    public record Query(ActorForPermission Actor, string ISBN) : IRequest<BookResponseDTO>;

    public class Handler(IBookQueryService bookQueryService) : IRequestHandler<Query, BookResponseDTO>
    {
        public async Task<BookResponseDTO> Handle(Query request, CancellationToken cancellationToken)
        {
            var isbn = ISBNCode.CreateWithValidation(request.ISBN);

            return await bookQueryService.FindByISBNAsync(request.Actor, isbn)
                ?? throw new ItemNotFoundException();
        }
    }
}
