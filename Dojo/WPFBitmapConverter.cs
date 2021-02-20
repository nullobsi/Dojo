using System;
using System.Drawing;
using System.Drawing.Imaging;
using System.Globalization;
using System.IO;
using System.Windows.Data;
using System.Windows.Media.Imaging;

namespace Dojo
{
  public class WPFBitmapConverter : IValueConverter
  {
    public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
    {
      MemoryStream memoryStream = new MemoryStream();
      ((Image) value).Save((Stream) memoryStream, ImageFormat.Png);
      BitmapImage bitmapImage = new BitmapImage();
      bitmapImage.BeginInit();
      memoryStream.Seek(0L, SeekOrigin.Begin);
      bitmapImage.StreamSource = (Stream) memoryStream;
      bitmapImage.EndInit();
      return (object) bitmapImage;
    }

    public object ConvertBack(
      object value,
      Type targetType,
      object parameter,
      CultureInfo culture)
    {
      throw new NotImplementedException();
    }
  }
}
