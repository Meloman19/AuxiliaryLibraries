using AuxiliaryLibraries.Media;
using System.Collections.Generic;

namespace AuxiliaryLibraries.GameFormat.Sprite
{
    public static class DDSHelper
    {
        static Dictionary<PixelFormatDDSAtlus, Media.Formats.DDS.DDSFourCC> DDSAtlusToDDS = new Dictionary<PixelFormatDDSAtlus, Media.Formats.DDS.DDSFourCC>()
        {
            { PixelFormatDDSAtlus.DXT1, Media.Formats.DDS.DDSFourCC.DXT1 },
            { PixelFormatDDSAtlus.DXT3, Media.Formats.DDS.DDSFourCC.DXT3 },
            { PixelFormatDDSAtlus.DXT5, Media.Formats.DDS.DDSFourCC.DXT5 }
        };

        static Dictionary<PixelFormatDDSAtlus, PixelFormat> DDSAtlusToAux = new Dictionary<PixelFormatDDSAtlus, PixelFormat>()
        {
            { PixelFormatDDSAtlus.Argb32, PixelFormats.Argb32 }
        };

        public static Media.Formats.DDS.DDSFourCC ConvertFromDDSAtlus(PixelFormatDDSAtlus nativePixelFormat)
        {
            if (DDSAtlusToDDS.ContainsKey(nativePixelFormat))
                return DDSAtlusToDDS[nativePixelFormat];
            else
                return Media.Formats.DDS.DDSFourCC.NONE;
        }

        public static PixelFormat DDSAtlusToPixelFormat(PixelFormatDDSAtlus nativePixelFormat)
        {
            if (DDSAtlusToAux.ContainsKey(nativePixelFormat))
                return DDSAtlusToAux[nativePixelFormat];
            else
                return PixelFormats.Undefined;
        }
    }
}