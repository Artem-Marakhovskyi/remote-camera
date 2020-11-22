using System;
using System.Threading.Tasks;
using Android.App;
using Android.OS;
using RemoteCameraControl.RemoteCameraControl.Interaction;

namespace RemoteCameraControl.Android.Interaction
{
public class MultipleChoiceFragment : DialogFragment, IMultipleChoiceDialog
    {
        private TaskCompletionSource<bool?> _taskCompletionSource;

        private string _title;
        private string _mainMessage;
        private string[] _buttons;

        public override void OnPause()
        {
            base.OnPause();

            Dismiss();
            Cancel();
        }

        /// <summary>
        /// Shows a popup with specified message.
        /// </summary>
        /// <returns>
        /// Task with result of <see cref="Nullable"/>.
        /// Returns <c>true</c> if user clicks on positive button,
        /// <c>false</c> if clicks on negative button,
        /// <c>null</c> if user cancels popup OR <see cref="OnPause"/>
        /// is called by Android Runtime. Fragment dismisses itself if
        /// <see cref="OnPause"/> is called.</returns>
        public Task<bool?> ShowAsync(string message, string title, params string[] buttons)
        {
            _taskCompletionSource = new TaskCompletionSource<bool?>();

            _mainMessage = message;
            _title = title;
            _buttons = buttons;
            
            Show(GalaSoft.MvvmLight.Views.ActivityBase.CurrentActivity.FragmentManager, nameof(MultipleChoiceFragment));

            return _taskCompletionSource.Task;
        }

        public override Dialog OnCreateDialog(Bundle savedInstanceState)
        {
            AlertDialog.Builder builder = new AlertDialog.Builder(Context);
            builder.SetTitle(_title);
            builder.SetMessage(_mainMessage);
            SetButtons(builder);
            Cancelable = false;

            return builder.Create();
        }

        private void SetButtons(AlertDialog.Builder builder)
        {
            switch (_buttons.Length)
            {
                case 1:
                    {
                        builder.SetPositiveButton(_buttons[0], (sender, e) => { _taskCompletionSource.TrySetResult(true); });
                        break;
                    }
                case 2:
                    {
                        builder.SetPositiveButton(_buttons[0], (sender, e) => { _taskCompletionSource.TrySetResult(true); });
                        builder.SetNegativeButton(_buttons[1], (sender, e) => { _taskCompletionSource.TrySetResult(false); });
                        break;
                    }
                case 3:
                    {
                        builder.SetPositiveButton(_buttons[0], (sender, e) => { _taskCompletionSource.TrySetResult(true); });
                        builder.SetNegativeButton(_buttons[1], (sender, e) => { _taskCompletionSource.TrySetResult(false); });
                        builder.SetNeutralButton(_buttons[2], (sender, e) => { _taskCompletionSource.TrySetResult(null); });
                        break;
                    }
            }
        }

        private void Cancel()
        {
            _taskCompletionSource.TrySetResult(null);
        }
    }
}