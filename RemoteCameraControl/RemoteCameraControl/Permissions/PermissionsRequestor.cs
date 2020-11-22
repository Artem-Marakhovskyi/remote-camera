using System.Linq;
using System.Threading.Tasks;
using Plugin.Permissions.Abstractions;

namespace RemoteCameraControl.Android.RemoteCameraControl.Permissions
{
    public class PermissionsRequestor : IPermissionsRequestor
    {
        private readonly IPermissions _permissions;

        public PermissionsRequestor(
            IPermissions permissions)
        {
            _permissions = permissions;
        }
        
        public async Task RequestInitiallyRequiredAsync()
        {
            await RequestPermissionAsync(Permission.Camera);
            await RequestPermissionAsync(Permission.MediaLibrary);
        }
        

        private async Task<PermissionStatus> RequestPermissionAsync(Permission permission)
        {
            var status = await _permissions.CheckPermissionStatusAsync(permission);

            if (status == Plugin.Permissions.Abstractions.PermissionStatus.Granted) 
                return status;
            
            return (await _permissions.RequestPermissionsAsync(permission)).First().Value;
        }
    }
}