using System;
using System.IO;
using Android.Graphics;
using Android.Media;

namespace RemoteCameraControl.Photo
{
    public static class BitmapExtensions
    {
        public static Bitmap Rotate(this Bitmap resizedBitmap, MemoryStream bitmapStream)
        {
            ExifInterface exif = null;
            try
            {
                //exif = new ExifInterface(bitmapStream);
                string orientation = "5";//exif.GetAttribute(ExifInterface.TagOrientation);

                Matrix matrix = new Matrix();
                switch (orientation)
                {
                    case "1": // landscape
                        break;
                    case "3":
                        matrix.PreRotate(180);
                        resizedBitmap = Bitmap.CreateBitmap(resizedBitmap, 0, 0, resizedBitmap.Width, resizedBitmap.Height, matrix, false);
                        matrix.Dispose();
                        matrix = null;
                        break;
                    case "4":
                        matrix.PreRotate(180);
                        resizedBitmap = Bitmap.CreateBitmap(resizedBitmap, 0, 0, resizedBitmap.Width, resizedBitmap.Height, matrix, false);
                        matrix.Dispose();
                        matrix = null;
                        break;
                    case "5":
                        matrix.PreRotate(90);
                        resizedBitmap = Bitmap.CreateBitmap(resizedBitmap, 0, 0, resizedBitmap.Width, resizedBitmap.Height, matrix, false);
                        matrix.Dispose();
                        matrix = null;
                        break;
                    case "6": // portrait
                        matrix.PreRotate(90);
                        resizedBitmap = Bitmap.CreateBitmap(resizedBitmap, 0, 0, resizedBitmap.Width, resizedBitmap.Height, matrix, false);
                        matrix.Dispose();
                        matrix = null;
                        break;
                    case "7":
                        matrix.PreRotate(-90);
                        resizedBitmap = Bitmap.CreateBitmap(resizedBitmap, 0, 0, resizedBitmap.Width, resizedBitmap.Height, matrix, false);
                        matrix.Dispose();
                        matrix = null;
                        break;
                    case "8":
                        matrix.PreRotate(-90);
                        resizedBitmap = Bitmap.CreateBitmap(resizedBitmap, 0, 0, resizedBitmap.Width, resizedBitmap.Height, matrix, false);
                        matrix.Dispose();
                        matrix = null;
                        break;
                }

                return resizedBitmap;
            }

            catch (IOException ex)
            {
                Console.WriteLine("An exception was thrown when reading exif from media file...:" + ex.Message);
                return null;
            }
        }
    }

}