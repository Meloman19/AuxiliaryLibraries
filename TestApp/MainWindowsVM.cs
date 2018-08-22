using AuxiliaryLibraries.WPF;
using System.Windows.Input;

namespace TestApp
{
    class MainWindowsVM : BindingObject
    {
        public MainWindowsVM()
        {
            CommandAct = new RelayCommand(command);
            ButtonText = "Click";


        }

        public ICommand CommandAct { get; }

        public string ButtonText { get; set; }

        public object NewDC
        {
            get { return this; }
        }

        public void command(object arg)
        {
            ButtonText = ((System.Windows.Point)arg).ToString();
            Notify("ButtonText");
        }
    }
}