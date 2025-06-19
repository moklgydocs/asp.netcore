namespace DDDCore.Services
{
    /// <summary>
    /// 领域服务接口
    /// 领域服务用于封装不自然属于任何实体或值对象的领域逻辑
    /// 通常处理多个聚合根之间的业务规则
    /// </summary>
    public interface IDomainService
    {
        // 标记接口，表明实现类是领域服务
        // 具体领域服务会实现此接口并添加特定业务方法
    }
}