namespace BookApi.Domain.DTOs.Responses;

public record BookResponseDTO(
    string ISBN,
    string Title,
    DateTime PublishedAt,
    IEnumerable<ItemSummaryResponseDTO> Authors,
    ItemSummaryResponseDTO Publisher
);
