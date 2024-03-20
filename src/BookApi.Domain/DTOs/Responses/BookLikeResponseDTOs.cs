namespace BookApi.Domain.DTOs.Responses;

public record BookLikeResponseDTO(bool IsLiked);

public record BookLikeListItemResponseDTO(UserSummaryResponseDTO User, DateTime LikedAt);
