namespace MokPermissions.Domain.Shared
{
    /// <summary>
    /// 权限授权状态，表示权限是被授予还是被禁止
    /// </summary>
    public enum PermissionGrantStatus
    {
        /// <summary>
        /// 权限被授予
        /// </summary>
        Granted = 1,

        /// <summary>
        /// 权限被禁止
        /// </summary>
        Prohibited = 0,

        /// <summary>
        /// 未定义（没有明确授权或禁止）
        /// </summary>
        Undefined = -1
    }
}
