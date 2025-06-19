namespace DDD.Core.Domain
{
    /// <summary>
    /// 业务规则接口 - 用于封装业务逻辑和不变性约束
    /// </summary>
    public interface IBusinessRule
    {
        /// <summary>
        /// 规则描述消息
        /// </summary>
        string Message { get; }

        /// <summary>
        /// 检查规则是否满足
        /// </summary>
        /// <returns>true表示满足规则，false表示违反规则</returns>
        bool IsSatisfied();
    }

    /// <summary>
    /// 业务规则验证异常
    /// </summary>
    public class BusinessRuleValidationException : Exception
    {
        public BusinessRuleValidationException(string message) : base(message)
        {
        }

        public BusinessRuleValidationException(string message, Exception innerException) 
            : base(message, innerException)
        {
        }
    }
}