namespace BookApi.Domain.DTOs.Commands;

public record BookCommandDTO(
    string ISBN,
    string Title,
    DateTime PublishedAt,
    int[] AuthorIds,
    int PublisherId
);
