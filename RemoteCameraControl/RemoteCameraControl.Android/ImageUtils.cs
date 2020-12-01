using System;
using System.IO;
using Android.Content;
using Android.Graphics;
using AndroidUri = Android.Net.Uri;

namespace RemoteCameraControl.Android
{
    public static class ImageUtils
    {
        public static Bitmap Decode(
            string filePath,
            int requiredWidth,
            int requiredHeight)
        {
            using (var options = new BitmapFactory.Options())
            {
                // setting up InJustDecodeBounds to not allocate memory for bitmap
                // Call is used to get options configuration.
                options.InJustDecodeBounds = true;
                BitmapFactory.DecodeFile(filePath, options);

                options.InSampleSize = GetInSampleSize(options, requiredWidth, requiredHeight);

                options.InJustDecodeBounds = false;

                return BitmapFactory.DecodeFile(filePath, options);
            }
        }

        public static Bitmap FromByteArray(byte[] byteArray, double height, double width)
        {
            if (height <= 0)
            {
                throw new ArgumentException($"{nameof(height)} argument shouldn't be negative or null");
            }

            if (width <= 0)
            {
                throw new ArgumentException($"{nameof(width)} argument shouldn't be negative or null");
            }

            var bitmap = DecodeBitmap(
                (bitmapFactoryOptions) => BitmapFactory.DecodeByteArray(
                    byteArray,
                    0,
                    byteArray.Length,
                    bitmapFactoryOptions),
                height,
                width);

            return bitmap;
        }

        public static Bitmap FromFile(string fileAbsolutePath, double height, double width)
        {
            if (string.IsNullOrEmpty(fileAbsolutePath))
            {
                throw new ArgumentNullException(nameof(fileAbsolutePath));
            }

            if (height < 0)
            {
                throw new ArgumentException($"{nameof(height)} argument shouldn't be negative or null");
            }

            if (width < 0)
            {
                throw new ArgumentException($"{nameof(width)} argument shouldn't be negative or null");
            }

            var bitmap = DecodeBitmap(
                (bitmapFactoryOptions) => BitmapFactory.DecodeFile(
                    fileAbsolutePath, bitmapFactoryOptions),
                height,
                width);

            return bitmap;
        }

        public static Bitmap FromStream(Stream stream, double height, double width)
        {
            if (height < 0)
            {
                throw new ArgumentException($"{nameof(height)} argument shouldn't be negative or null");
            }

            if (width < 0)
            {
                throw new ArgumentException($"{nameof(width)} argument shouldn't be negative or null");
            }

            var bitmap = DecodeBitmap(
                (bitmapFactoryOptions) =>
                {
                    var result = BitmapFactory.DecodeStream(
                        stream,
                        null,
                        bitmapFactoryOptions);

                    stream.Position = 0;

                    return result;
                },
                height,
                width);

            return bitmap;
        }

        public static Bitmap FromUri(
            ContentResolver contentResolver,
            AndroidUri uri,
            double height,
            double width)
        {
            if (height < 0)
            {
                throw new ArgumentException($"{nameof(height)} argument shouldn't be negative or null");
            }

            if (width < 0)
            {
                throw new ArgumentException($"{nameof(width)} argument shouldn't be negative or null");
            }

            var bitmap = DecodeBitmap(
                (bitmapFactoryOptions) =>
                {
                    using (var stream = contentResolver.OpenInputStream(uri))
                    {
                        return BitmapFactory.DecodeStream(stream, null, bitmapFactoryOptions);
                    }
                },
                height,
                width);

            return bitmap;
        }

        private static Bitmap DecodeBitmap(
            Func<BitmapFactory.Options, Bitmap> decodeBitmap,
            double height,
            double width)
        {
            using (var bitmapFactoryOptions = new BitmapFactory.Options { InJustDecodeBounds = true, })
            {
                decodeBitmap(bitmapFactoryOptions);

                bitmapFactoryOptions.InSampleSize =
                    FindInSampleSize(
                        bitmapFactoryOptions.OutHeight,
                        bitmapFactoryOptions.OutWidth,
                        height,
                        width);

                bitmapFactoryOptions.InJustDecodeBounds = false;

                var bitmap = decodeBitmap(bitmapFactoryOptions);

                return bitmap;
            }
        }

        private static int GetInSampleSize(
            BitmapFactory.Options options,
            int requiredWidth,
            int requiredHeight)
        {
            var height = options.OutHeight;
            var width = options.OutWidth;
            var inSampleSize = 1;

            if (height > requiredHeight || width > requiredWidth)
            {
                var halfHeight = height / 2;
                var halfWidth = width / 2;

                while ((halfHeight / inSampleSize) >= requiredHeight
                        && (halfWidth / inSampleSize) >= requiredWidth)
                {
                    inSampleSize *= 2;
                }
            }

            return inSampleSize;
        }

        private static int FindInSampleSize(
            int outHeight,
            int outWidth,
            double height,
            double width)
        {
            var inSampleSize = 1;

            if (height >= 0 && width >= 0)
            {
                // Calculate the largest inSampleSize value
                // that is a power of 2 and keeps both height and width
                // larger than the requested height and width.
                while (outHeight / (2 * inSampleSize) >= height
                    && outWidth / (2 * inSampleSize) >= width)
                {
                    inSampleSize *= 2;
                }
            }

            return inSampleSize;
        }
    }
}

