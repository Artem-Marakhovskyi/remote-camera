using System.Threading.Tasks;

namespace RemoteCameraControl.RemoteCameraControl.Interaction
{
    public interface IPlatformLoadingIndicator
    {
        Task ShowAsync(string status);

        Task HideAsync();
    }
}