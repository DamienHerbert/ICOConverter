using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using System.Drawing;
using System.Drawing.Imaging;
using System.Drawing.Drawing2D;

namespace ICOConverter
{
    class Program
    {
        static void Main(string[] args)
        {
            List<string> images = new List<string>();
            images.AddRange(System.IO.Directory.GetFiles(System.IO.Directory.GetCurrentDirectory(), "*.jpg"));
            images.AddRange(System.IO.Directory.GetFiles(System.IO.Directory.GetCurrentDirectory(), "*.jpeg"));
            images.AddRange(System.IO.Directory.GetFiles(System.IO.Directory.GetCurrentDirectory(), "*.png"));

            System.IO.Directory.CreateDirectory(System.IO.Directory.GetCurrentDirectory() + "\\output");
            foreach(string image in images)
            {
                ICOConverter icoConverter = new ICOConverter(image, System.IO.Path.GetDirectoryName(image) + "\\output\\" + System.IO.Path.GetFileNameWithoutExtension(image) + ".ico");
                icoConverter.Convert();
            }
        }

        public static Bitmap ResizeImage(Image image, int width, int height)
        {
            var destRect = new Rectangle(0, 0, width, height);
            var destImage = new Bitmap(width, height);

            destImage.SetResolution(image.HorizontalResolution, image.VerticalResolution);

            using (var graphics = Graphics.FromImage(destImage))
            {
                graphics.CompositingMode = CompositingMode.SourceCopy;
                graphics.CompositingQuality = CompositingQuality.HighQuality;
                graphics.InterpolationMode = InterpolationMode.HighQualityBicubic;
                graphics.SmoothingMode = SmoothingMode.HighQuality;
                graphics.PixelOffsetMode = PixelOffsetMode.HighQuality;

                using (var wrapMode = new ImageAttributes())
                {
                    wrapMode.SetWrapMode(WrapMode.TileFlipXY);
                    graphics.DrawImage(image, destRect, 0, 0, image.Width, image.Height, GraphicsUnit.Pixel, wrapMode);
                }
            }

            return destImage;
        }

        public static Image padImage(Image inputImage)
        {
            int size = inputImage.Width > inputImage.Height ? inputImage.Width : inputImage.Height;
            Bitmap padedImage = new Bitmap(size, size);
            using (Graphics graphics = Graphics.FromImage(padedImage))
            {
                graphics.FillRectangle(Brushes.Transparent, 0, 0, size, size);
                graphics.CompositingQuality = System.Drawing.Drawing2D.CompositingQuality.HighQuality;
                graphics.InterpolationMode = System.Drawing.Drawing2D.InterpolationMode.HighQualityBicubic;
                graphics.SmoothingMode = System.Drawing.Drawing2D.SmoothingMode.HighQuality;

                graphics.DrawImage(inputImage, size / 2 - inputImage.Width / 2, size / 2 - inputImage.Height / 2, inputImage.Width, inputImage.Height);
            }
            return padedImage;

        }

    }
}
