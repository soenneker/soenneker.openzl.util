using System;
using System.Collections.Generic;
namespace Soenneker.OpenZl.Util.Options;
/// <summary>Native codec-specific parameters. IDs and binary encodings follow the pinned upstream codec headers.</summary>
public sealed class OpenZlLocalParameters
{
    /// <summary>Integer parameters keyed by their native parameter ID.</summary>
    public IReadOnlyDictionary<int, int> Integers { get; init; } = new Dictionary<int, int>();
    /// <summary>Copied binary parameters. These must be flat data without embedded pointers. Native code copies them during registration.</summary>
    public IReadOnlyDictionary<int, ReadOnlyMemory<byte>> Data { get; init; } = new Dictionary<int, ReadOnlyMemory<byte>>();
}
