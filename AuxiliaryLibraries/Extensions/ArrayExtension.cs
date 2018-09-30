﻿using System;
using System.Collections.Generic;
using System.Linq;

namespace AuxiliaryLibraries.Extensions
{
    public static class ArrayExtension
    {
        public static T[] Copy<T>(this T[] array)
        {
            T[] returned = new T[array.Length];
            array.CopyTo(returned, 0);
            return returned;
        }

        public static T[,] Copy<T>(this T[] array, int firstDimension)
        {
            T[,] returned = new T[firstDimension, (array.Length + firstDimension - 1) / firstDimension];
            Buffer.BlockCopy(array, 0, returned, 0, array.Length);
            return returned;
        }

        public static T[] Copy<T>(this T[,] array)
        {
            T[] returned = new T[array.Length];
            Buffer.BlockCopy(array, 0, returned, 0, array.Length);
            return returned;
        }

        public static T[] SubArray<T>(this T[] data, int index, int length)
        {
            T[] result = new T[length];
            Array.Copy(data, index, result, 0, length);
            return result;
        }

        public static List<T[]> SplitArray<T>(this T[] array, int[] pos)
        {
            List<T[]> returned = new List<T[]>();


            for (int i = 0; i < pos.Length - 1; i++)
            {
                T[] temp = new T[pos[i + 1] - pos[i]];
                Array.Copy(array, pos[i], temp, 0, temp.Length);
                returned.Add(temp);
            }

            T[] temp2 = new T[array.Length - pos.Last()];
            Array.Copy(array, pos.Last(), temp2, 0, temp2.Length);
            returned.Add(temp2);

            return returned;
        }

        public static IEnumerable<T[]> Split<T>(this T[] array, int[] pos)
        {
            for (int i = 0; i < pos.Length - 1; i++)
            {
                T[] temp = new T[pos[i + 1] - pos[i]];
                Array.Copy(array, pos[i], temp, 0, temp.Length);
                yield return temp;
            }

            T[] temp2 = new T[array.Length - pos.Last()];
            Array.Copy(array, pos.Last(), temp2, 0, temp2.Length);
            yield return temp2;
        }
    }
}