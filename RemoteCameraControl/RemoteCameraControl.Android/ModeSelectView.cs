
using System;
using Android.App;
using Android.Content.PM;
using Android.Graphics;
using Android.OS;
using Android.Views;
using Android.Widget;
using GalaSoft.MvvmLight.Helpers;
using RemoteCameraControl.Android.SelectMode;
using ZXing;
using ZXing.Common;
using ZXing.Mobile;

namespace RemoteCameraControl.Android
{
    [Activity(Label = "ModeSelectView", Theme = "@style/AppTheme", LaunchMode = LaunchMode.SingleTask)]
    public class ModeSelectView : ActivityBase<ModeSelectViewModel>
    {
        private ImageView _barcodeImage;
        private View _initialLayout;
        private TextView _cameraConnect;
        private View _cameraAwaitLayout;
        private EditText _sessionNameEditText;
        private MobileBarcodeScanner _mobileBarcodeScanner;
        private Button _barcodeButton;

        protected override void OnCreate(Bundle savedInstanceState)
        {
            base.OnCreate(savedInstanceState);

            SetContentView(Resource.Layout.mode_select_view);

            var cameraButton = FindViewById<Button>(Resource.Id.be_camera_button);
            cameraButton.Click += CameraButton_Click;

            var rcButton = FindViewById<Button>(Resource.Id.be_rc_button);
            rcButton.Click += RcButton_Click;
            _mobileBarcodeScanner = new MobileBarcodeScanner();
            _barcodeButton = FindViewById<Button>(Resource.Id.barcodeButton);
            _barcodeButton.Click += OnBarcodeButton;

            _barcodeImage = FindViewById<ImageView>(Resource.Id.barcodeImage);
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

        private async void OnBarcodeButton(object sender, EventArgs e)
        {
            _mobileBarcodeScanner.UseCustomOverlay = false;

            //We can customize the top and bottom text of the default overlay
            _mobileBarcodeScanner.TopText = "Hold the camera up to the barcode\nAbout 6 inches away";
            _mobileBarcodeScanner.BottomText = "Wait for the barcode to automatically scan!";

            ////Start scanning
            var result = await _mobileBarcodeScanner.Scan();


            _sessionNameEditText.Text = result.Text;
            ViewModel.BecomeRc();
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

                try
                {
                    BitMatrix bitmapMatrix = null;
                    var message = _cameraConnect.Text;
                    bitmapMatrix = new MultiFormatWriter().encode(message, BarcodeFormat.QR_CODE, 660, 660);

                    var width = bitmapMatrix.Width;
                    var height = bitmapMatrix.Height;
                    int[] pixelsImage = new int[width * height];

                    for (int i = 0; i < height; i++)
                    {
                        for (int j = 0; j < width; j++)
                        {
                            if (bitmapMatrix[j, i])
                                pixelsImage[i * width + j] = (int)Convert.ToInt64(0xff000000);
                            else
                                pixelsImage[i * width + j] = (int)Convert.ToInt64(0xffffffff);

                        }
                    }

                    Bitmap bitmap = Bitmap.CreateBitmap(width, height, Bitmap.Config.Argb8888);
                    bitmap.SetPixels(pixelsImage, 0, width, 0, 0, width, height);

                    _barcodeImage.SetImageBitmap(bitmap);
                }
                catch (Exception ex)
                {
                    Console.WriteLine($"Exception {ex} ");
                }
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
