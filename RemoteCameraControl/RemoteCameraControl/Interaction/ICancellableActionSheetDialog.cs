using System.Threading.Tasks;

namespace RemoteCameraControl.RemoteCameraControl.Interaction
{
    public interface ICancellableActionSheetDialog
    {
        Task<string> ShowCancellableActionSheetAsync(string title, string cancel, params string[] buttons);
    }
}