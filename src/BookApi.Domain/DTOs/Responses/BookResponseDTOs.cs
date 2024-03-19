namespace BookApi.Domain.DTOs.Responses;

public record BookResponseDTO(
    string ISBN,
    string Title,
    DateTime PublishedAt,
    IEnumerable<ItemSummaryResponseDTO> Authors,
    ItemSummaryResponseDTO Publisher,
    int NumberOfLikes,
    bool IsLikedByMe
);

public record BookListItemResponseDTO(
    string ISBN,
    string Title,
    DateTime PublishedAt,
    IEnumerable<ItemSummaryResponseDTO> Authors,
    ItemSummaryResponseDTO Publisher,
    int NumberOfLikes,
    bool IsLikedByMe
);

public record BookSummaryResponseDTO(
    string ISBN,
    string Title,
    DateTime PublishedAt,
    IEnumerable<int> AuthorIds,
    int PublisherId
);

public record BookCreationResponseDTO(string ISBN);
