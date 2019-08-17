using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;



using System.IO;
using System.Drawing;
using System.Drawing.Drawing2D;

//standard ico image sizes (16, 32, 48, 256) (24bit colour, 8bit transparency)


namespace ICOConverter
{
    class ICOConverter
    {
        Image inputImage;
        string outputPath;
        int[] sizes = { 16, 32, 48, 256 };

        //Constructors
        ICOConverter() { }
        public ICOConverter(string input, string output)
        {
            inputImage = Image.FromFile(input);
            outputPath = output;

            //Pad Image
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
            inputImage = padedImage;
        }

        //Scale the origional image to the provides size. Note: The images height and withe are made the same in the constructer by pading the image with a transparent background
        Image scaleImage(int size)
        {
            Rectangle rec = new Rectangle(0, 0, size, size);
            Bitmap resizedImage = new Bitmap(size, size);
            resizedImage.SetResolution(inputImage.HorizontalResolution, inputImage.VerticalResolution);

            using (Graphics graphics = Graphics.FromImage(resizedImage))
            {
                graphics.CompositingMode = CompositingMode.SourceCopy;
                graphics.CompositingQuality = CompositingQuality.HighQuality;
                graphics.InterpolationMode = InterpolationMode.HighQualityBicubic;
                graphics.SmoothingMode = SmoothingMode.HighQuality;
                graphics.PixelOffsetMode = PixelOffsetMode.HighQuality;

                using (System.Drawing.Imaging.ImageAttributes wrapmode = new System.Drawing.Imaging.ImageAttributes())
                {
                    wrapmode.SetWrapMode(WrapMode.TileFlipXY);
                    graphics.DrawImage(inputImage, rec, 0, 0, inputImage.Width, inputImage.Height, GraphicsUnit.Pixel, wrapmode);
                }
                
            }
            return resizedImage;
        }
        //converts the image inputImage to a .ico file and outputs it to the path provided by outputPath
        public bool Convert()
        {
            if(!File.Exists(outputPath))
            {
                using (FileStream icon = File.Create(outputPath))
                using (BinaryWriter binaryWriter = new BinaryWriter(icon))
                {
                    binaryWriter.Write(new byte[] { 0, 0, 1, 0, 4, 0 });
                    int ofset = 6 + 16 * sizes.Length;
                    Console.Write("Output: " + outputPath + "\0");

                    foreach (int size in sizes)
                    {
                        Console.Write("size: " + size + "\0");
                        using (MemoryStream imageStream = new MemoryStream())
                        {
                            binaryWriter.Write(new byte[]
                            {
                                (byte)size, (byte)size,
                                0, 0, 0, 0, 32, 0
                            });
                            scaleImage(size).Save(imageStream, System.Drawing.Imaging.ImageFormat.Png);

                            binaryWriter.Write((int)imageStream.Length);
                            binaryWriter.Write(ofset);
                            //binaryWriter.Write(imageStream.ToArray());

                            //ofset += 16 + (int)imageStream.Length;
                            ofset += (int)imageStream.Length;
                        }
                    }
                    foreach (int size in sizes)
                    {
                        using (MemoryStream imageStream = new MemoryStream())
                        {
                            scaleImage(size).Save(imageStream, System.Drawing.Imaging.ImageFormat.Png);
                            binaryWriter.Write(imageStream.ToArray());
                        }
                    }

                }
                return true;
            }
            return false;
        }
    }
}
