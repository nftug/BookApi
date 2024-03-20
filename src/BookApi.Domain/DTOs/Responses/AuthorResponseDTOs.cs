namespace BookApi.Domain.DTOs.Responses;

public record AuthorResponseDTO(
    int AuthorId,
    string Name,
    IEnumerable<BookSummaryResponseDTO> Books
)
{
    public IEnumerable<PublisherSummaryResponseDTO> RelatedPublishers { get; init; } = null!;
}

public record AuthorSummaryResponseDTO(int AuthorId, string Name);
