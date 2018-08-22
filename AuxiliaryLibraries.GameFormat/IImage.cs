using AuxiliaryLibraries.Media;

namespace AuxiliaryLibraries.GameFormat
{
    public interface IImage
    {
        Bitmap GetBitmap();
        void SetBitmap(Bitmap bitmap);
    }
}