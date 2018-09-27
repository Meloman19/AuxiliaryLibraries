using AuxiliaryLibraries.WPF;
using System.IO;
using System.Windows;
using System.Windows.Input;
using System.Windows.Media.Imaging;

namespace TestApp
{
    class MainWindowsVM : BindingObject
    {
        public MainWindowsVM()
        {
            CommandAct = new RelayCommand(command);

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