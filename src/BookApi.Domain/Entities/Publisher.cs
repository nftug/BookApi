using BookApi.Domain.Abstractions.Entities;
using BookApi.Domain.DTOs.Commands;
using BookApi.Domain.Interfaces;
using BookApi.Domain.ValueObjects.Books;
using BookApi.Domain.ValueObjects.Shared;

namespace BookApi.Domain.Entities;

public class Publisher : AggregateEntityBase<Publisher>
{
    public PublisherName Name { get; private set; }
    public ItemId[] Books { get; } = [];

    public Publisher(
        int id,
        DateTime createdAt, DateTime? updatedAt,
        string createdByUserId, string? updatedByUserId,
        int versionId,
        string name,
        int[] bookIds
    ) : base(id, createdAt, updatedAt, createdByUserId, updatedByUserId, versionId)
    {
        Name = PublisherName.Reconstruct(name);
        Books = [.. bookIds.Select(ItemId.Reconstruct)];
    }

    private Publisher(PublisherCommandDTO command)
    {
        Name = PublisherName.CreateWithValidation(command.Name);
    }

    internal static Publisher CreateNew(
        AdminOnlyPermission permission, IDateTimeProvider dateTimeProvider, PublisherCommandDTO command
    )
        => new Publisher(command).CreateNew(permission, dateTimeProvider);

    internal void Update(
        AdminOnlyPermission permission, IDateTimeProvider dateTimeProvider, PublisherCommandDTO command
    )
    {
        Name = PublisherName.CreateWithValidation(command.Name);
        Update(permission, dateTimeProvider);
    }
}
