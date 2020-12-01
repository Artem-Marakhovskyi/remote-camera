using System;
using System.IO;
using System.Threading.Tasks;
using Android.Graphics;

namespace RemoteCameraControl.Android
{
    public static class BitmapExtensions
    {
        public static Bitmap CenterCrop(this Bitmap bitmap)
        {
            var isWidthSmaller = bitmap.Width < bitmap.Height;

            var smallerSide = isWidthSmaller ? bitmap.Width : bitmap.Height;

            var halfOfSidesDifference = Math.Abs(bitmap.Width - bitmap.Height) / 2;

            var left = isWidthSmaller ? 0 : halfOfSidesDifference;

            var top = isWidthSmaller ? halfOfSidesDifference : 0;

            var croppedBitmap = Bitmap.CreateBitmap(bitmap, left, top, smallerSide, smallerSide);

            return croppedBitmap;
        }

        /// <summary>
        /// Rounds bitmap corners.
        /// </summary>
        /// <param name="bitmap">Bitmap to process.</param>
        /// <param name="ratioToRound">Ratio of radius to round in dp to side in dp.</param>
        /// <returns>Bitmap with rounded corners.</returns>
        public static Bitmap Round(this Bitmap bitmap, float ratioToRound)
        {
            return bitmap.Round(ratioToRound, ratioToRound);
        }

        /// <summary>
        /// Rounds bitmap corners.
        /// </summary>
        /// <param name="bitmap">Bitmap to process.</param>
        /// <param name="ratioToRoundX">Ratio of radius to round in dp to width-side in dp.</param>
        /// <param name="ratioToRoundY">Ratio of radius to round in dp to height-side in dp.</param>
        /// <returns>Bitmap with rounded corners.</returns>
        public static Bitmap Round(this Bitmap bitmap, float ratioToRoundX, float ratioToRoundY)
        {
            var roundedBitmap = Bitmap.CreateBitmap(bitmap.Width, bitmap.Height, bitmap.GetConfig());

            using (var canvas = new Canvas(roundedBitmap))
            using (var shader = new BitmapShader(bitmap, Shader.TileMode.Clamp, Shader.TileMode.Clamp))
            using (var paint = new Paint(PaintFlags.AntiAlias))
            using (var rectangle = new RectF(0, 0, bitmap.Width, bitmap.Height))
            {
                paint.SetShader(shader);
                var radiusToRoundX = ratioToRoundX * bitmap.Width;
                var radiusToRoundY = ratioToRoundY * bitmap.Height;

                canvas.DrawRoundRect(rectangle, radiusToRoundX, radiusToRoundY, paint);

                return roundedBitmap;
            }
        }

        public static async Task<MemoryStream> CompressAsync(this Bitmap bitmap, int quality = 100)
        {
            var memoryStream = new MemoryStream();
            await bitmap.CompressAsync(Bitmap.CompressFormat.Jpeg, quality, memoryStream);

            return memoryStream;
        }
    }
}
