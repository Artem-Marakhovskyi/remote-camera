using System;
using System.Threading.Tasks;
using RemoteCameraControl.RemoteCameraControl.Interaction;

namespace RemoteCameraControl.Interaction
{
    public static class DialogsExtensions
    {
        //public static Task AlertOnConnectionIssueDuringContextSelection(this IDialogs me)
        //{
        //    me.ThrowIfArgumentNull(nameof(me));

        //    return me.AlertAsync(
        //        Resources.Resources.ConnectionRequiredToSelectContract,
        //        Resources.Resources.WarningText,
        //        Resources.Resources.Ok);
        //}

        //public static Task AlertOnConnectionIssuesDuringLoginAsync(this IDialogs me)
        //{
        //    me.ThrowIfArgumentNull(nameof(me));

        //    return me.AlertAsync(
        //        Resources.Resources.ConnectionLostDuringLoginProcess,
        //        Resources.Resources.Error,
        //        Resources.Resources.Ok);
        //}

        //public static Task AlertOnErrorAsync(this IDialogs me)
        //{
        //    me.ThrowIfArgumentNull(nameof(me));

        //    return me.AlertAsync(
        //        Resources.Resources.ErrorOccurred,
        //        Resources.Resources.Error,
        //        Resources.Resources.Ok);
        //}

        //public static Task AlertPasswordExpiredAsync(this IDialogs me)
        //{
        //    me.ThrowIfArgumentNull(nameof(me));

        //    return me.AlertAsync(
        //        Resources.Resources.PasswordExpiredDescription,
        //        Resources.Resources.PasswordExpired,
        //        Resources.Resources.Ok);
        //}

        public static Task<bool> ConfirmOnUnsavedDataAsync(this IDialogs me)
        {
            return me.ConfirmAsync(
                "You are leaving screen without saving the progress",
                string.Empty,
                "Stay",
                "Leave");
        }
    }
}
