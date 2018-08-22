using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Media;
using System.Windows.Media.Imaging;

namespace TestApp
{
    public static class AuxWPFBinding
    {
        private static Dictionary<AuxiliaryLibraries.Media.PixelFormat, PixelFormat> AuxToWPFdic = new Dictionary<AuxiliaryLibraries.Media.PixelFormat, PixelFormat>()
        {
            { AuxiliaryLibraries.Media.PixelFormats.Indexed4, PixelFormats.Indexed4 },
            { AuxiliaryLibraries.Media.PixelFormats.Indexed8, PixelFormats.Indexed8 },
            { AuxiliaryLibraries.Media.PixelFormats.Bgra32,   PixelFormats.Bgra32   },
        };

        private static Dictionary<AuxiliaryLibraries.Media.PixelFormat, AuxiliaryLibraries.Media.PixelFormat> AuxToWPFCompdic = new Dictionary<AuxiliaryLibraries.Media.PixelFormat, AuxiliaryLibraries.Media.PixelFormat>()
        {
            { AuxiliaryLibraries.Media.PixelFormats.Indexed4Reverse, AuxiliaryLibraries.Media.PixelFormats.Indexed4 }
        };

        private static Dictionary<PixelFormat, AuxiliaryLibraries.Media.PixelFormat> WPFToAuxdic = new Dictionary<PixelFormat, AuxiliaryLibraries.Media.PixelFormat>()
        {
            { PixelFormats.Indexed4, AuxiliaryLibraries.Media.PixelFormats.Indexed4 },
            { PixelFormats.Indexed8, AuxiliaryLibraries.Media.PixelFormats.Indexed8 },
            { PixelFormats.Bgra32,   AuxiliaryLibraries.Media.PixelFormats.Bgra32   }
        };

        public static PixelFormat AuxToWPF(AuxiliaryLibraries.Media.PixelFormat pixelFormat)
        {
            if (AuxToWPFdic.ContainsKey(pixelFormat))
                return AuxToWPFdic[pixelFormat];
            else
                return PixelFormats.Default;
        }

        public static AuxiliaryLibraries.Media.PixelFormat AuxToWPFComp(AuxiliaryLibraries.Media.PixelFormat pixelFormat)
        {
            if (AuxToWPFCompdic.ContainsKey(pixelFormat))
                return AuxToWPFCompdic[pixelFormat];
            else
                return AuxiliaryLibraries.Media.PixelFormats.Undefined;
        }

        public static AuxiliaryLibraries.Media.PixelFormat WPFToAux(PixelFormat pixelFormat)
        {
            if (WPFToAuxdic.ContainsKey(pixelFormat))
                return WPFToAuxdic[pixelFormat];
            else
                return AuxiliaryLibraries.Media.PixelFormats.Undefined;
        }

        public static BitmapSource GetBitmapSource(this AuxiliaryLibraries.Media.Bitmap bitmap)
        {
            if (bitmap == null)
                throw new ArgumentNullException("bitmap");

            PixelFormat pix = AuxToWPF(bitmap.PixelFormat);
            if (pix != PixelFormats.Default)
            {
                Color[] colors = bitmap.CopyPalette()?.Select(x => Color.FromArgb(x.A, x.R, x.G, x.B)).ToArray();
                return BitmapSource.Create(bitmap.Width, bitmap.Height,
                    96, 96,
                    pix,
                    colors == null ? null : new BitmapPalette(colors),
                    bitmap.CopyData(),
                    AuxiliaryLibraries.Media.ImageHelper.GetStride(bitmap.PixelFormat, bitmap.Width));
            }
            else
            {
                AuxiliaryLibraries.Media.PixelFormat compPix = AuxToWPFComp(bitmap.PixelFormat);
                if (compPix != AuxiliaryLibraries.Media.PixelFormats.Undefined)
                    return bitmap.ConvertTo(compPix, null).GetBitmapSource();
            }
            return bitmap.ConvertTo(AuxiliaryLibraries.Media.PixelFormats.Bgra32, null).GetBitmapSource();
        }

        public static AuxiliaryLibraries.Media.Bitmap GetBitmap(this BitmapSource bitmapSource)
        {
            AuxiliaryLibraries.Media.PixelFormat pix = WPFToAux(bitmapSource.Format);
            if (pix != AuxiliaryLibraries.Media.PixelFormats.Undefined)
            {
                var colors = bitmapSource.Palette?.Colors.Select(x => System.Drawing.Color.FromArgb(x.A, x.R, x.G, x.B)).ToArray();
                return new AuxiliaryLibraries.Media.Bitmap(bitmapSource.PixelWidth, bitmapSource.PixelHeight,
                pix, bitmapSource.GetData(), colors);
            }
            return null;
        }

        public static byte[] GetData(this BitmapSource image)
        {
            var width = image.PixelWidth;
            var height = image.PixelHeight;
            var bitPerPixel = image.Format.BitsPerPixel;
            var stride = GetStride(image.Format, width);

            var LengthData = (height * width * bitPerPixel) / 8;

            var returned = new byte[LengthData];
            image.CopyPixels(returned, stride, 0);

            return returned;
        }

        public static int GetStride(PixelFormat pixelFormat, int width)
        {
            return (pixelFormat.BitsPerPixel * width + 7) / 8;
        }

        public static void SaveToPNG(BitmapSource image, string path)
        {
            Directory.CreateDirectory(Path.GetDirectoryName(Path.GetFullPath(path)));
            using (FileStream FS = new FileStream(path, FileMode.Create))
            {
                PngBitmapEncoder PNGencoder = new PngBitmapEncoder();

                PNGencoder.Frames.Add(BitmapFrame.Create(image));
                PNGencoder.Save(FS);
            }
        }

        public static BitmapSource OpenPNG(string path)
        {
            return new BitmapImage(new Uri(Path.GetFullPath(path)));

            //using (FileStream FS = new FileStream(path, FileMode.Open))
            //{
            //    var returned = new BitmapImage(new Uri(Path.GetFullPath(path)));
            //    returned.BeginInit();
            //    returned.StreamSource = FS;
            //    returned.EndInit();

            //    return returned;
            //}
        }
    }
}