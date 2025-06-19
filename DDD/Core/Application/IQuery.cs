using MediatR;

namespace DDD.Core.Application
{
    /// <summary>
    /// 查询接口 - CQRS模式中的查询
    /// 查询不修改系统状态，只返回数据
    /// </summary>
    /// <typeparam name="TResult">查询结果类型</typeparam>
    public interface IQuery<out TResult> : IRequest<TResult>
    {
    }

    /// <summary>
    /// 查询处理器接口
    /// </summary>
    /// <typeparam name="TQuery">查询类型</typeparam>
    /// <typeparam name="TResult">查询结果类型</typeparam>
    public interface IQueryHandler<in TQuery, TResult> : IRequestHandler<TQuery, TResult>
        where TQuery : IQuery<TResult>
    {
    }
}