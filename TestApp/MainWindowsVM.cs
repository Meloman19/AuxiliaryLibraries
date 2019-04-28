using AuxiliaryLibraries.WPF;
using System;
using System.Collections;
using System.IO;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Input;
using System.Windows.Media.Imaging;
using System.Windows.Threading;

namespace TestApp
{
    class MainWindowsVM : BindingObject
    {
        private Dispatcher temp;

        private string log = "";
        public string Log
        {
            get { return log; }
            set
            {
                log = value;
                Notify(nameof(Log));
            }
        }

        public MainWindowsVM()
        {
            CommandAct = new RelayCommand(command);
            temp = Dispatcher.CurrentDispatcher;
            test();
        }

        private void test()
        {
        }

        public ICommand CommandAct { get; }

        public async void command(object arg)
        {
            await Task.Run((Action)test);
        }

        public BitmapSource Image { get; set; } = null;
        public Rect ImageRect { get; set; }

        public Rect LeftRect { get; set; } = new Rect(10, 10, 100, 100);
    }
}