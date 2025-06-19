using MediatR;

namespace DDD.Core.Application
{
    /// <summary>
    /// 命令接口 - CQRS模式中的命令
    /// 命令表示对系统状态的修改操作
    /// </summary>
    public interface ICommand : IRequest
    {
    }

    /// <summary>
    /// 带返回值的命令接口
    /// </summary>
    /// <typeparam name="TResult">返回值类型</typeparam>
    public interface ICommand<out TResult> : IRequest<TResult>
    {
    }

    /// <summary>
    /// 命令处理器接口
    /// </summary>
    /// <typeparam name="TCommand">命令类型</typeparam>
    public interface ICommandHandler<in TCommand> : IRequestHandler<TCommand>
        where TCommand : ICommand
    {
    }

    /// <summary>
    /// 带返回值的命令处理器接口
    /// </summary>
    /// <typeparam name="TCommand">命令类型</typeparam>
    /// <typeparam name="TResult">返回值类型</typeparam>
    public interface ICommandHandler<in TCommand, TResult> : IRequestHandler<TCommand, TResult>
        where TCommand : ICommand<TResult>
    {
    }
}