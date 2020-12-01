using System;
using System.IO;
using System.Threading.Tasks;
using Android.Content;
using Android.Graphics;
using Android.OS;
using Java.IO;
using RemoteCameraControl.Logger;
using ExifInterface = Android.Media.ExifInterface;
using Orientation = Android.Media.Orientation;
using Uri = Android.Net.Uri;
using File = Java.IO.File;

namespace RemoteCameraControl.Android
{
    public static class ImageRotationExtensions
    {
        private static ILogger _logger = XLabs.Ioc.Resolver.Resolve<ILogger>();

        [System.Diagnostics.CodeAnalysis.SuppressMessage("Design", "CA1031:Do not catch general exception types", Justification = "Exception is handled and logged")]
        public static async Task<MemoryStream> RotatePhotoAsync(
            this Uri uri,
            ContentResolver contentResolver,
            int width,
            int height)
        {
            ExifInterface exifInterface = null;

            try
            {
                using (var stream = contentResolver.OpenInputStream(uri))
                {
                    if ((int)Build.VERSION.SdkInt > 23)
                    {
                        exifInterface = new ExifInterface(stream);
                    }
                    else
                    {
                        exifInterface = new ExifInterface(uri.Path);
                    }
                }

                using (var matrix = GetOrientation(exifInterface))
                using (var bitmap = ImageUtils.FromUri(contentResolver, uri, height, width))
                {
                    if (matrix.IsIdentity)
                    {
                        return await bitmap.CompressAsync();
                    }

                    using (var rotatedBitmap = RotateBitmap(bitmap, matrix))
                    {
                        return await rotatedBitmap.CompressAsync();
                    }
                }
            }
            catch (Exception e)
            {
                _logger.LogError("Photo rotation error has occurred", e);
                return null;
            }
            finally
            {
                exifInterface?.Dispose();
            }
        }

        [System.Diagnostics.CodeAnalysis.SuppressMessage("Design", "CA1031:Do not catch general exception types", Justification = "Exception is handled and logged")]
        public static async Task<MemoryStream> RotatePhotoAsync(
            this byte[] imageData,
            string tempDirectory,
            int requiredWidth,
            int requiredHeight)
        {
            ExifInterface exifInterface = null;
            var memoryStream = new MemoryStream(imageData);

            try
            {
                if ((int)Build.VERSION.SdkInt > 23)
                {
                    exifInterface = new ExifInterface(memoryStream);
                }
                else
                {
                    var file = await SavePhotoToDiskAsync(imageData, tempDirectory).ConfigureAwait(false);
                    if (file == null)
                    {
                        return memoryStream;
                    }

                    exifInterface = new ExifInterface(file.CanonicalPath);

                    DeletePhotoFromDisk(file);
                }

                using (var options = new BitmapFactory.Options())
                using (var matrix = GetOrientation(exifInterface))
                {
                    if (matrix.IsIdentity)
                    {
                        memoryStream.Position = 0;
                        return memoryStream;
                    }

                    options.InJustDecodeBounds = true;
                    options.InPreferredConfig = Bitmap.Config.Rgb565;
                    memoryStream.Position = 0;

                    await BitmapFactory.DecodeStreamAsync(memoryStream, null, options).ConfigureAwait(false);
                    var inSampleSize = GetInSampleSize(options, requiredWidth, requiredHeight);

                    options.InJustDecodeBounds = false;
                    options.InSampleSize = inSampleSize;
                    memoryStream.Position = 0;

                    using (var bitmap = await BitmapFactory.DecodeStreamAsync(memoryStream, null, options))
                    using (var rotatedBitmap = RotateBitmap(bitmap, matrix))
                    {
                        memoryStream.Dispose();

                        return await rotatedBitmap.CompressAsync();
                    }
                }
            }
            catch (Exception e)
            {
                _logger.LogError("Photo rotation error has occurred", e);
                return memoryStream;
            }
            finally
            {
                exifInterface?.Dispose();
            }
        }

