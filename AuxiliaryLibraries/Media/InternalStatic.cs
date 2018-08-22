﻿using System;
using System.Collections.Generic;
using System.Drawing;
using System.Text;

namespace AuxiliaryLibraries.Media
{
    internal static class PixelFormatHelper
    {
        internal static int GetPixelFormatBPP(PixelFormatEnum pixelFormatEnum)
        {
            switch (pixelFormatEnum)
            {
                case (PixelFormatEnum.Indexed4):
                case (PixelFormatEnum.Indexed4Reverse):
                    return 4;
                case (PixelFormatEnum.Indexed8):
                    return 8;
                case (PixelFormatEnum.Bgra32):
                case (PixelFormatEnum.Rgba32):
                case (PixelFormatEnum.Rgba32PS2):
                case (PixelFormatEnum.Argb32):
                    return 32;
                default:
                    return 0;
            }
        }

        internal static bool GetPixelFormatIndexed(PixelFormatEnum pixelFormatEnum)
        {
            switch (pixelFormatEnum)
            {
                case (PixelFormatEnum.Indexed4):
                case (PixelFormatEnum.Indexed4Reverse):
                case (PixelFormatEnum.Indexed8):
                    return true;
                default:
                    return false;
            }
        }

        internal static byte ReverseByte(byte toReverse)
        {
            int temp = ((toReverse >> 4) & 0xF) + ((toReverse & 0xF) << 4);
            return Convert.ToByte(temp);
        }
    }

    internal static class BitHelper
    {
        // AlphaPS2ToPC
        // static byte ConvertAlphaToPC(byte original)
        // {
        //     if ((int)original - 0x80 <= 0)
        //         return (byte)Math.Round((((float)original / 0x80) * 0xFF));
        //     else
        //         return (byte)(0xFF - ((((float)original - 0x80) / 0x80) * 0xFF)); //wrap around
        // }
        internal static byte[] AlphaPS2ToPC = new byte[]
        {
              0,   2,   4,   6,   8,  10,  12,  14,  16,  18,  20,  22,  24,  26,  28,  30,
             32,  34,  36,  38,  40,  42,  44,  46,  48,  50,  52,  54,  56,  58,  60,  62,
             64,  66,  68,  70,  72,  74,  76,  78,  80,  82,  84,  86,  88,  90,  92,  94,
             96,  98, 100, 102, 104, 106, 108, 110, 112, 114, 116, 118, 120, 122, 124, 126,
            128, 129, 131, 133, 135, 137, 139, 141, 143, 145, 147, 149, 151, 153, 155, 157,
            159, 161, 163, 165, 167, 169, 171, 173, 175, 177, 179, 181, 183, 185, 187, 189,
            191, 193, 195, 197, 199, 201, 203, 205, 207, 209, 211, 213, 215, 217, 219, 221,
            223, 225, 227, 229, 231, 233, 235, 237, 239, 241, 243, 245, 247, 249, 251, 253,
            255, 253, 251, 249, 247, 245, 243, 241, 239, 237, 235, 233, 231, 229, 227, 225,
            223, 221, 219, 217, 215, 213, 211, 209, 207, 205, 203, 201, 199, 197, 195, 193,
            191, 189, 187, 185, 183, 181, 179, 177, 175, 173, 171, 169, 167, 165, 163, 161,
            159, 157, 155, 153, 151, 149, 147, 145, 143, 141, 139, 137, 135, 133, 131, 129,
            127, 125, 123, 121, 119, 117, 115, 113, 111, 109, 107, 105, 103, 101,  99,  97,
             95,  93,  91,  89,  87,  85,  83,  81,  79,  77,  75,  73,  71,  69,  67,  65,
             63,  61,  59,  57,  55,  53,  51,  49,  47,  45,  43,  41,  39,  37,  35,  33,
             31,  29,  27,  25,  23,  21,  19,  17,  15,  13,  11,   9,   7,   5,   3,   1
        };

        // AlphaPCToPS2
        // static byte ConvertAlphaToPS2(byte original)
        // {
        //     return (byte)Math.Round((((float)original / 0xFF) * 0x80));
        // }
        internal static byte[] AlphaPCToPS2 = new byte[]
        {
              0,   1,   1,   2,   2,   3,   3,   4,   4,   5,   5,   6,   6,   7,   7,   8,
              8,   9,   9,  10,  10,  11,  11,  12,  12,  13,  13,  14,  14,  15,  15,  16,
             16,  17,  17,  18,  18,  19,  19,  20,  20,  21,  21,  22,  22,  23,  23,  24,
             24,  25,  25,  26,  26,  27,  27,  28,  28,  29,  29,  30,  30,  31,  31,  32,
             32,  33,  33,  34,  34,  35,  35,  36,  36,  37,  37,  38,  38,  39,  39,  40,
             40,  41,  41,  42,  42,  43,  43,  44,  44,  45,  45,  46,  46,  47,  47,  48,
             48,  49,  49,  50,  50,  51,  51,  52,  52,  53,  53,  54,  54,  55,  55,  56,
             56,  57,  57,  58,  58,  59,  59,  60,  60,  61,  61,  62,  62,  63,  63,  64,
             64,  65,  65,  66,  66,  67,  67,  68,  68,  69,  69,  70,  70,  71,  71,  72,
             72,  73,  73,  74,  74,  75,  75,  76,  76,  77,  77,  78,  78,  79,  79,  80,
             80,  81,  81,  82,  82,  83,  83,  84,  84,  85,  85,  86,  86,  87,  87,  88,
             88,  89,  89,  90,  90,  91,  91,  92,  92,  93,  93,  94,  94,  95,  95,  96,
             96,  97,  97,  98,  98,  99,  99, 100, 100, 101, 101, 102, 102, 103, 103, 104,
            104, 105, 105, 106, 106, 107, 107, 108, 108, 109, 109, 110, 110, 111, 111, 112,
            112, 113, 113, 114, 114, 115, 115, 116, 116, 117, 117, 118, 118, 119, 119, 120,
            120, 121, 121, 122, 122, 123, 123, 124, 124, 125, 125, 126, 126, 127, 127, 128,
        };
    }
}