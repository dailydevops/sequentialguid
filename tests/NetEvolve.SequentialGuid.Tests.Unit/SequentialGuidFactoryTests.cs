namespace NetEvolve.SequentialGuid.Tests.Unit;

using System;
using System.Diagnostics.CodeAnalysis;
using System.Linq;
using NetEvolve.Extensions.XUnit;
using Xunit;

[UnitTest]
[ExcludeFromCodeCoverage]
public class SequentialGuidFactoryTests
{
    [Fact]
    public void NewGuid_RandomNull_ArgumentNullException() =>
        _ = Assert.Throws<ArgumentNullException>(
            "random",
            () => _ = SequentialGuidFactory.NewGuid(null!, DateTimeOffset.UtcNow)
        );

    [Theory]
    [InlineData(SequentialGuidType.AsString)]
    [InlineData(SequentialGuidType.AsBinary)]
    [InlineData(SequentialGuidType.AtEnd)]
    public void NewGuid_NotGuidEmpty_Expected(SequentialGuidType sequentialGuidType)
    {
        var result = SequentialGuidFactory.NewGuid(sequentialGuidType);

        Assert.NotEqual(Guid.Empty, result);
    }

    [Theory]
    [MemberData(nameof(GetData))]
    public void NewGuid_Theory_Expected(
        [NotNull] Guid[] expected,
        Random random,
        DateTimeOffset utcNow,
        SequentialGuidType sequentialGuidType
    )
    {
        var numberOfResults = expected.Length;
        var result = Enumerable
            .Range(0, numberOfResults)
            .Select(_ => SequentialGuidFactory.NewGuid(random, utcNow, sequentialGuidType))
            .ToArray();

        Assert.Equal(expected, result);
    }

    public static TheoryData<Guid[], Random, DateTimeOffset, SequentialGuidType> GetData()
    {
        var random = new Random(2511);
        var utcNow = new DateTimeOffset(1970, 1, 1, 0, 0, 1, 0, TimeSpan.Zero);

        var data = new TheoryData<Guid[], Random, DateTimeOffset, SequentialGuidType>
        {
            {

                [
                    Guid.Parse("3883122c-dbe8-ff00-9a12-e457b67ae9ad"),
                    Guid.Parse("3883122c-dbe8-1d00-560d-b020d2ace641"),
                ],
                random,
                utcNow,
                SequentialGuidType.AsString
            },
            {

                [
                    Guid.Parse("2c128338-e8db-5c00-5e69-9ee63acf9893"),
                    Guid.Parse("2c128338-e8db-1b00-3d42-e77f8a7d7ee0"),
                    Guid.Parse("2c128338-e8db-e200-e498-cf73742fc85a"),
                ],
                random,
                utcNow,
                SequentialGuidType.AsBinary
            },
            {

                [
                    Guid.Parse("5ff55952-18c9-a3d5-c100-3883122cdbe8"),
                    Guid.Parse("d8b460e9-bf14-5f23-ac00-3883122cdbe8"),
                ],
                random,
                utcNow,
                SequentialGuidType.AtEnd
            }
        };

        return data;
    }
}