        private static int GetInSampleSize(BitmapFactory.Options options, int requiredWidth, int requiredHeight)
        {
            float height = options.OutHeight;
            float width = options.OutWidth;

            if (requiredWidth == 0)
            {
                requiredWidth = (int)((requiredHeight / height) * width);
            }

            if (requiredHeight == 0)
            {
                requiredHeight = (int)((requiredWidth / width) * height);
            }

            var inSampleSize = 1;

            if (height > requiredHeight || width > requiredWidth)
            {
                var heightRatio = (int)Math.Round(height / requiredHeight) + 1;
                var widthRatio = (int)Math.Round(width / requiredWidth) + 1;
                inSampleSize = Math.Min(heightRatio, widthRatio);
            }

            return inSampleSize;
        }

        private static Matrix GetOrientation(ExifInterface exifInterface)
        {
            var orientation = (Orientation)exifInterface.GetAttributeInt(ExifInterface.TagOrientation, (int)Orientation.Undefined);

            var matrix = new Matrix();
            switch (orientation)
            {
                case Orientation.Normal:
                    break;
                case Orientation.FlipHorizontal:
                    matrix.SetScale(-1, 1);
                    break;
                case Orientation.Rotate180:
                    matrix.SetRotate(180);
                    break;
                case Orientation.FlipVertical:
                    matrix.SetRotate(180);
                    matrix.PostScale(-1, 1);
                    break;
                case Orientation.Transpose:
                    matrix.SetRotate(90);
                    matrix.PostScale(-1, 1);
                    break;
                case Orientation.Rotate90:
                    matrix.SetRotate(90);
                    break;
                case Orientation.Transverse:
                    matrix.SetRotate(-90);
                    matrix.PostScale(-1, 1);
                    break;
                case Orientation.Rotate270:
                    matrix.SetRotate(-90);
                    break;
                case Orientation.Undefined:
                    _logger.LogDebug("Undefined photo orientation");
                    break;
            }

            return matrix;
        }

        [System.Diagnostics.CodeAnalysis.SuppressMessage("Design", "CA1031:Do not catch general exception types", Justification = "Exception is logged")]
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Reliability", "CA2000:Dispose objects before losing scope", Justification = "File is used outside this method.")]
        private static async Task<Java.IO.File> SavePhotoToDiskAsync(byte[] data, string directory)
        {
            Java.IO.File pictureFile;
            string photoFileName;
            using (var pictureFileDir = new Java.IO.File(directory))
            {
                if (!pictureFileDir.Exists() && !pictureFileDir.Mkdirs())
                {
                    _logger.LogError("Can't create directory to save image");
                    return null;
                }

                photoFileName = $"Picture-{Guid.NewGuid().ToString()}.jpg";
                var imageFilePath = $"{pictureFileDir.Path}{Java.IO.File.Separator}{photoFileName}";
                pictureFile = new Java.IO.File(imageFilePath);
            }

            FileOutputStream fileOutputStream = null;
            try
            {
                fileOutputStream = new FileOutputStream(pictureFile);
                await fileOutputStream.WriteAsync(data);
            }
            catch (Exception e)
            {
                _logger.LogError($"File {photoFileName} has not been saved: {e.Message}", e);
            }
            finally
            {
                fileOutputStream?.Close();
                fileOutputStream?.Dispose();
            }

            return pictureFile;
        }

        [System.Diagnostics.CodeAnalysis.SuppressMessage("Design", "CA1031:Do not catch general exception types", Justification = "Exception is logged")]
        private static void DeletePhotoFromDisk(Java.IO.File file)
        {
            try
            {
                var deleted = file.Delete();

                if (!deleted)
                {
                    _logger.LogInfo($"File has not been deleted: {file.AbsolutePath}");
                }
            }
            catch (Exception e)
            {
                _logger.LogError($"Can't delete file: {file.AbsolutePath}", e);
            }
            finally
            {
                file.Dispose();
            }
        }

        private static Bitmap RotateBitmap(Bitmap bitmap, Matrix matrix)
        {
            return Bitmap.CreateBitmap(bitmap, 0, 0, bitmap.Width, bitmap.Height, matrix, true);
        }
    }
}
