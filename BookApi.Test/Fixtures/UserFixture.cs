using BookApi.Domain.ValueObjects.Shared;

namespace BookApi.Test.Fixtures;

public static class UserFixture
{
    public static readonly ActorForPermission Admin = new(1, "Admin", true);
    public static readonly ActorForPermission User1 = new(2, "User 1", false);
}
