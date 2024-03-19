using BookApi.Domain.ValueObjects.Shared;

namespace BookApi.Test.Fixtures;

public static class UserFixture
{
    public static readonly ActorForPermission Admin = new(ItemId.Reconstruct(1), "Admin", true);
    public static readonly ActorForPermission Admin2 = new(ItemId.Reconstruct(2), "Admin 2", true);
    public static readonly ActorForPermission User1 = new(ItemId.Reconstruct(3), "User 1", false);
}
