namespace BookApi.Domain.DTOs.Responses;

public record AuthorResponseDTO
(
    int ID,
    string Name,
    BookSummaryResponseDTO[] Books,
    ItemSummaryResponseDTO[] RelatedPublishers
);
