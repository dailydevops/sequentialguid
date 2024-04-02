namespace NetEvolve.SequentialGuid;

using System;
using System.Diagnostics.CodeAnalysis;

/// <summary>
/// Factory class to create sequential <see cref="Guid"/>s.
/// </summary>
public static class SequentialGuidFactory
{
    /// <summary>
    /// Creates a new sequential <see cref="Guid"/> based on <paramref name="sequentialGuid"/>.
    /// </summary>
    /// <param name="sequentialGuid">Optional parameter. Defines the characteristics of the Sequential Guid.</param>
    /// <returns>A sequential <see cref="Guid"/>.</returns>
    public static Guid NewGuid(SequentialGuidType sequentialGuid = SequentialGuidType.AsString) =>
        NewGuid(Random.Shared, DateTimeOffset.UtcNow, sequentialGuid);

    /// <summary>
    /// Creates a new sequential <see cref="Guid"/> based on <paramref name="sequentialGuid"/>.
    /// </summary>
    /// <param name="random"></param>
    /// <param name="utcNow"></param>
    /// <param name="sequentialGuid">Optional parameter. Defines the characteristics of the Sequential Guid.</param>
    /// <returns>A sequential <see cref="Guid"/>.</returns>
    [SuppressMessage("Security", "CA5394:Do not use insecure randomness")]
    internal static Guid NewGuid(
        [NotNull] Random random,
        DateTimeOffset utcNow,
        SequentialGuidType sequentialGuid = SequentialGuidType.AsString
    )
    {
        ArgumentNullException.ThrowIfNull(random);

        var timeStamp = utcNow.Ticks / 10000L;
        var timeStampBytes = BitConverter.GetBytes(timeStamp).AsSpan();

        if (BitConverter.IsLittleEndian)
        {
            timeStampBytes.Reverse();
        }

        Span<byte> guidBytes = stackalloc byte[16];

        if (sequentialGuid == SequentialGuidType.AtEnd)
        {
            timeStampBytes[2..8].CopyTo(guidBytes[10..16]);
            random.NextBytes(guidBytes[..9]);

            return new Guid(guidBytes);
        }

        timeStampBytes[2..8].CopyTo(guidBytes[..6]);

        if (sequentialGuid == SequentialGuidType.AsString && BitConverter.IsLittleEndian)
        {
            guidBytes[..4].Reverse();
            guidBytes[4..6].Reverse();
        }

        random.NextBytes(guidBytes[7..]);

        return new Guid(guidBytes);
    }
}
