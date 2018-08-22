﻿using AuxiliaryLibraries.Extension;
using System.Collections.Generic;
using System.IO;
using System.Linq;

namespace AuxiliaryLibraries.GameFormat
{
    public static class GameFormatHelper
    {
        public static Dictionary<string, FormatEnum> FileTypeDic = new Dictionary<string, FormatEnum>()
        {
            //Containers
            { ".bin", FormatEnum.BIN },
            { ".pak",  FormatEnum.BIN },
            { ".pac",  FormatEnum.BIN },
            { ".p00",  FormatEnum.BIN },
            { ".p01",  FormatEnum.BIN },
            { ".arc",  FormatEnum.BIN },
            { ".dds2", FormatEnum.BIN },

            { ".bf",  FormatEnum.BF  },
            { ".pm1", FormatEnum.PM1 },
            { ".bvp", FormatEnum.BVP },
            { ".tbl", FormatEnum.TBL },

            { ".ctd", FormatEnum.FTD },
            { ".ftd", FormatEnum.FTD },
            { ".ttd", FormatEnum.FTD },

            //Graphic containers
            { ".spr", FormatEnum.SPR },
            { ".spd", FormatEnum.SPD },

            //Graphic
            { ".fnt", FormatEnum.FNT },
            { ".tmx", FormatEnum.TMX },
            { ".dds", FormatEnum.DDS },

            //Text
            { ".bmd", FormatEnum.BMD },
            { ".msg", FormatEnum.BMD },
            { ".ptp", FormatEnum.PTP }
        };

        public static ObjectContainer OpenFile(string name, byte[] data, FormatEnum type)
        {
            try
            {
                object Obj;

                if (type == FormatEnum.BIN)
                    Obj = new FileContainer.BIN(data);
                else if (type == FormatEnum.SPR)
                    Obj = new SpriteContainer.SPR(data);
                else if (type == FormatEnum.TMX)
                    Obj = new Sprite.TMX(data);
                else if (type == FormatEnum.BF)
                    Obj = new FileContainer.BF(data, name);
                else if (type == FormatEnum.PM1)
                    Obj = new FileContainer.PM1(data);
                else if (type == FormatEnum.BMD)
                    Obj = new Text.BMD(data);
                else if (type == FormatEnum.PTP)
                    Obj = new Text.PTP(data);
                else if (type == FormatEnum.FNT)
                    Obj = new Other.FNT.FNT(data);
                else if (type == FormatEnum.BVP)
                    Obj = new FileContainer.BVP(name, data);
                else if (type == FormatEnum.TBL)
                    try
                    {
                        Obj = new FileContainer.TBL(data, name);
                    }
                    catch
                    {
                        Obj = new FileContainer.BIN(data);
                    }
                else if (type == FormatEnum.FTD)
                    Obj = new Text.FTD(data);
                else if (type == FormatEnum.DDS)
                    try
                    {
                        Obj = new Sprite.DDS(data);
                    }
                    catch
                    {
                        Obj = new Sprite.DDSAtlus(data);
                    }
                else if (type == FormatEnum.SPD)
                    Obj = new SpriteContainer.SPD(data);
                else
                    Obj = new Other.DAT(data);

                return new ObjectContainer(name, Obj);
            }
            catch
            {
                return new ObjectContainer(name, null);
            }
        }

        public static FormatEnum GetFormat(string name)
        {
            string ext = Path.GetExtension(name).ToLower().TrimEnd(' ');
            if (FileTypeDic.ContainsKey(ext))
                return FileTypeDic[ext];
            else
                return FormatEnum.DAT;
        }

        public static FormatEnum GetFormat(byte[] data)
        {
            if (data.Length >= 0xc)
            {
                byte[] buffer = data.SubArray(8, 4);
                if (buffer.SequenceEqual(new byte[] { 0x31, 0x47, 0x53, 0x4D }) | buffer.SequenceEqual(new byte[] { 0x4D, 0x53, 0x47, 0x31 }))
                    return FormatEnum.BMD;
                else if (buffer.SequenceEqual(new byte[] { 0x54, 0x4D, 0x58, 0x30 }))
                    return FormatEnum.TMX;
                else if (buffer.SequenceEqual(new byte[] { 0x53, 0x50, 0x52, 0x30 }))
                    return FormatEnum.SPR;
                else if (buffer.SequenceEqual(new byte[] { 0x46, 0x4C, 0x57, 0x30 }))
                    return FormatEnum.BF;
                else if (buffer.SequenceEqual(new byte[] { 0x50, 0x4D, 0x44, 0x31 }))
                    return FormatEnum.PM1;
            }
            return FormatEnum.Unknown;
        }
    }
}