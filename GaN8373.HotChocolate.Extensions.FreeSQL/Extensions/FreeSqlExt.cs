using FreeSql.Internal.Model;

namespace GaN8373.HotChocolate.Extensions.FreeSQL.Extensions;

public static class FreeSqlExt
{
    public static bool HasNextPage(this BasePagingInfo paging)
    {
        if (paging.PageSize <= 0 || paging.Count <= 0) return false;

        var totalPages = (paging.Count + paging.PageSize - 1) / paging.PageSize;

        return paging.PageNumber < totalPages;
    }

    public static bool HasPreviousPage(this BasePagingInfo paging)
    {
        if (paging.PageSize <= 0 || paging.Count <= 0) return false;
        return paging.PageNumber > 1;
    }
}