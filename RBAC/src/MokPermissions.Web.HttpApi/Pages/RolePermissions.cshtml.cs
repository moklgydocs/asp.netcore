using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using MokPermissions.Application.Contracts;
using MokPermissions.Domain;
using MokPermissions.Domain.Entitys;
using MokPermissions.Domain.Manager;

namespace MokPermissions.Web.HttpApi.Pages
{
    [Authorize("Permission.RoleManagement.View")]
    public class RolePermissionsModel : PageModel
    {
        private readonly IRolePermissionService _rolePermissionService;
        private readonly PermissionDefinitionManager _permissionDefinitionManager;

        [BindProperty(SupportsGet = true)]
        public Guid RoleId { get; set; }

        [BindProperty(SupportsGet = true)]
        public string RoleName { get; set; }

        public List<PermissionGroupViewModel> Groups { get; set; }

        [BindProperty]
        public List<string> SelectedPermissions { get; set; }

        public RolePermissionsModel(
            IRolePermissionService rolePermissionService,
            PermissionDefinitionManager permissionDefinitionManager)
        {
            _rolePermissionService = rolePermissionService;
            _permissionDefinitionManager = permissionDefinitionManager;
        }

        public async Task<IActionResult> OnGetAsync()
        {
            await LoadPermissionsAsync();
            return Page();
        }

        public async Task<IActionResult> OnPostAsync()
        {
            if (!await CheckUpdatePermissionAsync())
            {
                return Forbid();
            }

            await _rolePermissionService.SetPermissionsAsync(RoleId, SelectedPermissions ?? new List<string>());

            return RedirectToPage("./RolePermissions", new { RoleId, RoleName });
        }

        private async Task<bool> CheckUpdatePermissionAsync()
        {
            var permissionChecker = HttpContext.RequestServices.GetRequiredService<IPermissionChecker>();
            return await permissionChecker.IsGrantedAsync("RoleManagement.Update");
        }

        private async Task LoadPermissionsAsync()
        {
            var groups = _permissionDefinitionManager.GetGroups();
            var grantedPermissions = await _rolePermissionService.GetPermissionsAsync(RoleId);

            SelectedPermissions = grantedPermissions
                .Where(p => p.IsGranted)
                .Select(p => p.Name)
                .ToList();

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
                    AddPermissionToViewModel(
                        permission,
                        grantedPermissions,
                        groupViewModel.Permissions);
                }

                Groups.Add(groupViewModel);
            }
        }

        private void AddPermissionToViewModel(
            PermissionDefinition permission,
            List<PermissionGrant> grantedPermissions,
            List<PermissionViewModel> permissions,
            PermissionViewModel parent = null)
        {
            var isGranted = grantedPermissions
                .Any(p => p.Name == permission.Name && p.IsGranted);

            var permissionViewModel = new PermissionViewModel
            {
                Name = permission.Name,
                DisplayName = permission.DisplayName,
                Description = permission.Description,
                IsGranted = isGranted,
                Parent = parent,
                Children = new List<PermissionViewModel>()
            };

            permissions.Add(permissionViewModel);

            foreach (var child in permission.Children)
            {
                AddPermissionToViewModel(
                    child,
                    grantedPermissions,
                    permissionViewModel.Children,
                    permissionViewModel);
            }
        }
    }
}
