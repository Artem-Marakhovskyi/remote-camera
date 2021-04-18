using System;
using Android.Content.Res;
using Android.Runtime;
using Android.Views;
using static Android.Hardware.Camera;
using System.Threading.Tasks;
using System.Linq;
using Android.Graphics;
using System.IO;

namespace RemoteCameraControl.Android
{
    public class CameraSurface : SurfaceView, ISurfaceHolderCallback, IPictureCallback
    {
        private const int MinPictureSize = 1200;

        // Camera2 API has more capabilities but is more complex to use and not needed for current requirements of Umbella modules
        private global::Android.Hardware.Camera _camera;
        private TaskCompletionSource<byte[]> _tcs;
        private bool _shouldInitializeCamera;

        // a flag that would prevent from calling TakePicture() while previous picture is being saved
        private bool _safeToTakePicture;

        public CameraSurface(global::Android.Content.Context context)
            : base(context)
        {
            Holder.AddCallback(this);
        }

        public Task<byte[]> TakePhotoAsync()
        {
            _tcs = new TaskCompletionSource<byte[]>();

            if (_safeToTakePicture)
            {
                _safeToTakePicture = false;
                _camera.TakePicture(null, null, null, this);
            }

            return _tcs.Task;
        }

        public void OpenCamera(global::Android.Hardware.Camera camera)
        {
            _camera = camera;
            var cameraParams = _camera.GetParameters();

            if (Resources.Configuration.Orientation != global::Android.Content.Res.Orientation.Landscape)
            {
                cameraParams.Set("orientation", "portrait");
                _camera.SetDisplayOrientation(90);
                cameraParams.SetRotation(90);
            }
            else
            {
                cameraParams.Set("orientation", "landscape");
                _camera.SetDisplayOrientation(0);
                cameraParams.SetRotation(0);
            }

            var bestPictureSize = GetWorstPictureSize(cameraParams);
            if (bestPictureSize != default(Size))
            {
                cameraParams.SetPictureSize(bestPictureSize.Width, bestPictureSize.Height);
            }

            _camera.SetParameters(cameraParams);

            if (_shouldInitializeCamera)
            {
                _camera.SetPreviewDisplay(Holder);
                _camera.StartPreview();
                _shouldInitializeCamera = false;
            }
        }

        public void ReleaseResources()
        {
            _shouldInitializeCamera = false;
            _tcs = null;
        }

        public void SurfaceChanged(ISurfaceHolder holder, [GeneratedEnum] Format format, int width, int height)
        {
        }

        public void SurfaceCreated(ISurfaceHolder holder)
        {
            if (_camera == null)
            {
                _shouldInitializeCamera = true;
            }
            else
            {
                _camera.SetPreviewDisplay(Holder);
                _camera.StartPreview();
                _safeToTakePicture = true;
            }
        }

        public void SurfaceDestroyed(ISurfaceHolder holder)
        {
        }

        public async void OnPictureTaken(byte[] data, global::Android.Hardware.Camera camera)
        {
            if (data == null)
            {
                _safeToTakePicture = true;
                return;
            }

            var bitmap = BitmapFactory.DecodeByteArray(data, 0, data.Length);
            var ms = await bitmap.CompressAsync(50);


            _tcs.SetResult(ms.ToArray());
            _safeToTakePicture = true;
        }

        private Bitmap GetResizedBitmap(Bitmap image)
        {
            int width = image.Width;
            int height = image.Height;

            return Bitmap.CreateScaledBitmap(image, image.Width/10, image.Height/10, true);
        }

        protected override void Dispose(bool disposing)
        {
            ReleaseResources();

            base.Dispose(disposing);
        }

        private Size GetBestPictureSize(Parameters cameraParams)
        {
            var metrics = Resources.DisplayMetrics;
            var screenRatio = (float)metrics.WidthPixels / metrics.HeightPixels;

            //var bestPictureSize = cameraParams.SupportedPictureSizes
            //    .Where(it => it.Width > MinPictureSize && it.Height > MinPictureSize)
            //    .Where(it => it.Width < MinPictureSize * 2 || it.Height < MinPictureSize * 2)
            //    .OrderBy(it => it.Width / it.Height - screenRatio)
            //    .FirstOrDefault();

            var bestPictureSize = cameraParams.SupportedPictureSizes.Last();

            return bestPictureSize;
        }

        private Size GetWorstPictureSize(Parameters cameraParams)
        {
            var worstPictureSize = cameraParams.SupportedPictureSizes.First();

            return worstPictureSize;

        }
    }
}
