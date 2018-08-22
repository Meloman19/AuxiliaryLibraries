using System.Collections.Generic;

namespace AuxiliaryLibraries.GameFormat
{
    public interface IGameFile
    {
        FormatEnum Type { get; }
        List<ObjectContainer> SubFiles { get; }
        int GetSize();
        byte[] GetData();
    }
}