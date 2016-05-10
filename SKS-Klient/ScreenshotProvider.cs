using System.Drawing;
using System.Drawing.Imaging;
using System.IO;
using System.IO.Compression;
using System.Windows.Forms;

namespace SKS_Klient
{
    class ScreenshotProvider
    {
        public static byte[] GetScreenshot()
        {
            Bitmap bitmap = new Bitmap(Screen.PrimaryScreen.Bounds.Width, Screen.PrimaryScreen.Bounds.Height, PixelFormat.Format24bppRgb);
            Graphics screenshot = Graphics.FromImage(bitmap);
            screenshot.CopyFromScreen(Screen.PrimaryScreen.Bounds.X, Screen.PrimaryScreen.Bounds.Y, 0, 0, Screen.PrimaryScreen.Bounds.Size, CopyPixelOperation.SourceCopy);
            ImageConverter converter = new ImageConverter();
            byte[] screenshotBytes = (byte[])converter.ConvertTo(bitmap, typeof(byte[]));
            return Compress(screenshotBytes);
        }

        private static byte[] Compress(byte[] oryginalBytes)
        {
            using (var msi = new MemoryStream(oryginalBytes))
            using (var mso = new MemoryStream())
            {
                using (var gs = new DeflateStream(mso, CompressionMode.Compress))
                {
                    msi.CopyTo(gs);
                }

                return mso.ToArray();
            }
        }
    }
}
