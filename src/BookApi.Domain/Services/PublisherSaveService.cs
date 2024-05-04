using BookApi.Domain.DTOs.Commands;
using BookApi.Domain.Entities;
using BookApi.Domain.Exceptions;
using BookApi.Domain.Interfaces;
using BookApi.Domain.ValueObjects.Shared;

namespace BookApi.Domain.Services;

public class PublisherSaveService(IPublisherRepository publisherRepository, IDateTimeProvider dateTimeProvider)
{
    public async Task<Publisher> CreateAsync(AdminOnlyPermission permission, PublisherCommandDTO command)
    {
        await VerifySameNameItemNotExistAsync(command.Name);

        var newPublisher = Publisher.CreateNew(permission, dateTimeProvider, command);
        await publisherRepository.SaveAsync(permission.Actor, newPublisher);
        return newPublisher;
    }

    public async Task UpdateAsync(AdminOnlyPermission permission, Publisher publisher, PublisherCommandDTO command)
    {
        await VerifySameNameItemNotExistAsync(command.Name, exitedEntity: publisher);

        publisher.Update(permission, dateTimeProvider, command);
        await publisherRepository.SaveAsync(permission.Actor, publisher);
    }

    private async Task VerifySameNameItemNotExistAsync(string name, Publisher? exitedEntity = null)
    {
        if (await publisherRepository.AnyByNameAsync(name, exitedEntity?.ItemId))
            throw new ValidationErrorException("既に同じ名前の出版社が存在します。");
    }
}
