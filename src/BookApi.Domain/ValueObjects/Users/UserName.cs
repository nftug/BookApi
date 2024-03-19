using BookApi.Domain.Abstractions.ValueObjects;

namespace BookApi.Domain.ValueObjects.Users;

public record UserName : EntityNameBase<UserName>
{
    protected override string FieldDisplayNameCore => "ユーザー名";
    protected override int LimitLengthCore => 30;
}
