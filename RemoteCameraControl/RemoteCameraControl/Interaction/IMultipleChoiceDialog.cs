using System.Threading.Tasks;

namespace RemoteCameraControl.RemoteCameraControl.Interaction
{
    public interface IMultipleChoiceDialog
    {
        Task<bool?> ShowAsync(string message, string title, params string[] buttons);
    }
}