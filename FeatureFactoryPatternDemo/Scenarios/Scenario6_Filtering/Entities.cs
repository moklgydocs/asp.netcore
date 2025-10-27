// 场景6：数据过滤实体类
// 定义用于演示多租户数据隔离的实体类

using System;
using System.ComponentModel.DataAnnotations;

namespace FeatureFactoryPatternDemo.Scenarios.Scenario6_Filtering
{
    #region 1. 基础实体类

    /// <summary>
    /// 基础实体类
    /// 包含所有实体共有的属性
    /// </summary>
    public abstract class BaseEntity
    {
        /// <summary>
        /// 主键ID
        /// </summary>
        [Key]
        public int Id { get; set; }

        /// <summary>
        /// 创建时间
        /// </summary>
        public DateTime CreateTime { get; set; } = DateTime.Now;

        /// <summary>
        /// 更新时间
        /// </summary>
        public DateTime UpdateTime { get; set; } = DateTime.Now;

        /// <summary>
        /// 是否已删除（软删除标记）
        /// </summary>
        public bool IsDeleted { get; set; } = false;
    }

    #endregion

    #region 2. 用户实体

    /// <summary>
    /// 用户实体
    /// 使用多租户过滤器和软删除过滤器
    /// </summary>
    [Filter(FilterType.MultiTenant, TenantIdProperty = "TenantId")]
    [Filter(FilterType.SoftDelete, IsDeletedProperty = "IsDeleted")]
    public class User : BaseEntity
    {
        /// <summary>
        /// 用户名
        /// </summary>
        [Required]
        [StringLength(50)]
        public string Username { get; set; }

        /// <summary>
        /// 邮箱
        /// </summary>
        [Required]
        [EmailAddress]
        [StringLength(100)]
        public string Email { get; set; }

        /// <summary>
        /// 租户ID
        /// </summary>
        [Required]
        [StringLength(50)]
        public string TenantId { get; set; }

        /// <summary>
        /// 角色
        /// </summary>
        [StringLength(20)]
        public string Role { get; set; }

        /// <summary>
        /// 最后登录时间
        /// </summary>
        public DateTime? LastLoginTime { get; set; }

        public override string ToString()
        {
            return $"User(Id={Id}, Username={Username}, TenantId={TenantId}, Role={Role})";
        }
    }

    #endregion

    #region 3. 产品实体

    /// <summary>
    /// 产品实体
    /// 使用多租户过滤器
    /// </summary>
    [Filter(FilterType.MultiTenant, TenantIdProperty = "TenantId")]
    public class Product : BaseEntity
    {
        /// <summary>
        /// 产品名称
        /// </summary>
        [Required]
        [StringLength(100)]
        public string Name { get; set; }

        /// <summary>
        /// 产品描述
        /// </summary>
        [StringLength(500)]
        public string Description { get; set; }

        /// <summary>
        /// 价格
        /// </summary>
        public decimal Price { get; set; }

        /// <summary>
        /// 库存数量
        /// </summary>
        public int StockQuantity { get; set; }

        /// <summary>
        /// 租户ID
        /// </summary>
        [Required]
        [StringLength(50)]
        public string TenantId { get; set; }

        /// <summary>
        /// 分类ID
        /// </summary>
        public int CategoryId { get; set; }

        public override string ToString()
        {
            return $"Product(Id={Id}, Name={Name}, Price={Price:C}, Stock={StockQuantity}, TenantId={TenantId})";
        }
    }

    #endregion

    #region 4. 订单实体

    /// <summary>
    /// 订单实体
    /// 使用多租户过滤器、软删除过滤器和时间范围过滤器
    /// </summary>
    [Filter(FilterType.MultiTenant, TenantIdProperty = "TenantId")]
    [Filter(FilterType.SoftDelete, IsDeletedProperty = "IsDeleted")]
    [Filter(FilterType.TimeRange, TimeProperty = "CreateTime", StartTime = "2024-01-01", EndTime = "2024-12-31")]
    public class Order : BaseEntity
    {
        /// <summary>
        /// 订单编号
        /// </summary>
        [Required]
        [StringLength(50)]
        public string OrderNumber { get; set; }

        /// <summary>
        /// 用户ID
        /// </summary>
        public int UserId { get; set; }

        /// <summary>
        /// 总金额
        /// </summary>
        public decimal TotalAmount { get; set; }

        /// <summary>
        /// 订单状态
        /// </summary>
        [StringLength(20)]
        public string Status { get; set; }

        /// <summary>
        /// 租户ID
        /// </summary>
        [Required]
        [StringLength(50)]
        public string TenantId { get; set; }

        /// <summary>
        /// 完成时间
        /// </summary>
        public DateTime? CompletedTime { get; set; }

        public override string ToString()
        {
            return $"Order(Id={Id}, OrderNumber={OrderNumber}, Total={TotalAmount:C}, Status={Status}, TenantId={TenantId})";
        }
    }

    #endregion

    #region 5. 权限实体

    /// <summary>
    /// 权限实体
    /// 使用权限过滤器
    /// </summary>
    [Filter(FilterType.Permission, UserId = "admin", PermissionChecker = "AdminPermissionChecker")]
    public class Permission : BaseEntity
    {
        /// <summary>
        /// 权限名称
        /// </summary>
        [Required]
        [StringLength(50)]
        public string Name { get; set; }

        /// <summary>
        /// 权限代码
        /// </summary>
        [Required]
        [StringLength(20)]
        public string Code { get; set; }

        /// <summary>
        /// 描述
        /// </summary>
        [StringLength(200)]
        public string Description { get; set; }

        /// <summary>
        /// 父权限ID
        /// </summary>
        public int? ParentId { get; set; }

        public override string ToString()
        {
            return $"Permission(Id={Id}, Name={Name}, Code={Code}, ParentId={ParentId})";
        }
    }

    #endregion

    #region 6. 自定义业务实体

    /// <summary>
    /// 自定义业务实体
    /// 演示自定义过滤器的使用
    /// </summary>
    public class CustomBusinessEntity : BaseEntity
    {
        /// <summary>
        /// 业务名称
        /// </summary>
        [Required]
        [StringLength(100)]
        public string BusinessName { get; set; }

        /// <summary>
        /// 业务类型
        /// </summary>
        [StringLength(50)]
        public string BusinessType { get; set; }

        /// <summary>
        /// 业务状态
        /// </summary>
        [StringLength(20)]
        public string Status { get; set; }

        /// <summary>
        /// 优先级
        /// </summary>
        public int Priority { get; set; }

        /// <summary>
        /// 关联用户ID
        /// </summary>
        public int UserId { get; set; }

        public override string ToString()
        {
            return $"CustomBusinessEntity(Id={Id}, BusinessName={BusinessName}, Type={BusinessType}, Status={Status}, Priority={Priority})";
        }
    }

    #endregion
}
