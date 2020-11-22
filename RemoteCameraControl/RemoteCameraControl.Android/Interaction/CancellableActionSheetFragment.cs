using System.Threading.Tasks;
using Android.App;
using Android.Content;
using Android.OS;
using RemoteCameraControl.RemoteCameraControl.Interaction;

namespace RemoteCameraControl.Android.Interaction
{
    public class CancellableActionSheetFragment :
        DialogFragment,
        ICancellableActionSheetDialog,
        ICancellableActionSheetDialogFactory
    {
        private TaskCompletionSource<string> _taskCompletionSource;

        private string _title;
        private string _cancel;
        private string[] _buttons;

        public override void OnPause()
        {
            base.OnPause();

            Cancel();
            Dismiss();
        }

        public Task<string> ShowCancellableActionSheetAsync(string title, string cancel, params string[] buttons)
        {
            _taskCompletionSource = new TaskCompletionSource<string>();

            _title = title;
            _cancel = cancel;
            _buttons = buttons;
                    
            Show(GalaSoft.MvvmLight.Views.ActivityBase.CurrentActivity.FragmentManager, nameof(CancellableActionSheetFragment));
            
            return _taskCompletionSource.Task;
        }

        public override Dialog OnCreateDialog(Bundle savedInstanceState)
        {
            AlertDialog.Builder builder = new AlertDialog.Builder(Context);
            builder.SetTitle(_title);
            builder.SetNeutralButton(_cancel, (sender, e) => Cancel());
            builder.SetItems(_buttons, (sender, e) => { _taskCompletionSource.TrySetResult(_buttons[e.Which]); });

            return builder.Create();
        }

        public override void OnCancel(IDialogInterface dialog)
        {
            base.OnCancel(dialog);

            Cancel();
        }

        private void Cancel() => _taskCompletionSource.TrySetResult(_cancel);

        public ICancellableActionSheetDialog GetDialog()
        {
            return new CancellableActionSheetFragment();
        }
    }
}