using BookApi.Domain.Abstractions.ValueObjects;
using BookApi.Domain.Entities;
using BookApi.Domain.Exceptions;
using BookApi.Domain.Interfaces;
using BookApi.Domain.ValueObjects.Shared;

namespace BookApi.Domain.Services;

public class BookSaveService(
    IBookRepository bookRepository,
    IAuthorRepository authorRepository,
    IPublisherRepository publisherRepository
)
{
    public async Task<ItemID> CreateAsync(
        AdminOnlyPermission permission,
        string name, string isbn, int[] authorIDs, int publisherID, DateTime publishedAt
    )
    {
        await ValidateAllAuthorsExistedAsync(permission.Actor, authorIDs);
        await ValidatePublisherExistedAsync(permission.Actor, publisherID);

        if (await bookRepository.AnyByISBNAsync(isbn))
            throw new ValidationErrorException("既に同じISBNコードの書籍が存在します。");

        var newBook = Book.CreateNew(permission, name, isbn, authorIDs, publisherID, publishedAt);

        await bookRepository.SaveAsync(permission.Actor, newBook);
        return newBook.ItemID;
    }

    public async Task UpdateAsync(
        AdminOnlyPermission permission,
        Book book,
        string name, string isbn, int[] authorIDs, int publisherID, DateTime publishedAt
    )
    {
        await ValidateAllAuthorsExistedAsync(permission.Actor, authorIDs);
        await ValidatePublisherExistedAsync(permission.Actor, publisherID);

        book.Update(permission, name, isbn, authorIDs, publisherID, publishedAt);

        await bookRepository.SaveAsync(permission.Actor, book);
    }

    private async Task ValidateAllAuthorsExistedAsync(IActor actor, int[] authorIDs)
    {
        if (!await authorRepository.IsAllIDsExistedAsync(actor, [.. authorIDs]))
            throw new ValidationErrorException("存在しない著者IDが含まれています。");
    }

    private async Task ValidatePublisherExistedAsync(IActor actor, int publisherID)
    {
        if (!await publisherRepository.AnyAsync(actor, publisherID))
            throw new ValidationErrorException("存在しない出版社IDが指定されています。");
    }
}
