using Commonality.Converters;
using System;
using Windows.UI;
using Windows.UI.Xaml.Media;

namespace GenController.Uwp.Converters
{
    public class BooleanRedBrushConverter : DefaultConverter<Brush>
    {
        public BooleanRedBrushConverter(): base(new SolidColorBrush(Colors.Black),new SolidColorBrush(Colors.Red))
        {
        }
    }

    public class BooleanRedBrushConverterXaml : XamlValueConverter<BooleanRedBrushConverter>
    {
    }
}
