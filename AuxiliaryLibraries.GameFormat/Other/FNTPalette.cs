using System.Collections.Generic;
using System.Drawing;
using System.IO;

namespace AuxiliaryLibraries.GameFormat.Other
{
    public class FNTPalette
    {
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

        public FNTPalette(BinaryReader reader, int NumberOfColor)
        {
            Pallete = new Color[NumberOfColor];
            for (int i = 0; i < NumberOfColor; i++)
            {
                byte R = reader.ReadByte();
                byte G = reader.ReadByte();
                byte B = reader.ReadByte();
                byte A = AlphaPS2ToPC[reader.ReadByte()];
                Pallete[i] = Color.FromArgb(A, R, G, B);
            }
        }

        public Color[] Pallete { get; set; }

        public int Size
        {
            get { return Pallete.Length * 4; }
        }

        public void Get(BinaryWriter writer)
        {
            foreach (var color in Pallete)
            {
                writer.Write(color.R);
                writer.Write(color.G);
                writer.Write(color.B);
                writer.Write(color.A);
            }
        }

        public Color[] GetImagePalette()
        {
            List<Color> palette = new List<Color>();
            foreach (var color in Pallete)
                palette.Add(Color.FromArgb(0xFF, color));

            return palette.ToArray();
        }
    }
}