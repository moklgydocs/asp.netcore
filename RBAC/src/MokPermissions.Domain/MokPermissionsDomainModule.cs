using Mok.Modularity;
using MokPermissions.Domain.Shared;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MokPermissions.Domain
{
    [DependsOn(typeof(MokPermissionsDomainSharedModule))]
    public class MokPermissionsDomainModule : MokModule
    {
    }
}
