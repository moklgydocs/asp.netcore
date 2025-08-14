using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using MokPermissions.Domain;

namespace MokPermissions.Web.HttpApi.Pages
{
    [Authorize("Permission.PermissionManagement")]
    public class PermissionsModel : PageModel
    {
        private readonly IPermissionManager _permissionManager;
        private readonly PermissionDefinitionManager _permissionDefinitionManager;

        [BindProperty(SupportsGet = true)]
        public string ProviderName { get; set; }

        [BindProperty(SupportsGet = true)]
        public string ProviderKey { get; set; }

        public List<PermissionGroupViewModel> Groups { get; set; }

        public PermissionsModel(
            IPermissionManager permissionManager,
            PermissionDefinitionManager permissionDefinitionManager)
        {
            _permissionManager = permissionManager;
            _permissionDefinitionManager = permissionDefinitionManager;
        }

        public async Task<IActionResult> OnGetAsync()
        {
            if (string.IsNullOrEmpty(ProviderName) || string.IsNullOrEmpty(ProviderKey))
            {
                return BadRequest("Provider name and key must be specified.");
            }

            await LoadPermissionsAsync();
            return Page();
        }

        public async Task<IActionResult> OnPostAsync(string permissionName, bool isGranted)
        {
            if (string.IsNullOrEmpty(ProviderName) || string.IsNullOrEmpty(ProviderKey))
            {
                return BadRequest("Provider name and key must be specified.");
            }

            if (isGranted)
            {
                await _permissionManager.GrantAsync(permissionName, ProviderName, ProviderKey);
            }
            else
            {
                await _permissionManager.ProhibitAsync(permissionName, ProviderName, ProviderKey);
            }

            await LoadPermissionsAsync();
            return Page();
        }

        private async Task LoadPermissionsAsync()
        {
            var groups = _permissionDefinitionManager.GetGroups();
            var grantedPermissions = await _permissionManager.GetAllAsync(ProviderName, ProviderKey);

            Groups = new List<PermissionGroupViewModel>();

            foreach (var group in groups)
            {
                var groupViewModel = new PermissionGroupViewModel
                {
                    Name = group.Name,
                    DisplayName = group.DisplayName,
                    Permissions = new List<PermissionViewModel>()
                };

                foreach (var permission in group.Permissions)
                {
                    await AddPermissionToViewModelRecursively(
                        permission,
                        grantedPermissions,
                        groupViewModel.Permissions);
                }

                Groups.Add(groupViewModel);
            }
        }

        private async Task AddPermissionToViewModelRecursively(
            PermissionDefinition permission,
            List<PermissionGrant> grantedPermissions,
            List<PermissionViewModel> permissions,
            PermissionViewModel parent = null)
        {
            var isGranted = false;
            var isProhibited = false;

            var grantedPermission = grantedPermissions
                .FirstOrDefault(p => p.Name == permission.Name);

            if (grantedPermission != null)
            {
                isGranted = grantedPermission.IsGranted;
                isProhibited = !grantedPermission.IsGranted;
            }

            var permissionViewModel = new PermissionViewModel
            {
                Name = permission.Name,
                DisplayName = permission.DisplayName,
                Description = permission.Description,
                IsGranted = isGranted,
                IsProhibited = isProhibited,
                Parent = parent,
                Children = new List<PermissionViewModel>()
            };

            permissions.Add(permissionViewModel);

            foreach (var child in permission.Children)
            {
                await AddPermissionToViewModelRecursively(
                    child,
                    grantedPermissions,
                    permissionViewModel.Children,
                    permissionViewModel);
            }
        }
    }

    public class PermissionGroupViewModel
    {
        public string Name { get; set; }
        public string DisplayName { get; set; }
        public List<PermissionViewModel> Permissions { get; set; }
    }

    public class PermissionViewModel
    {
        public string Name { get; set; }
        public string DisplayName { get; set; }
        public string Description { get; set; }
        public bool IsGranted { get; set; }
        public bool IsProhibited { get; set; }
        public PermissionViewModel Parent { get; set; }
        public List<PermissionViewModel> Children { get; set; }
    }
}
