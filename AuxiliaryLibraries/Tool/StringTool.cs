using System;
using System.Linq;

namespace AuxiliaryLibraries.Tool
{
    public static class StringTool
    {
        public static byte[] SplitString(string str, char del)
        {
            string[] temp = str.Split(del);
            return Enumerable.Range(0, temp.Length).Select(x => Convert.ToByte(temp[x], 16)).ToArray();
        }
    }
}