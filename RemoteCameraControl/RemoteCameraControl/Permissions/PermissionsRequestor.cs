using System.Linq;
using System.Threading.Tasks;
using Plugin.Permissions;
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
            await RequestPermissionAsync<CameraPermission>();
            await RequestPermissionAsync<MediaLibraryPermission>();
            //await RequestPermissionAsync<LocationPermission>();
        }
        

        private async Task<PermissionStatus> RequestPermissionAsync<T>() where T : BasePermission, new()
        {
            var status = await _permissions.CheckPermissionStatusAsync<T>();

            if (status == Plugin.Permissions.Abstractions.PermissionStatus.Granted) 
                return status;
            
            return await _permissions.RequestPermissionAsync<T>();
        }
    }
}