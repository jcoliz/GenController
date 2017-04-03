using ManiaLabs.Portable.Base.ValueConverters;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Windows.UI;
using Windows.UI.Xaml.Data;
using Windows.UI.Xaml.Media;

namespace IotHello.Uwp.Converters
{
    public class BooleanRedBrushConverter : DefaultConverter
    {
        SolidColorBrush Red = new SolidColorBrush(Colors.Red);
        SolidColorBrush Black = new SolidColorBrush(Colors.Black);
        public override object Convert(object value, Type targetType, object parameter)
        {
            return base.Convert<Brush>(Black, Red, value, targetType, parameter);
        }
    }

    public class BooleanRedBrushConverterXaml : Platform.XamlValueConverter<BooleanRedBrushConverter>
    {
    }
}
