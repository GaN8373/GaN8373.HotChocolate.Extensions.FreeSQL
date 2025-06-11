using FreeSql.Internal.Model;
using GaN8373.HotChocolate.Extensions.FreeSQL.Extensions;
using HotChocolate.Types.Pagination;

namespace GaN8373.HotChocolate.Extensions.FreeSQL.Utils;

public class PagingUtil
{
    public static Connection<T> CreateConnection<T>(IEnumerable<T> queryable, BasePagingInfo paging, Func<T, string> createCursor)
    {
        var readOnlyCollection = queryable.Select(x => new Edge<T>(x, createCursor(x))).ToList().AsReadOnly();


        return new Connection<T>(readOnlyCollection, new ConnectionPageInfo(paging.HasNextPage(), paging.HasPreviousPage(), null, null), (int)paging.Count);
    }

    // public static PageConnection<T> CreatePageConnection<T>(IEnumerable<T> queryable, BasePagingInfo paging, Func<T, string> createCursor, int maxRelativeCursorCount = 5)
    // {
    //     var page = new Page<T>([..queryable], paging.HasNextPage(), paging.HasPreviousPage(), createCursor, (int?)paging.Count);
    //
    //     return new PageConnection<T>(page, maxRelativeCursorCount);
    // }
}