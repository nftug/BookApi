using BookApi.Domain.ValueObjects.Shared;

namespace BookApi.Test.Fixtures;

public static class UserFixture
{
    public static readonly ActorForPermission Admin = new(1, "Admin", true);
}
