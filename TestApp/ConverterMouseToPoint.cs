using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Input;

namespace TestApp
{
    public class ConverterMouseToPoint : AuxiliaryLibraries.WPF.Interactivity.ICommandArgConverter
    {
        public object GetArguments(object[] args)
        {
            return (args[1] as MouseEventArgs).GetPosition(args[0] as IInputElement);
            
        }
    }
}
