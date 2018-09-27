﻿using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;

namespace AuxiliaryLibraries.Extensions
{
    public static class StreamExtension
    {
        public static void WriteString(this BinaryWriter BW, string String, int Length)
        {
            byte[] buffer = Encoding.ASCII.GetBytes(String);
            BW.Write(buffer);

            for (int i = 0; i < Length - String.Length; i++)
                BW.Write((byte)0);
        }

        public static int[] ReadInt32Array(this BinaryReader BR, int count)
        {
            int[] returned = new int[count];

            for (int i = 0; i < count; i++)
                returned[i] = BR.ReadInt32();

            return returned;
        }

        public static int[][] ReadInt32ArrayArray(this BinaryReader BR, int count, int stride)
        {
            int[][] returned = new int[count][];

            for (int i = 0; i < count; i++)
                returned[i] = BR.ReadInt32Array(stride);

            return returned;
        }

        public static void WriteInt32Array(this BinaryWriter BW, int[] array)
        {
            for (int i = 0; i < array.Length; i++)
                BW.Write(array[i]);
        }

        public static bool CheckEntrance(this Stream B, byte[] Bytes)
        {
            if (Bytes.Length != 0)
            {
                if (B.Position < B.Length)
                    if (B.ReadByte() == Bytes[0])
                        return B.CheckEntrance(Bytes.Skip(1).ToArray());

                return false;
            }
            else return true;
        }
    }
}