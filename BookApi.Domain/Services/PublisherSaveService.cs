using BookApi.Domain.Entities;
using BookApi.Domain.Exceptions;
using BookApi.Domain.Interfaces;
using BookApi.Domain.ValueObjects.Shared;

namespace BookApi.Domain.Services;

public class PublisherSaveService(IPublisherRepository publisherRepository)
{
    public async Task<Publisher> CreateAsync(AdminOnlyPermission permission, string name)
    {
        if (await publisherRepository.AnyByNameAsync(name))
            throw new ValidationErrorException("既に同じ名前の出版社が存在します。");

        var newPublisher = Publisher.CreateNew(permission, name);

        await publisherRepository.SaveAsync(permission.Actor, newPublisher);
        return newPublisher;
    }

    public async Task UpdateAsync(AdminOnlyPermission permission, Publisher publisher, string name)
    {
        // NOTE: 今のところはインフラ層に頼るバリデーションなどはないが、後々の拡張性を考慮して実装しておく
        publisher.Update(permission, name);
        await publisherRepository.SaveAsync(permission.Actor, publisher);
    }
}
