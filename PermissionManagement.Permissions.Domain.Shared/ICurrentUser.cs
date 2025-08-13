using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Text;
using System.Threading.Tasks;

namespace MokPermissions.Application.Contracts
{
    /// <summary>
    /// 当前用户接口，用于获取当前用户信息
    /// </summary>
    public interface ICurrentUser
    {
        /// <summary>
        /// 是否已认证
        /// </summary>
        bool IsAuthenticated { get; }

        /// <summary>
        /// 用户ID
        /// </summary>
        string Id { get; }

        /// <summary>
        /// 用户名
        /// </summary>
        string UserName { get; }

        /// <summary>
        /// 用户角色
        /// </summary>
        string[] Roles { get; }

        /// <summary>
        /// 用户主体
        /// </summary>
        ClaimsPrincipal Principal { get; }

        /// <summary>
        /// 获取用户声明
        /// </summary>
        /// <param name="claimType">声明类型</param>
        /// <returns>声明值</returns>
        string FindClaimValue(string claimType);

        /// <summary>
        /// 获取用户声明
        /// </summary>
        /// <param name="claimType">声明类型</param>
        /// <returns>声明值列表</returns>
        string[] FindClaimValues(string claimType);
    }
}
