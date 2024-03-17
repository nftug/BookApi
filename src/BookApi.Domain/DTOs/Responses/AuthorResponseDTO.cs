namespace BookApi.Domain.DTOs.Responses;

public record AuthorResponseDTO(
    int Id,
    string Name,
    IEnumerable<BookSummaryResponseDTO> Books
)
{
    public IEnumerable<ItemSummaryResponseDTO> RelatedPublishers { get; init; } = null!;
}
