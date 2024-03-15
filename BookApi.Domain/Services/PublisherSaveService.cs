using BookApi.Domain.Entities;
using BookApi.Domain.Exceptions;
using BookApi.Domain.Interfaces;
using BookApi.Domain.ValueObjects.Shared;

namespace BookApi.Domain.Services;

public class PublisherSaveService(IPublisherRepository PublisherRepository)
{
    public async Task<ItemID> CreateAsync(AdminOnlyPermission permission, string name)
    {
        if (await PublisherRepository.AnyByNameAsync(name))
            throw new ValidationErrorException("既に同じ名前の出版社が存在します。");

        var newPublisher = Publisher.CreateNew(permission, name);

        await PublisherRepository.SaveAsync(permission.Actor, newPublisher);
        return newPublisher.ItemID;
    }

    public async Task UpdateAsync(AdminOnlyPermission permission, Publisher Publisher, string name)
    {
        // NOTE: 今のところはインフラ層に頼るバリデーションなどはないが、後々の拡張性を考慮して実装しておく
        Publisher.Update(permission, name);
        await PublisherRepository.SaveAsync(permission.Actor, Publisher);
    }
}
