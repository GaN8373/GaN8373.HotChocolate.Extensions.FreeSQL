# Quick Start

```csharp
    /// <summary>
    ///     查询 系统用户
    /// </summary>
    [UsePaging]
    [UseProjection]
    [UseFiltering]
    [UseSorting]
    public static Connection<SysUser> GetSysUser(IResolverContext context,
        [Service] IFreeSql db)
    {
        var select = context.FillGraphqlParams<SysUser>(db, out var paging);
        var projectionSelector = context.TryExtractProjectionSelector<SysUser>(t => new SysUser
        {
            UserId = t.UserId
        });

        var list = projectionSelector == null ? [] : select.ToList(projectionSelector);

        return PagingUtil.CreateConnection(list, paging, t => t.UserId.ToString());
    }
```
