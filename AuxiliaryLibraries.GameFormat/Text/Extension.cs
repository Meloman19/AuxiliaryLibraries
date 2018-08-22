﻿using AuxiliaryLibraries.Extension;
using AuxiliaryLibraries.GameFormat.Other.FNT;
using AuxiliaryLibraries.Tool;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;

namespace AuxiliaryLibraries.GameFormat.Text
{
    public static class Extension
    {
        public static List<TextBaseElement> GetTextBaseList(this string String, Encoding FontMap)
        {
            List<TextBaseElement> MyByteArrayList = new List<TextBaseElement>();

            foreach (var a in Regex.Split(String, "(\r\n|\r|\n)"))
                if (Regex.IsMatch(a, "\r\n|\r|\n"))
                    MyByteArrayList.Add(new TextBaseElement(false, new byte[] { 0x0A }));
                else
                    foreach (var b in Regex.Split(a, @"({[^}]+})"))
                        if (Regex.IsMatch(b, @"{.+}"))
                            MyByteArrayList.Add(new TextBaseElement(false, StringTool.SplitString(b.Substring(1, b.Length - 2), ' ')));
                        else
                            MyByteArrayList.Add(new TextBaseElement(true, FontMap.GetBytes(b)));

            return MyByteArrayList;
        }

        public static List<TextBaseElement> GetTextBaseList(this byte[] array)
        {
            List<TextBaseElement> returned = new List<TextBaseElement>();

            bool isText = true;
            List<byte> temp = new List<byte>();

            for (int i = 0; i < array.Length; i++)
            {
                if (0x20 <= array[i] & array[i] < 0x80)
                {
                    temp.Add(array[i]);
                }
                else if (0x80 <= array[i] & array[i] < 0xF0)
                {
                    temp.Add(array[i]);
                    i = i + 1;
                    temp.Add(array[i]);
                }
                else
                {
                    if (0x00 <= array[i] & array[i] < 0x20)
                    {
                        if (temp.Count != 0)
                        {
                            returned.Add(new TextBaseElement(isText, temp.ToArray()));
                            temp.Clear();
                        }

                        isText = false;
                        temp.Add(array[i]);

                        returned.Add(new TextBaseElement(isText, temp.ToArray()));
                        isText = true;
                        temp.Clear();
                    }
                    else
                    {
                        if (temp.Count != 0)
                        {
                            returned.Add(new TextBaseElement(isText, temp.ToArray()));
                            //type = "Text";
                            isText = true;
                            temp.Clear();
                        }

                        isText = false;
                        temp.Add(array[i]);
                        int count = (array[i] - 0xF0) * 2 - 1;
                        for (int k = 0; k < count; k++)
                        {
                            i++;
                            temp.Add(array[i]);
                        }

                        returned.Add(new TextBaseElement(isText, temp.ToArray()));
                        isText = true;
                        temp.Clear();
                    }
                }
            }

            if (temp.Count != 0)
            {
                returned.Add(new TextBaseElement(isText, temp.ToArray()));
                temp.Clear();
            }


            return returned;
        }

        public static List<string> SplitBySystem(this string str)
        {
            List<string> returned = new List<string>();

            foreach (var a in Regex.Split(str, "(\r\n|\r|\n)"))
                if (Regex.IsMatch(a, "\r\n|\r|\n"))
                    returned.Add("{0A}");
                else
                    foreach (var b in Regex.Split(a, @"({[^}]+})"))
                        returned.Add(b);

            return returned;
        }

        public static void ParseStrings(this IList<MSGstr> Strings, byte[] SourceBytes)
        {
            Strings.Clear();

            int Index = 0;
            foreach (var Bytes in SplitSourceBytes(SourceBytes))
            {
                MSGstr MSG = new MSGstr(Index, "");

                List<TextBaseElement> temp = Bytes.GetTextBaseList();

                int tempdown = 0;
                int temptop = temp.Count;

                for (int i = 0; i < temp.Count; i++)
                {
                    if (temp[i].IsText)
                    {
                        tempdown = i;
                        i = temp.Count;
                    }
                    else
                        MSG.Prefix.Add(temp[i]);
                }

                if (MSG.Prefix.Count < temp.Count)
                {
                    for (int i = temp.Count - 1; i >= tempdown; i--)
                    {
                        if (temp[i].IsText)
                        {
                            temptop = i;
                            i = 0;
                        }
                        else
                            MSG.Postfix.Add(temp[i]);
                    }

                    var temparray = MSG.Postfix.Reverse().ToList();

                    MSG.Postfix.Clear();
                    foreach (var a in temparray)
                        MSG.Postfix.Add(a);


                    for (int i = tempdown; i <= temptop; i++)
                        MSG.OldString.Add(temp[i]);
                }

                Strings.Add(MSG);
                Index++;
            }
        }

