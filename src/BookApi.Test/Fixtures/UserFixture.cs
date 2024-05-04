using BookApi.Domain.ValueObjects.Shared;
using BookApi.Domain.ValueObjects.Users;

namespace BookApi.Test.Fixtures;

public static class UserFixture
{
    public static readonly Actor Admin = new(ItemId.Reconstruct(1), UserId.Reconstruct("Admin"), true);
    public static readonly Actor Admin2 = new(ItemId.Reconstruct(2), UserId.Reconstruct("Admin 2"), true);
    public static readonly Actor User1 = new(ItemId.Reconstruct(3), UserId.Reconstruct("User 1"), false);
}
