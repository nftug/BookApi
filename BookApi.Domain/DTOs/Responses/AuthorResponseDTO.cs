namespace BookApi.Domain.DTOs.Responses;

public record AuthorResponseDTO(
    int ID,
    string Name,
    IEnumerable<BookSummaryResponseDTO> Books
)
{
    public IEnumerable<ItemSummaryResponseDTO> RelatedPublishers { get; init; } = null!;
}
