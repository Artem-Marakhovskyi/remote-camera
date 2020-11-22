using System.Threading.Tasks;

namespace RemoteCameraControl.Blue
{
    public interface IBluetooth
    {
        Task GetStatusAsync();
    }
}