        public static string SplitByWidth(this string String, Encoding FontMap, PersonaFont Font, int width)
        {
            string returned = String.Join(" ", Regex.Split(String, @"\\n|\r\n|\r|\n"));

            List<TextBaseElement> temp = returned.GetTextBaseList(FontMap);
            List<int> widthlist = new List<int>();

            foreach (var a in temp)
            {
                if (a.IsText)
                    for (int i = 0; i < a.Array.Length; i++)
                    {
                        VerticalCut verticalCut = new VerticalCut();
                        if (a.Array[i] == 0x20)
                        {
                            widthlist.Add(9);
                            continue;
                        }
                        else if (0x20 < a.Array[i] & a.Array[i] < 0x80)
                            verticalCut = Font.GetVerticalCut(a.Array[i]);
                        else if (0x80 <= a.Array[i] & a.Array[i] < 0xF0)
                        {
                            int newindex = (a.Array[i] - 0x81) * 0x80 + a.Array[i + 1] + 0x20;
                            i++;
                            verticalCut = Font.GetVerticalCut(newindex);
                        }

                        if (verticalCut.Right - verticalCut.Left > 0)
                            widthlist.Add(verticalCut.Right - verticalCut.Left - 1);
                        else
                            widthlist.Add(verticalCut.Right - verticalCut.Left);
                    }
                else
                    widthlist.AddRange(new int[a.GetSystem().Length]);
            }

            int index = 0;
            int widthsum = 0;
            while (index < widthlist.Count)
            {
                if (widthsum + widthlist[index] <= width)
                {
                    widthsum += widthlist[index];
                    index++;
                }
                else
                {
                    bool te = true;
                    while (index != 0 & te)
                    {
                        if (widthlist[index - 1] != 0 & returned[index - 1] == ' ')
                        {
                            returned = returned.Insert(index, "\n");
                            widthlist.Insert(index, 0);
                            te = false;
                        }
                        index--;
                    }
                    widthsum = 0;
                }
            }

            return returned;
        }

        public static string SplitByLineCount(this string String, Dictionary<char, int> charWidth, int lineCount)
        {
            string input = String.Join(" ", Regex.Split(String, @"\r\n|\r|\n"));

            List<string> tempStr = new List<string>();
            List<bool> tempBool = new List<bool>();
            List<int> tempWidth = new List<int>();

            var split = input.SplitBySystem();
            foreach (var a in split)
            {
                try
                {
                    StringTool.SplitString(a.Substring(1, a.Length - 2), ' ');
                    tempStr.Add(a);
                    tempBool.Add(false);
                }
                catch
                {
                    foreach (var b in Regex.Split(a, @"( )").Where(x => x != ""))
                    {
                        tempStr.Add(b);
                        tempBool.Add(true);
                    }
                }
            }

            for (int i = 0; i < tempStr.Count; i++)
            {
                if (tempBool[i])
                {
                    int temp = 0;
                    foreach (var a in tempStr[i])
                        if (charWidth.ContainsKey(a))
                            temp += charWidth[a];

                    tempWidth.Add(temp);
                }
                else
                {
                    if (tempStr[i].Equals("{F1 81}") | tempStr[i].Equals("{F1 82}") | tempStr[i].Equals("{F1 83}"))
                        tempWidth.Add(10);
                    else
                        tempWidth.Add(0);
                }
            }

            List<int> indexies = new List<int>();
            List<string> returned = new List<string>();
            int width = tempWidth.Sum() / lineCount;
            int tempwidth = 0;
            int tempind = 0;
            for (int i = 0; i < tempWidth.Count; i++)
            {
                if (tempwidth + tempWidth[i] > width)
                {
                    indexies.Add(tempind);
                    tempind = i + 1;
                    tempwidth = 0;
                    if (indexies.Count == lineCount)
                        break;
                }
                else
                    tempwidth += tempWidth[i];
            }
            if (indexies.Count != lineCount)
                indexies.Add(tempind);

            var splitedByLineCount = String.Join("\n", tempStr.ToArray().SplitArray(indexies.ToArray()).Select(x => String.Join("", x)).Select(x => x.TrimStart(' ')));

            return splitedByLineCount;
        }

        public static List<byte[]> SplitSourceBytes(this byte[] B)
        {
            List<byte[]> returned = new List<byte[]>();

            if (B.Length == 0)
                return returned;

            byte[] LineSplit = B.ToArray().Take((B[0] - 0xF0) * 2).ToArray();

            List<byte> String = new List<byte>();
            for (int i = 0; i < B.Length; i++)
            {
                if (B.CheckEntrance(LineSplit, i))
                {
                    if (String.Count != 0)
                    {
                        returned.Add(String.ToArray());
                        String.Clear();
                    }
                }

                String.Add(B[i]);
            }

            if (String.Count != 0)
            {
                returned.Add(String.ToArray());
                String.Clear();
            }

            return returned;
        }

        public static string MSGListToSystem(this IList<TextBaseElement> list)
        {
            string returned = "";
            foreach (var Bytes in list)
            {
                byte[] temp = Bytes.Array.ToArray();
                if (temp.Length > 0)
                {
                    returned += "{" + System.Convert.ToString(temp[0], 16).PadLeft(2, '0').ToUpper();
                    for (int i = 1; i < temp.Length; i++)
                    {
                        returned += "\u00A0" + System.Convert.ToString(temp[i], 16).PadLeft(2, '0').ToUpper();
                    }
                    returned += "} ";
                }
            }
            return returned;
        }

        public static string GetString(this IList<TextBaseElement> ByteCollection, Encoding encoding, bool LineSplit = false)
        {
            string returned = "";

            foreach (var MSG in ByteCollection)
                returned += MSG.GetText(encoding, LineSplit);

            return returned;
        }

        public static string GetString(this IList<TextBaseElement> ByteCollection)
        {
            string returned = "";

            foreach (var MSG in ByteCollection)
                returned += MSG.GetSystem();

            return returned;
        }

        public static byte[] GetByteArray(this IList<TextBaseElement> ByteCollection)
        {
            List<byte> temp = new List<byte>();
            foreach (var a in ByteCollection)
                temp.AddRange(a.Array.ToArray());
            return temp.ToArray();
        }
    }
}