using FreeSql;
using FreeSql.Internal.Model;
using HotChocolate.Resolvers;

namespace GaN8373.HotChocolate.Extensions.FreeSQL.Extensions;

public static class ResolverContextExt
{
    /// <summary>
    ///     填充GraphQL参数到select
    /// </summary>
    /// <param name="db"></param>
    /// <param name="context"></param>
    /// <typeparam name="T"></typeparam>
    /// <tip>要手动include</tip>
    /// <returns></returns>
    /// <required>
    ///     <see cref="HotChocolate.Types.UsePagingAttribute" />
    ///     <see cref="UseFilteringAttribute" />
    ///     <see cref="UseSortingAttribute" />
    /// </required>
    public static ISelect<T> FillGraphqlParams<T>(this IResolverContext context, IFreeSql db)
        where T : class
    {
        var select = db.Select<T>();

        context.TryFillWhereParams(select);
        context.FillPagingParams(select, out var isInverseOrder);
        context.TryFillSortingParams(select, isInverseOrder);
        return select;
    }

    ///  <summary>
    /// 
    /// </summary>
    /// <inheritdoc cref="FillGraphqlParams{T}(HotChocolate.Resolvers.IResolverContext,IFreeSql)"/>
    public static ISelect<T> FillGraphqlParams<T>(this IResolverContext context, IFreeSql db, out BasePagingInfo paging, bool requiredTotalCount = false)
        where T : class
    {
        var select = db.Select<T>();

        context.TryFillWhereParams(select);
        paging = context.FillPagingParams(select, out var isInverseOrder, requiredTotalCount);
        context.TryFillSortingParams(select, isInverseOrder);
        return select;
    }
}