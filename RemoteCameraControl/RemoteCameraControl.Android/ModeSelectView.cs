
using System;
using Android.App;
using Android.Graphics;
using Android.OS;
using Android.Views;
using Android.Widget;
using GalaSoft.MvvmLight.Helpers;
using RemoteCameraControl.Android.SelectMode;
using ZXing;

namespace RemoteCameraControl.Android
{
    [Activity(Label = "ModeSelectView", Theme = "@style/AppTheme")]
    public class ModeSelectView : ActivityBase<ModeSelectViewModel>
    {
        private View _initialLayout;
        private TextView _cameraConnect;
        private View _cameraAwaitLayout;
        private EditText _sessionNameEditText;

        protected override void OnCreate(Bundle savedInstanceState)
        {
            base.OnCreate(savedInstanceState);

            SetContentView(Resource.Layout.mode_select_view);

            var cameraButton = FindViewById<Button>(Resource.Id.be_camera_button);
            cameraButton.Click += CameraButton_Click;

            var rcButton = FindViewById<Button>(Resource.Id.be_rc_button);
            rcButton.Click += RcButton_Click;

            _initialLayout = FindViewById(Resource.Id.linLayout);
            _cameraConnect = FindViewById<TextView>(Resource.Id.camera_connect_to_a_session);
            _cameraAwaitLayout = FindViewById(Resource.Id.cameraIsWaitingLinearLayout);
            _sessionNameEditText = FindViewById<EditText>(Resource.Id.session_name);

            this.SetBinding(
                () => ViewModel.SessionName,
                () => _sessionNameEditText.Text,
                BindingMode.TwoWay);

            ViewModel.PropertyChanged += ViewModel_PropertyChanged;
            OnCameraAwaitSessionName();
        }

        private void ViewModel_PropertyChanged(object sender, System.ComponentModel.PropertyChangedEventArgs e)
        {
            if (e.PropertyName == nameof(ViewModel.CameraAwaitSessionName))
            {
                OnCameraAwaitSessionName();
            }
        }

        private void OnCameraAwaitSessionName()
        {
            _cameraConnect.Text = ViewModel.CameraAwaitSessionName;
            
            if (string.IsNullOrWhiteSpace(ViewModel.CameraAwaitSessionName))
            {
                _initialLayout.Visibility = ViewStates.Visible;
                _cameraAwaitLayout.Visibility = ViewStates.Gone;
            }
            else
            {
                _initialLayout.Visibility = ViewStates.Gone;
                _cameraAwaitLayout.Visibility = ViewStates.Visible;
            }
        }

        private void RcButton_Click(object sender, System.EventArgs e)
        {
            ViewModel.BecomeRc();
        }

        private void CameraButton_Click(object sender, System.EventArgs e)
        {
            ViewModel.BecomeCamera();
        }
    }
}
