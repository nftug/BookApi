namespace BookApi.Domain.DTOs.Responses;

public record PublisherResponseDTO(
    int Id,
    string Name,
    IEnumerable<BookSummaryResponseDTO> Books
)
{
    public IEnumerable<ItemSummaryResponseDTO> RelatedAuthors { get; init; } = null!;
}
