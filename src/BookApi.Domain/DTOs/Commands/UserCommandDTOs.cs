namespace BookApi.Domain.DTOs.Commands;

public record UserNameCommandDTO(string UserName);

public record UserPasswordCommandDTO(string OldPassword, string NewPassword);
