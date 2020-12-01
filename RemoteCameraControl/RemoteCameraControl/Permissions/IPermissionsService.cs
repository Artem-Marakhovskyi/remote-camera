using System.Threading.Tasks;

namespace RemoteCameraControl.Permissions
{
    public interface IPermissionService
    {
        Task<PermissionStatus> RequestPermissionAsync(FeaturePermission permission);

        Task<PermissionStatus> CheckPermissionStatusAsync(FeaturePermission permission);
    }
}
