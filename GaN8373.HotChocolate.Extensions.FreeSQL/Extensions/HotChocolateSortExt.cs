using System.Collections.Frozen;
using System.Reflection;
using System.Text;
using FreeSql;
using GaN8373.HotChocolate.Extensions.FreeSQL.Utils;
using HotChocolate.Data.Sorting;
using HotChocolate.Resolvers;

namespace GaN8373.HotChocolate.Extensions.FreeSQL.Extensions;

public static class HotChocolateSortExt
{
    /// <summary>
    /// </summary>
    /// <after>
    ///     <see cref="FillPagingParams{T}" />
    /// </after>
    /// <required>
    ///     <see cref="UseSortingAttribute" />
    /// </required>
    public static void TryFillSortingParams<T>(this IResolverContext context, ISelect<T> select,
        bool isInverseOrder = false)
        where T : class
    {
        var sortingContext = context.GetSortingContext();

        foreach (var dictionary in sortingContext?.ToList() ?? [])
        foreach (var pair in dictionary)
        {
            var fieldName = new StringBuilder();
            FillFieldSort(pair, fieldName, select, isInverseOrder);
        }
    }

    private static void FillFieldSort<T>(KeyValuePair<string, object?> pair, in StringBuilder fieldName,
        in ISelect<T> select, bool isInverseOrder = false, Type? type = null)
        where T : class
    {
        MatchEntityProperties(pair.Key, type ?? typeof(T), out var property);
        if (property == null) return;

        fieldName.Append(property.Name);
        switch (pair.Value)
        {
            case string str:
                select.OrderByPropertyName(fieldName.ToString(), (str == "ASC") ^ isInverseOrder);
                break;
            case IDictionary<string, object?> dict:
            {
                foreach (var keyValuePair in dict)
                    FillFieldSort(keyValuePair, new StringBuilder(fieldName.Append('.').ToString()), select, isInverseOrder, property.PropertyType);
                break;
            }
        }
    }

    private static void MatchEntityProperties(string propertyName, Type type, out PropertyInfo? property)
    {
        var typeCache = CacheUtil.TypePropertyCache;
        typeCache.TryGetValue(type, out var properties);
        if (properties == null)
        {
            properties = type.GetProperties().ToFrozenDictionary(
                p => p.Name,
                p => p,
                StringComparer.OrdinalIgnoreCase);
            typeCache[type] = properties;
        }

        properties.TryGetValue(propertyName, out property);
    }
}