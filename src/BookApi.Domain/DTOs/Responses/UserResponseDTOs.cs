namespace BookApi.Domain.DTOs.Responses;

public record UserResponseDTO(
    string UserId,
    string UserName,
    DateTime RegisteredAt,
    int NumberOfBookLikes
);

public record UserSummaryResponseDTO(string UserId, string UserName);
