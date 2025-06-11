using FreeSql;
using FreeSql.Internal.Model;
using HotChocolate.Resolvers;
using HotChocolate.Types;

namespace GaN8373.HotChocolate.Extensions.FreeSQL.Extensions;

public static class HotChocolatePagingExt
{
    /// <summary>
    ///     传统分页, 没有游标支持
    /// </summary>
    /// <warning>需要在WHERE参数添加之后执行</warning>
    /// ///
    /// <after>
    ///     <see cref="TryFillWhereParams{T}" />
    /// </after>
    /// <required>
    ///     <see cref="UsePagingAttribute" />
    /// </required>
    public static BasePagingInfo FillPagingParams<T>(this IResolverContext context, ISelect<T> select, out bool isInverseOrder)
        where T : class
    {
        isInverseOrder = false;

        var take = context.ArgumentOptional<int>("first");
        if (take.IsEmpty)
        {
            take = context.ArgumentOptional<int>("last");
            if (take.IsEmpty)
                take = 50;
            else
                isInverseOrder = true;
        }

        var offset = context.ArgumentOptional<int>("after");
        if (offset.IsEmpty) offset = context.ArgumentOptional<int>("before");

        if (offset.IsEmpty) offset = 0;

        var basePagingInfo = new BasePagingInfo
        {
            PageSize = take.Value,
            PageNumber = offset / take.Value + 1
        };

        select.Page(basePagingInfo);

        return basePagingInfo;
    }
}