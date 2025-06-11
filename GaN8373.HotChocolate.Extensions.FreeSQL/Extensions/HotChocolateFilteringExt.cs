using FreeSql;
using HotChocolate.Data.Filters;
using HotChocolate.Resolvers;

namespace GaN8373.HotChocolate.Extensions.FreeSQL.Extensions;

public static class HotChocolateFilteringExt
{
    /// <summary>
    ///     填充GraphQL参数到select
    /// </summary>
    /// <tip>要手动include</tip>
    /// <returns></returns>
    /// <required>
    ///     <see cref="UseFilteringAttribute" />
    /// </required>
    // ReSharper disable once InconsistentNaming
    public static void TryFillWhereParams<T>(this IResolverContext context, ISelect<T> select)
        where T : class
    {
        var filterContext = context.GetFilterContext();

        var asPredicate = filterContext?.AsPredicate<T>();
        if (asPredicate != null)
            select.Where(asPredicate);
    }
}