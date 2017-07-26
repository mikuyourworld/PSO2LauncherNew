using System.IO;
using System.Windows.Media.Imaging;

namespace Leayal.Ugoira.WPF
{
    internal static class QuickMethods
    {
        internal static BitmapImage CreateBitmapImage(Stream _stream)
        {
            return CreateBitmapImage(_stream, false);
        }
        internal static BitmapImage CreateBitmapImage(Stream _stream, bool leaveStreamOpen)
        {
            BitmapImage result = new BitmapImage();
            result.BeginInit();
            result.CreateOptions = BitmapCreateOptions.PreservePixelFormat;
            result.CacheOption = BitmapCacheOption.OnLoad;
            result.StreamSource = _stream;
            result.EndInit();
            if (result.CanFreeze)
                result.Freeze();
            if (!leaveStreamOpen)
                _stream.Dispose();
            return result;
        }
    }
}
