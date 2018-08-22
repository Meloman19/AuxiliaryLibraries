﻿using System.Runtime.InteropServices;

namespace AuxiliaryLibraries.GameFormat.Sprite
{
    struct TMXHeader
    {
        public int ID;
        public int FileSize;
        public uint MagicNumber;
        public int Padding;
        public byte PaletteCount;
        public TMXPixelFormatEnum PaletteFormat;
        public ushort Width;
        public ushort Height;
        public TMXPixelFormatEnum PixelFormat;
        public byte MipMapCount;
        public byte MipMapK;
        public byte MipMapL;
        public ushort WrapMode;
        public uint TextureID;
        public uint ClutID;
        [MarshalAs(UnmanagedType.ByValArray, SizeConst = 28)]
        public byte[] Comment;
    }
}