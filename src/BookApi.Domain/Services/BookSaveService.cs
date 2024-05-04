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
        await VerifySameISBNNotExistAsync(command.ISBN, origin: book);
        await VerifyAllAuthorsExistedAsync(permission.Actor, command.AuthorIds);
        await VerifyPublisherExistedAsync(permission.Actor, command.PublisherId);

        book.Update(permission, dateTimeProvider, command);
        await bookRepository.SaveAsync(permission.Actor, book);
    }

    private async Task VerifyAllAuthorsExistedAsync(IActor actor, int[] authorIds)
    {
        if (!await authorRepository.IsAllIdsExistedAsync(actor, [.. authorIds]))
            throw new ValidationErrorException("存在しない著者IDが含まれています。");
    }

    private async Task VerifyPublisherExistedAsync(IActor actor, int publisherId)
    {
        if (!await publisherRepository.AnyAsync(actor, publisherId))
            throw new ValidationErrorException("存在しない出版社IDが指定されています。");
    }

    private async Task VerifySameISBNNotExistAsync(string isbn, Book? origin = null)
    {
        var isbnCode = ISBNCode.CreateWithValidation(isbn);
        if (await bookRepository.AnyByISBNAsync(isbnCode, origin is { } ? origin.ISBN : null))
            throw new ValidationErrorException("既に同じISBNコードの書籍が存在します。");
    }
}
