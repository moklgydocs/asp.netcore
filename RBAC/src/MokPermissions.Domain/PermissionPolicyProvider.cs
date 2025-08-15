using Microsoft.AspNetCore.Authorization;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Options;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MokPermissions.Domain
{
    /// <summary>
    /// 权限策略提供程序
    /// </summary>
    public class PermissionPolicyProvider : DefaultAuthorizationPolicyProvider
    {
        private readonly IServiceScopeFactory _serviceScopeFactory;

        public PermissionPolicyProvider(
            IOptions<AuthorizationOptions> options,
            IServiceScopeFactory serviceScopeFactory)
            : base(options)
        {
            _serviceScopeFactory = serviceScopeFactory;
        }

        public override async Task<AuthorizationPolicy> GetPolicyAsync(string policyName)
        {
            // 首先检查是否是默认策略
            var policy = await base.GetPolicyAsync(policyName);
            if (policy != null)
            {
                return policy;
            }

            // 如果是Permission策略，则创建一个包含权限要求的策略
            if (policyName.StartsWith("Permission"))
            {
                var permissionName = policyName.Substring("Permission".Length + 1);

                var policyBuilder = new AuthorizationPolicyBuilder();
                policyBuilder.RequireAuthenticatedUser();
                policyBuilder.AddRequirements(new PermissionRequirement(permissionName));

                return policyBuilder.Build();
            }

            return null;
        }
    }
}
