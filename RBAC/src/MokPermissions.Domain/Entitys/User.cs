using MokPermissions.Domain.Shared.MultiTenant;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MokPermissions.Domain.Entitys
{
    /// <summary>
    /// 用户
    /// </summary>
    public class User: IHasTenant
    {
        /// <summary>
        /// 用户ID
        /// </summary>
        public Guid Id { get; set; }

        /// <summary>
        /// 用户名
        /// </summary>
        public string UserName { get; set; }

        /// <summary>
        /// 邮箱
        /// </summary>
        public string Email { get; set; }

        /// <summary>
        /// 姓名
        /// </summary>
        public string Name { get; set; }

        /// <summary>
        /// 租户ID
        /// </summary>
        public Guid? TenantId { get; set; }

        public User()
        {
            Id = Guid.NewGuid();
        }

        public User(
            Guid id,
            string userName,
            string email,
            string name = null,
            Guid? tenantId = null)
        {
            Id = id;
            UserName = userName;
            Email = email;
            Name = name;
            TenantId = tenantId;
        }
    }

}
