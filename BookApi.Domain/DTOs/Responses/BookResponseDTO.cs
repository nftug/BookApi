namespace BookApi.Domain.DTOs.Responses;

public record BookResponseDTO(
    string ISBN,
    string Title,
    DateTime PublishedAt,
    ItemSummaryResponseDTO[] Authors,
    ItemSummaryResponseDTO Publisher
);
