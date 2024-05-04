using BookApi.Domain.Abstractions.ValueObjects;
using BookApi.Domain.DTOs.Commands;
using BookApi.Domain.Entities;
using BookApi.Domain.Exceptions;
using BookApi.Domain.Interfaces;
using BookApi.Domain.ValueObjects.Books;
using BookApi.Domain.ValueObjects.Shared;

namespace BookApi.Domain.Services;

public class BookSaveService(
    IBookRepository bookRepository,
    IAuthorRepository authorRepository,
    IPublisherRepository publisherRepository,
    IDateTimeProvider dateTimeProvider
)
{
    public async Task<Book> CreateAsync(AdminOnlyPermission permission, BookCommandDTO command)
    {
        await VerifySameISBNNotExistAsync(command.ISBN);
        await VerifyAllAuthorsExistedAsync(permission.Actor, command.AuthorIds);
        await VerifyPublisherExistedAsync(permission.Actor, command.PublisherId);

        var newBook = Book.CreateNew(permission, dateTimeProvider, command);
        await bookRepository.SaveAsync(permission.Actor, newBook);
        return newBook;
    }

    public async Task UpdateAsync(AdminOnlyPermission permission, Book book, BookCommandDTO command)
    {
        await VerifySameISBNNotExistAsync(command.ISBN, existedEntity: book);
        await VerifyAllAuthorsExistedAsync(permission.Actor, command.AuthorIds);
        await VerifyPublisherExistedAsync(permission.Actor, command.PublisherId);

        book.Update(permission, dateTimeProvider, command);
        await bookRepository.SaveAsync(permission.Actor, book);
    }

    private async Task VerifyAllAuthorsExistedAsync(IActor actor, IEnumerable<int> authorIds)
    {
        HashSet<ItemId> authorItemIds = [.. authorIds.Select(x => ItemId.CreateWithValidation(x))];
        if (!await authorRepository.IsAllIdsExistedAsync(actor, authorItemIds))
            throw new ValidationErrorException("存在しない著者IDが含まれています。");
    }

    private async Task VerifyPublisherExistedAsync(IActor actor, int publisherId)
    {
        if (!await publisherRepository.AnyAsync(actor, ItemId.CreateWithValidation(publisherId)))
            throw new ValidationErrorException("存在しない出版社IDが指定されています。");
    }

    private async Task VerifySameISBNNotExistAsync(string isbn, Book? existedEntity = null)
    {
        var isbnCode = ISBNCode.CreateWithValidation(isbn);
        if (await bookRepository.AnyByISBNAsync(isbnCode, existedEntity is { } ? existedEntity.ISBN : null))
            throw new ValidationErrorException("既に同じISBNコードの書籍が存在します。");
    }
}
