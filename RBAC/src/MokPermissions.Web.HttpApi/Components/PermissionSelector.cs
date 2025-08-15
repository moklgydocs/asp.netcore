using Microsoft.AspNetCore.Components;
using MokPermissions.Domain;
using MokPermissions.Domain.Manager;
using MokPermissions.Domain.Shared;
using MokPermissions.Web.HttpApi.Pages;

namespace MokPermissions.Web.HttpApi.Components
{
    public partial class PermissionSelector
    {
        [Inject]
        private PermissionDefinitionManager PermissionDefinitionManager { get; set; }

        [Inject]
        private IPermissionChecker PermissionChecker { get; set; }

        [Parameter]
        public string SelectedPermission { get; set; }

        [Parameter]
        public EventCallback<string> SelectedPermissionChanged { get; set; }

        private List<PermissionGroupViewModel> Groups { get; set; }

        protected override async Task OnInitializedAsync()
        {
            await LoadPermissionsAsync();
            await base.OnInitializedAsync();
        }

        private async Task LoadPermissionsAsync()
        {
            var groups = PermissionDefinitionManager.GetGroups();
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
                        groupViewModel.Permissions);
                }

                Groups.Add(groupViewModel);
            }
        }

        private async Task AddPermissionToViewModelRecursively(
            PermissionDefinition permission,
            List<PermissionViewModel> permissions,
            PermissionViewModel parent = null)
        {
            var isGranted = await PermissionChecker.IsGrantedAsync(permission.Name);

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
                await AddPermissionToViewModelRecursively(
                    child,
                    permissionViewModel.Children,
                    permissionViewModel);
            }
        }

        private async Task OnPermissionSelected(string permissionName)
        {
            SelectedPermission = permissionName;
            await SelectedPermissionChanged.InvokeAsync(permissionName);
        }
    }
}
