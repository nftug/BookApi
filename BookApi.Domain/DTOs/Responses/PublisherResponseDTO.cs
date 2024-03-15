namespace BookApi.Domain.DTOs.Responses;

public record PublisherResponseDTO(
    int ID,
    string Name,
    IEnumerable<BookSummaryResponseDTO> Books
)
{
    public IEnumerable<ItemSummaryResponseDTO> RelatedAuthors { get; init; } = null!;
}
