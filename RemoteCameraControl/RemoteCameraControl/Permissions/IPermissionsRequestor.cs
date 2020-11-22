using System.Threading.Tasks;

namespace RemoteCameraControl.Android.RemoteCameraControl.Permissions
{
    public interface IPermissionsRequestor
    {
        Task RequestInitiallyRequiredAsync();
    }
}