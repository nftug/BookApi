namespace BookApi.Domain.DTOs.Responses;

public record PublisherResponseDTO
(
    int ID,
    string Name,
    BookSummaryResponseDTO[] Books,
    ItemSummaryResponseDTO[] RelatedAuthors
);
