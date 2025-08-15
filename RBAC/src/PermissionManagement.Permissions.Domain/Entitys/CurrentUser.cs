using Microsoft.AspNetCore.Http;
using MokPermissions.Domain.Shared;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Text;
using System.Threading.Tasks;

namespace MokPermissions.Domain.Entitys
{
    /// <summary>
    /// 当前用户实现
    /// </summary>
    public class CurrentUser : ICurrentUser
    {
        private readonly IHttpContextAccessor _httpContextAccessor;
        public bool IsAuthenticated => Principal?.Identity?.IsAuthenticated ?? false;

        public string Id => FindClaimValue(ClaimTypes.NameIdentifier);

        public string UserName => FindClaimValue(ClaimTypes.Name);

        public string[] Roles => FindClaimValues(ClaimTypes.Role);

        public ClaimsPrincipal Principal => _httpContextAccessor.HttpContext?.User;

        public CurrentUser(IHttpContextAccessor httpContextAccessor)
        {
            _httpContextAccessor = httpContextAccessor;
        }

        public string FindClaimValue(string claimType)
        {
            return Principal?.FindFirst(claimType)?.Value ?? string.Empty;
        }

        public string[] FindClaimValues(string claimType)
        {
            return Principal?.FindAll(claimType).Select(c => c.Value)
                .ToArray() ?? Array.Empty<string>();
        }
    }
}
