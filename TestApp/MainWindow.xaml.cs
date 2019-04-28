using AuxiliaryLibraries.Extensions;
using AuxiliaryLibraries.Media;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Windows;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using AuxiliaryLibraries.WPF.Wrapper;

namespace TestApp
{
    /// <summary>
    /// Логика взаимодействия для MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        public MainWindow()
        {
            InitializeComponent();

            //byte[] temp = new byte[10000000];
            //for (int i = 0; i < temp.Length; i++)
            //    temp[i] = 0xFF;
            //byte[,] temp2 = temp.Copy(10);

            //List<string> result = new List<string>();
            //System.Diagnostics.Stopwatch stopwatch = new System.Diagnostics.Stopwatch();

            //for (int i = 0; i < 5; i++)
            //{
            //    GC.Collect();
            //    System.Threading.Thread.Sleep(1000);
            //    string ret = "";

            //    stopwatch.Start();
            //    //var result1 = DataToDataConverter.Indexed4ToIndexed8(temp);
            //    stopwatch.Stop();
            //    ret += stopwatch.Elapsed.TotalMilliseconds.ToString() + "\t";
            //    stopwatch.Reset();

            //    stopwatch.Start();
            //    //var result2 = DataToDataConverter.Indexed4ToIndexed8_2(temp2, 1000000);
            //    stopwatch.Stop();
            //    ret += stopwatch.Elapsed.TotalMilliseconds.ToString() + "\t";
            //    stopwatch.Reset();

            //    result.Add(ret);
            //}

            //File.WriteAllLines("D:\\res.txt", result);

            //string result = "{" + Environment.NewLine;
            //for (int a = 0; a < 256; a++)
            //{
            //    byte a5 = ConvertAlphaToPS2((byte)a);

            //    result += a5.ToString().PadLeft(4, ' ');
            //    result += ",";
            //    if ((a + 1) % 16 == 0)
            //        result += Environment.NewLine;
            //}
            //result += "}";
            //File.WriteAllLines("D:\\bit.txt", new string[] { result });

        }

        private void Window_Drop(object sender, DragEventArgs e)
        {
           
        }
    }
}
