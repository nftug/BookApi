namespace BookApi.Domain.DTOs.Responses;

public record BookSummaryResponseDTO(
    string ISBN,
    string Title,
    DateTime PublishedAt,
    IEnumerable<int> AuthorIDs,
    int PublisherID
);
