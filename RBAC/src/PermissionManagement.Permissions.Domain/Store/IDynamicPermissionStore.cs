using MokPermissions.Domain.Entitys;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MokPermissions.Domain.Shared
{
    /// <summary>
    /// 动态权限存储接口
    /// </summary>
    public interface IDynamicPermissionStore
    {
        /// <summary>
        /// 获取所有动态权限记录
        /// </summary>
        /// <returns>动态权限记录列表</returns>
        Task<List<DynamicPermissionRecord>> GetPermissionsAsync();

        /// <summary>
        /// 添加或更新动态权限记录
        /// </summary>
        /// <param name="record">动态权限记录</param>
        Task SavePermissionAsync(DynamicPermissionRecord record);

        /// <summary>
        /// 删除动态权限记录
        /// </summary>
        /// <param name="name">权限名称</param>
        Task DeletePermissionAsync(string name);
    }

}
