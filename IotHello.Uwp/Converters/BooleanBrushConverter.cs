using Common.Converters;
using System;
using Windows.UI;
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

    public class BooleanRedBrushConverterXaml : XamlValueConverter<BooleanRedBrushConverter>
    {
    }
}
