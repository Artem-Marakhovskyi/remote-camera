using System.Threading.Tasks;

namespace RemoteCameraControl.RemoteCameraControl.Interaction
{
    public interface ILoadingIndicator
    {
        Task ShowAsync(string status);

        Task HideAsync(bool forceInstantly);

        Task<bool> IsShownAsync();
    }
}