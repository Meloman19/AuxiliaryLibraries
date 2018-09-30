using AuxiliaryLibraries.Media;
using System.Collections.Generic;

namespace AuxiliaryLibraries.GameFormat.Sprite
{
    public static class DDSHelper
    {
        static Dictionary<DDSAtlusPixelFormat, Media.Formats.DDS.DDSFourCC> DDSAtlusToDDS = new Dictionary<DDSAtlusPixelFormat, Media.Formats.DDS.DDSFourCC>()
        {
            { DDSAtlusPixelFormat.DXT1, Media.Formats.DDS.DDSFourCC.DXT1 },
            { DDSAtlusPixelFormat.DXT3, Media.Formats.DDS.DDSFourCC.DXT3 },
            { DDSAtlusPixelFormat.DXT5, Media.Formats.DDS.DDSFourCC.DXT5 }
        };

        static Dictionary<DDSAtlusPixelFormat, PixelFormat> DDSAtlusToAux = new Dictionary<DDSAtlusPixelFormat, PixelFormat>()
        {
            { DDSAtlusPixelFormat.Argb32, PixelFormats.Argb32 }
        };

        public static Media.Formats.DDS.DDSFourCC ConvertFromDDSAtlus(DDSAtlusPixelFormat nativePixelFormat)
        {
            if (DDSAtlusToDDS.ContainsKey(nativePixelFormat))
                return DDSAtlusToDDS[nativePixelFormat];
            else
                return Media.Formats.DDS.DDSFourCC.NONE;
        }

        public static PixelFormat DDSAtlusToPixelFormat(DDSAtlusPixelFormat nativePixelFormat)
        {
            if (DDSAtlusToAux.ContainsKey(nativePixelFormat))
                return DDSAtlusToAux[nativePixelFormat];
            else
                return PixelFormats.Undefined;
        }
    }
}