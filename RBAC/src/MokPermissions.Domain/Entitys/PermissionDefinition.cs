using MokPermissions.Domain.Shared.MultiTenant;

namespace MokPermissions.Domain
{
    /// <summary>
    /// 表示系统中的一个权限
    /// </summary>
    public class PermissionDefinition : IHasTenant
    {

        /// <summary>
        /// 权限的唯一名称
        /// </summary>
        public string Name { get; }

        /// <summary>
        /// 权限的显示名称，用于UI显示
        /// </summary>
        public string DisplayName { get; set; }

        /// <summary>
        /// 权限的详细描述
        /// </summary>
        public string Description { get; set; }

        /// <summary>
        /// 是否授权
        /// </summary>
        public bool IsGranted { get; set; }

        /// <summary>
        /// 是否默认授予此权限
        /// </summary>
        public bool IsGrantedByDefault { get; set; }

        /// <summary>
        /// 权限组，用于对权限进行分组
        /// </summary>
        public string Group { get; set; }

        public int? Level { get => GetLevel(); }

        /// <summary>
        /// 父权限，用于构建权限层次结构
        /// </summary>
        public PermissionDefinition Parent { get; private set; }

        /// <summary>
        /// 子权限列表
        /// </summary>
        public List<PermissionDefinition> Children { get; }
        public Guid? TenantId { get; set; }

        /// <summary>
        /// 创建一个新的权限定义
        /// </summary>
        /// <param name="name">权限名称</param>
        /// <param name="displayName">显示名称</param>
        /// <param name="description">权限描述</param>
        /// <param name="isGrantedByDefault">是否默认授予</param>
        public PermissionDefinition(
            string name,
            string displayName = null,
            string description = null,
            bool isGrantedByDefault = false)
        {
            Name = name ?? throw new ArgumentNullException(nameof(name));
            DisplayName = displayName ?? name;
            Description = description;
            IsGrantedByDefault = isGrantedByDefault;
            Children = new List<PermissionDefinition>();
        }

        /// <summary>
        /// 添加子权限
        /// </summary>
        /// <param name="name">权限名称</param>
        /// <param name="displayName">显示名称</param>
        /// <param name="description">权限描述</param>
        /// <param name="isGrantedByDefault">是否默认授予</param>
        /// <returns>创建的子权限</returns>
        public virtual PermissionDefinition AddChild(
            string name,
            string displayName = null,
            string description = null,
            bool isGrantedByDefault = false)
        {
            var child = new PermissionDefinition(
                name,
                displayName,
                description,
                isGrantedByDefault
            )
            {
                Parent = this
            };

            Children.Add(child);
            return child;
        }

        /// <summary>
        /// 获取权限的完整名称，包括所有父权限
        /// </summary>
        /// <returns>完整的权限名称</returns>
        public string GetFullName()
        {
            if (Parent == null)
            {
                return Name;
            }
            return $"{Parent.GetFullName()}.{Name}";
        }

        /// <summary>
        /// 获取权限的层级
        /// </summary>
        /// <returns></returns>
        public int GetLevel() => Parent == null ? 1 : Parent.GetLevel() + 1;



    }
}
