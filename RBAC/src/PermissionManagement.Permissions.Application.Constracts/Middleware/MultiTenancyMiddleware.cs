using Microsoft.AspNetCore.Http;
using MokPermissions.Domain.Shared;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MokPermissions.Application.Contracts.Middleware
{
    /// <summary>
    /// 多租户中间件，用于提取当前租户信息
    /// </summary>
    public class MultiTenancyMiddleware
    {
        private readonly RequestDelegate _next;

        public MultiTenancyMiddleware(RequestDelegate next)
        {
            _next = next;
        }

        public async Task InvokeAsync(HttpContext context, ICurrentTenant currentTenant)
        {
            // 从请求中提取租户ID（这里只是示例，实际中可能从域名、请求头、Cookie等提取）
            if (context.Request.Headers.TryGetValue("TenantId", out var tenantIdHeader) &&
                Guid.TryParse(tenantIdHeader, out var tenantId))
            {
                using (currentTenant.Change(tenantId))
                {
                    await _next(context);
                }
            }
            else
            {
                await _next(context);
            }
        }
    }
}
