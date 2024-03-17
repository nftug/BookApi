using BookApi.Domain.Entities;
using BookApi.Domain.Exceptions;
using BookApi.Domain.Interfaces;
using BookApi.Domain.ValueObjects.Shared;

namespace BookApi.Domain.Services;

public class PublisherSaveService(IPublisherRepository publisherRepository, IDateTimeProvider dateTimeProvider)
{
    public async Task<Publisher> CreateAsync(AdminOnlyPermission permission, string name)
    {
        await VerifySameNameItemNotExistAsync(name);

        var newPublisher = Publisher.CreateNew(permission, dateTimeProvider, name);
        await publisherRepository.SaveAsync(permission.Actor, newPublisher);
        return newPublisher;
    }

    public async Task UpdateAsync(AdminOnlyPermission permission, Publisher publisher, string name)
    {
        await VerifySameNameItemNotExistAsync(name, excludedItemId: publisher.Id);

        publisher.Update(permission, dateTimeProvider, name);
        await publisherRepository.SaveAsync(permission.Actor, publisher);
    }

    private async Task VerifySameNameItemNotExistAsync(string name, int? excludedItemId = null)
    {
        if (await publisherRepository.AnyByNameAsync(name, excludedItemId))
            throw new ValidationErrorException("既に同じ名前の出版社が存在します。");
    }
}
