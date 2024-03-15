namespace BookApi.Domain.DTOs.Responses;

public record AuthorResponseDTO(
    int ID,
    string Name,
    IEnumerable<BookSummaryResponseDTO> Books,
    IEnumerable<ItemSummaryResponseDTO> RelatedPublishers
);
