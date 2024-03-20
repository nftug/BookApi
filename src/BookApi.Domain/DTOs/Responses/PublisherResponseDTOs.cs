namespace BookApi.Domain.DTOs.Responses;

public record PublisherResponseDTO(
    int Id,
    string Name,
    IEnumerable<BookSummaryResponseDTO> Books
)
{
    public IEnumerable<AuthorSummaryResponseDTO> RelatedAuthors { get; init; } = null!;
}

public record PublisherSummaryResponseDTO(int PublisherId, string Name);