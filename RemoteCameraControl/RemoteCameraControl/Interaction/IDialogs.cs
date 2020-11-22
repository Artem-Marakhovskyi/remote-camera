using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;

namespace RemoteCameraControl.RemoteCameraControl.Interaction
{
public interface IDialogs
    {
        void ShowLoading(string message = null);

        void HideLoading(bool forceInstantly = false);

        Task<bool> ConfirmAsync(
            string message,
            string title,
            string okText,
            string cancelText);

        Task<bool> ConfirmAsync(
            string message,
            string title,
            string okText,
            string cancelText,
            CancellationToken ct);

        Task<bool?> MultipleChoiceConfirmAsync(
            string message,
            string title,
            params string[] buttons);

        Task AlertAsync(
            string message,
            string title,
            string okText);

        Task AlertAsync(
            string message,
            string title,
            string okText,
            CancellationToken ct);

        IDisposable ShowActionSheet(
           string title,
           string cancel,
           string destructive,
           List<ActionSheetButtonModel> actionSheetButtons);

        Task<string> ShowActionSheetAsync(
            string title,
            string cancel,
            string destructive,
            string[] buttons,
            CancellationToken cancelToken = default(CancellationToken));

        Task<string> ShowCancellableActionSheetAsync(
            string title,
            string cancel,
            string[] buttons);

        IDisposable Toast(
            string title,
            TimeSpan? dismissTimer = null);

        Task<(bool, string)> PromptAsync(
            string message,
            string title,
            string okText,
            string cancelText,
            string placeholder,
            string inputText = null,
            int? maxLength = null,
            bool inputTypePassword = false);

        Task<(bool Ok, DateTime SelectedDate)> DatePromptAsync(
            DateTime? selectedDate = null,
            string title = null,
            string okText = null,
            string cancelText = null, 
            DateTime? minimumDate = null,
            DateTime? maximumDate = null,
            CancellationToken cancelToken = default(CancellationToken));
    }
}