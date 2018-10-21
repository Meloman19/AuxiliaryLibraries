using AuxiliaryLibraries.GameFormat.Other;
using AuxiliaryLibraries.GameFormat.Text;
using AuxiliaryLibraries.WPF;
using AuxiliaryLibraries.WPF.Extensions;
using AuxiliaryLibraries.WPF.Wrapper;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Windows;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Xml.Linq;

namespace TestApp
{
    class MainWindowsVM : BindingObject
    {
        public MainWindowsVM()
        {
            CommandAct = new RelayCommand(command);

            BMD bMD = new BMD(File.ReadAllBytes(@"d:\PS2\Other\DATA EDIT\Text\1.BMD"));
            File.WriteAllBytes(@"d:\PS2\Other\DATA EDIT\Text\2.BMD", bMD.GetData());

            //test();
        }

        private void test()
        {
            string path = @"d:\PS2\Persona 3\DATA\";

            foreach (var file in Directory.EnumerateFiles(path, "*.*", SearchOption.AllDirectories))
            {
                var a = AuxiliaryLibraries.GameFormat.GameFormatHelper.OpenFile(Path.GetFileName(file), File.ReadAllBytes(file));
                if (a.Object != null)
                {
                    //var b = a.GetAllObjectFiles(AuxiliaryLibraries.GameFormat.FormatEnum.BMD);

                    //foreach(var bmd in b)
                    //{
                    //    var current = bmd.Object as BMD;

                    //    foreach(var msg in current.Msg)
                    //        foreach(var str in msg.M)
                    //}
                }
            }
        }

        public ICommand CommandAct { get; }

        public void command(object arg)
        {
            var bitmap = AuxiliaryLibraries.WPF.Tools.ImageTools.OpenPNG(@"d:\Visual Studio 2017\Project\PersonaEditor\PersonaEditorGUI\Resource\settings-work-tool.png");
            Image = bitmap;
            ImageRect = new Rect(0, 0, bitmap.PixelWidth, bitmap.PixelHeight);

            Notify("Image");
            Notify("ImageRect");
        }

        public BitmapSource Image { get; set; } = null;
        public Rect ImageRect { get; set; }

        public Rect LeftRect { get; set; } = new Rect(10, 10, 100, 100);
    }
}