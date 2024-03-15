namespace BookApi.Domain.DTOs.Responses;

public record PublisherResponseDTO(
    int ID,
    string Name,
    IEnumerable<BookSummaryResponseDTO> Books,
    IEnumerable<ItemSummaryResponseDTO> RelatedAuthors
);
