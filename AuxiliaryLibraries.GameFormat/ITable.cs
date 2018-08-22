using System.Xml.Linq;

namespace AuxiliaryLibraries.GameFormat
{
    public interface ITable
    {
        XDocument GetTable();
        void SetTable(XDocument xDocument);
    }
}