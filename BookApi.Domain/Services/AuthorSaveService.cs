using BookApi.Domain.Entities;
using BookApi.Domain.Exceptions;
using BookApi.Domain.Interfaces;
using BookApi.Domain.ValueObjects.Shared;

namespace BookApi.Domain.Services;

public class AuthorSaveService(IAuthorRepository authorRepository, IDateTimeProvider dateTimeProvider)
{
    public async Task<Author> CreateAsync(AdminOnlyPermission permission, string name)
    {
        if (await authorRepository.AnyByNameAsync(name))
            throw new ValidationErrorException("既に同じ名前の著者が存在します。");

        var newAuthor = Author.CreateNew(permission, dateTimeProvider, name);

        await authorRepository.SaveAsync(permission.Actor, newAuthor);
        return newAuthor;
    }

    public async Task UpdateAsync(AdminOnlyPermission permission, Author author, string name)
    {
        // NOTE: 今のところはインフラ層に頼るバリデーションなどはないが、後々の拡張性を考慮して実装しておく
        author.Update(permission, dateTimeProvider, name);
        await authorRepository.SaveAsync(permission.Actor, author);
    }
}
