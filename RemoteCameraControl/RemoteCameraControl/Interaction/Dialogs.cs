using Acr.UserDialogs;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace RemoteCameraControl.RemoteCameraControl.Interaction
{
    public class Dialogs : IDialogs
    {
        private readonly IUserDialogs _dialogs;
        private readonly ILoadingIndicator _loadingIndicator;
        private readonly IMultipleChoiceDialog _multipleChoiceDialog;
        private readonly ICancellableActionSheetDialogFactory _cancellableActionSheetDialogFactory;

        private bool _stashedLoadingIndicator;
        private string _loadingIndicatorMessage;


        public Dialogs(
            IUserDialogs dialogs,
            ILoadingIndicator loadingIndicator,
            IMultipleChoiceDialog multipleChoiceDialog,
            ICancellableActionSheetDialogFactory cancellableActionSheetDialogFactory)
        {
            _dialogs = dialogs;
            _loadingIndicator = loadingIndicator;
            _multipleChoiceDialog = multipleChoiceDialog;
            _cancellableActionSheetDialogFactory = cancellableActionSheetDialogFactory;
        }

        public Task AlertAsync(string message, string title, string okText)
        {
            return AlertAsync(message, title, okText, CancellationToken.None);
        }

        public async Task AlertAsync(
            string message,
            string title,
            string okText,
            CancellationToken ct)
        {
            await HideLoadingIfNeededAsync();

            await _dialogs
                .AlertAsync(
                    message,
                    title,
                    okText,
                    ct);

            await ShowLoadingIfNeededAsync();
        }

        public Task<bool> ConfirmAsync(
            string message,
            string title,
            string okText,
            string cancelText)
        {
            return ConfirmAsync(message, title, okText, cancelText, CancellationToken.None);
        }

        public async Task<bool> ConfirmAsync(
            string message,
            string title,
            string okText,
            string cancelText,
            CancellationToken ct)
        {
            await HideLoadingIfNeededAsync();

            var result = await _dialogs.ConfirmAsync(
                message,
                title,
                okText,
                cancelText,
                ct);

            await ShowLoadingIfNeededAsync();

            return result;
        }

        public Task<bool?> MultipleChoiceConfirmAsync(
            string message,
            string title,
            params string[] buttons)
        {
            if (string.IsNullOrWhiteSpace(message))
            {
                throw new ArgumentNullException(nameof(message));
            }

            if (buttons == null)
            {
                throw new ArgumentNullException(nameof(buttons));
            }

            if (buttons.Length == 0 || buttons.Length > 3)
            {
                throw new ArgumentOutOfRangeException(nameof(buttons));
            }

            if (buttons.Any(string.IsNullOrWhiteSpace))
            {
                throw new ArgumentNullException(nameof(buttons), "In one of its items");
            }

            return _multipleChoiceDialog.ShowAsync(
                message,
                title,
                buttons);
        }

        public void HideLoading(bool forceInstantly = false)
        {
            _ = _loadingIndicator.HideAsync(forceInstantly);
            _stashedLoadingIndicator = false;
            _loadingIndicatorMessage = null;
        }

        public void ShowLoading(string message = null)
        {
            message = string.IsNullOrWhiteSpace(message) ? "Loading..." : message;
            if (!_stashedLoadingIndicator)
            {
                _ = _loadingIndicator.ShowAsync(message);
            }

            _stashedLoadingIndicator = false;
            _loadingIndicatorMessage = message;
        }

        public Task<string> ShowActionSheetAsync(
            string title,
            string cancel,
            string destructive,
            string[] buttons,
            CancellationToken cancelToken = default(CancellationToken))
        {
            return _dialogs.ActionSheetAsync(
                title,
                cancel,
                destructive,
                cancelToken,
                buttons);
        }

        public Task<string> ShowCancellableActionSheetAsync(
            string title,
            string cancel,
            string[] buttons)
        {
            var dialog = _cancellableActionSheetDialogFactory.GetDialog();

            return dialog.ShowCancellableActionSheetAsync(
                title,
                cancel,
                buttons);
        }

        public IDisposable Toast(
            string title,
            TimeSpan? dismissTimer = null)
        {
            return _dialogs.Toast(
                title,
                dismissTimer);
        }

        public async Task<(bool, string)> PromptAsync(
            string message,
            string title,
            string okText,
            string cancelText,
            string placeholder,
            string inputText = null,
            int? maxLength = null,
            bool inputTypePassword = false)
        {
            PromptConfig config = new PromptConfig
            {
                Message = message,
                Title = title,
                Text = inputText,
                OkText = okText,
                CancelText = cancelText,
                Placeholder = placeholder,
                MaxLength = maxLength,
                InputType = inputTypePassword ? InputType.Password : InputType.Name,
            };
            var promtResult = await _dialogs.PromptAsync(config);
            var result = (promtResult.Ok, promtResult.Text);
            return result;
        }

        public async Task<(bool Ok, DateTime SelectedDate)> DatePromptAsync(
            DateTime? selectedDate = null,
            string title = null,
            string okText = null,
            string cancelText = null,
            DateTime? minimumDate = null,
            DateTime? maximumDate = null,
            CancellationToken cancelToken = default(CancellationToken))
        {
            var config = new DatePromptConfig
            {
                OkText = okText ?? "Ok",
                CancelText = cancelText ?? "Cancel",
                SelectedDate = selectedDate,
                MinimumDate = minimumDate,
                MaximumDate = maximumDate,
                Title = title,
            };

            var dialog = await _dialogs.DatePromptAsync(config, cancelToken);
            return (dialog.Ok, dialog.SelectedDate);
        }

        public IDisposable ShowActionSheet(
            string title,
            string cancel,
            string destructive,
            List<ActionSheetButtonModel> actionSheetButtons)
        {
            var config = new ActionSheetConfig();
            config.SetTitle(title);
            config.SetCancel(cancel);
            config.Destructive = string.IsNullOrEmpty(destructive) ? null : new ActionSheetOption(destructive);

            foreach (var button in actionSheetButtons)
            {
                config.Add(button.Text, button.Action, button.IconName);
            }

            return _dialogs.ActionSheet(config);
        }

        /// <summary>
        /// Hides loading indicator if it is shown.
        /// </summary>
        private async Task HideLoadingIfNeededAsync()
        {
            if (await _loadingIndicator.IsShownAsync())
            {
                await _loadingIndicator.HideAsync(forceInstantly: true);
                _stashedLoadingIndicator = true;
            }
        }

        /// <summary>
        /// Shows loading indicator only if it was hidden as a result of
        /// <see cref="HideLoadingIfNeededAsync"/> call.
        /// </summary>
        private async Task ShowLoadingIfNeededAsync()
        {
            if (_stashedLoadingIndicator)
            {
                await _loadingIndicator.ShowAsync(_loadingIndicatorMessage);
                _stashedLoadingIndicator = false;
            }
        }
    }
}