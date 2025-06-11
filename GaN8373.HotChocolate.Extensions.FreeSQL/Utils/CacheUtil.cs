using System.Collections.Concurrent;
using System.Collections.Frozen;
using System.Reflection;

namespace GaN8373.HotChocolate.Extensions.FreeSQL.Utils;

public static class CacheUtil
{
    public static ConcurrentDictionary<Type, FrozenDictionary<string, PropertyInfo>> TypePropertyCache { get; } = new();
}