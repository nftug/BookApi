namespace BookApi.Domain.DTOs.Commands;

public record BookLikeEditCommandDTO(string UserId, bool IsLiked);
