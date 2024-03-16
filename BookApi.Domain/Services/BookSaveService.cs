using BookApi.Domain.Abstractions.ValueObjects;
using BookApi.Domain.Entities;
using BookApi.Domain.Exceptions;
using BookApi.Domain.Interfaces;
using BookApi.Domain.ValueObjects.Books;
using BookApi.Domain.ValueObjects.Shared;

namespace BookApi.Domain.Services;

public class BookSaveService(
    IBookRepository bookRepository,
    IAuthorRepository authorRepository,
    IPublisherRepository publisherRepository
)
{
    public async Task<Book> CreateAsync(
        AdminOnlyPermission permission,
        string name, string isbn, int[] authorIds, int publisherId, DateTime publishedAt
    )
    {
        await ValidateAllAuthorsExistedAsync(permission.Actor, authorIds);
        await ValidatePublisherExistedAsync(permission.Actor, publisherId);

        var isbnCode = ISBNCode.CreateWithValidation(isbn);
        if (await bookRepository.AnyByISBNAsync(isbnCode))
            throw new ValidationErrorException("既に同じISBNコードの書籍が存在します。");

        var newBook = Book.CreateNew(permission, name, isbn, authorIds, publisherId, publishedAt);

        await bookRepository.SaveAsync(permission.Actor, newBook);
        return newBook;
    }

    public async Task UpdateAsync(
        AdminOnlyPermission permission,
        Book book,
        string name, string isbn, int[] authorIds, int publisherId, DateTime publishedAt
    )
    {
        await ValidateAllAuthorsExistedAsync(permission.Actor, authorIds);
        await ValidatePublisherExistedAsync(permission.Actor, publisherId);

        book.Update(permission, name, isbn, authorIds, publisherId, publishedAt);

        await bookRepository.SaveAsync(permission.Actor, book);
    }

    private async Task ValidateAllAuthorsExistedAsync(IActor actor, int[] authorIds)
    {
        if (!await authorRepository.IsAllIdsExistedAsync(actor, [.. authorIds]))
            throw new ValidationErrorException("存在しない著者IDが含まれています。");
    }

    private async Task ValidatePublisherExistedAsync(IActor actor, int publisherId)
    {
        if (!await publisherRepository.AnyAsync(actor, publisherId))
            throw new ValidationErrorException("存在しない出版社IDが指定されています。");
    }
}
