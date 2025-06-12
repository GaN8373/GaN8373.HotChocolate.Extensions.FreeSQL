using System.Linq.Expressions;
using HotChocolate.Execution.Processing;
using HotChocolate.Resolvers;

namespace GaN8373.HotChocolate.Extensions.FreeSQL.Extensions;

public static class HotChocolateProjectionSelectorExt
{
    /// <summary>
    /// 
    /// </summary>
    /// <inheritdoc cref="TryExtractProjectionSelector{T}"/>
    /// <returns>如果gql没有选择任何字段，并且 customAssignment为空，则会选择所有字段</returns>
    public static Expression<Func<T, T>> ExtractProjectionSelector<T>(this IResolverContext context, Expression<Func<T, T>>? customAssignment = null)
        where T : class, new()
    {
        return TryExtractProjectionSelector(context, customAssignment) ?? (t => new T());
    }

    /// <summary>
    /// 
    /// </summary>
    /// <param name="context"></param>
    /// <param name="customAssignment">要 t => new T{ XXX = t.XXX }, 将成员绑定合gql选择的字段绑定, 查询时强制包含这些字段, 尽管这些字段在返回给前端后仍然会被裁剪, 但是后端进行处理时可以确定这些字段是非空的</param>
    /// <typeparam name="T"></typeparam>
    /// <returns></returns>
    public static Expression<Func<T, T>>? TryExtractProjectionSelector<T>(this IResolverContext context, Expression<Func<T, T>>? customAssignment = null)
        where T : class
    {
        var contextSelection = context.Selection;
        Expression<Func<T, T>>? asSelector = contextSelection.AsSelector<T>();
        var newBindings = new List<MemberBinding>();

        var memberInitExpression = asSelector?.Body as MemberInitExpression;
        if (memberInitExpression != null)
        {
            newBindings.AddRange(ExtractMemberBindings(memberInitExpression));
        }

        var customAssignmentBody = customAssignment?.Body as MemberInitExpression;
        if (customAssignmentBody != null)
        {
            newBindings.AddRange(ExtractMemberBindings(customAssignmentBody));
        }

        if (newBindings.Count == 0)
        {
            return null;
        }

        // 使用原始的 NewExpression 和新的 Bindings 创建一个新的 MemberInitExpression
        var newMemberInitExpression = Expression.MemberInit(memberInitExpression?.NewExpression ?? customAssignmentBody?.NewExpression!, newBindings);

        var newSelector = Expression.Lambda<Func<T, T>>(newMemberInitExpression, asSelector?.Parameters ?? customAssignment?.Parameters);

        return newSelector;
    }


    public static IEnumerable<MemberBinding> ExtractMemberBindings(MemberInitExpression memberInitExpression)
    {
        var newBindings = new List<MemberBinding>();

        foreach (var binding in memberInitExpression.Bindings)
            switch (binding)
            {
                // 如果是 ConditionalExpression，则取其 IfFalse (else) 部分
                case MemberAssignment { Expression: ConditionalExpression conditionalExpression } memberAssignment:
                    newBindings.Add(Expression.Bind(memberAssignment.Member, conditionalExpression.IfFalse));
                    break;
                // 否则保持不变
                case MemberAssignment:
                    newBindings.Add(binding);
                    break;
                default:
                    // 其他类型的绑定，例如 MemberListBinding 或 MemberMemberBinding，这里简单保持不变
                    newBindings.Add(binding);
                    break;
            }

        return newBindings;
    }
}