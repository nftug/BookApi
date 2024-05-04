using BookApi.Domain.DTOs.Commands;
using BookApi.Domain.Entities;
using BookApi.Domain.Exceptions;
using BookApi.Domain.Interfaces;
using BookApi.Domain.ValueObjects.Shared;

namespace BookApi.Domain.Services;

public class AuthorSaveService(IAuthorRepository authorRepository, IDateTimeProvider dateTimeProvider)
{
    public async Task<Author> CreateAsync(AdminOnlyPermission permission, AuthorCommandDTO command)
    {
        await VerifySameNameItemNotExistAsync(command.Name);

        var newAuthor = Author.CreateNew(permission, dateTimeProvider, command);
        await authorRepository.SaveAsync(permission.Actor, newAuthor);
        return newAuthor;
    }

    public async Task UpdateAsync(AdminOnlyPermission permission, Author author, AuthorCommandDTO command)
    {
        await VerifySameNameItemNotExistAsync(command.Name, excludedItemId: author.Id);

        author.Update(permission, dateTimeProvider, command);
        await authorRepository.SaveAsync(permission.Actor, author);
    }

    private async Task VerifySameNameItemNotExistAsync(string name, int? excludedItemId = null)
    {
        if (await authorRepository.AnyByNameAsync(name, excludedItemId))
            throw new ValidationErrorException("既に同じ名前の著者が存在します。");
    }
}
