using BookApi.Domain.ValueObjects.Shared;

namespace BookApi.Test.Fixtures;

public static class UserFixture
{
    public static readonly Actor Admin = new(ItemId.Reconstruct(1), "Admin", true);
    public static readonly Actor Admin2 = new(ItemId.Reconstruct(2), "Admin 2", true);
    public static readonly Actor User1 = new(ItemId.Reconstruct(3), "User 1", false);
}
