using System;
using System.IO;
using System.Windows.Media.Imaging;

namespace AuxiliaryLibraries.WPF.Tools
{
    public static class ImageTools
    {
        public static BitmapSource OpenPNG(string path) => new BitmapImage(new Uri(Path.GetFullPath(path)));

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
    }
}