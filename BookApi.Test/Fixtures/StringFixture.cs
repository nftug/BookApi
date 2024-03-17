namespace BookApi.Test.Fixtures;

public static class StringFixture
{
    public static string GetAsciiDummyString(int length)
    {
        var random = new Random();
        return
            new(Enumerable.Range(0, length)
                .Select(_ => (char)('A' + random.Next(26)))
                .ToArray());
    }
}
