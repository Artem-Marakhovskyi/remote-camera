using System;
namespace RemoteCameraControl.Permissions
{
    public enum PermissionStatus
    {
        /// <summary>
        /// Denied by user
        /// </summary>
        Denied,

        /// <summary>
        /// Disabled on device
        /// </summary>
        Disabled,

        /// <summary>
        /// Granted by user
        /// </summary>
        Granted,

        /// <summary>
        /// Restricted (only iOS)
        /// </summary>
        Restricted,

        Unknown
    }
}
