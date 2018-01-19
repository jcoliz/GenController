using System;
using Windows.UI.Xaml;
using Common.Converters;

namespace IotHello.Uwp.Converters
{
    public class DefaultToVisibilityConverter : DefaultConverter
    {
        public override object Convert(object value, Type targetType, object parameter)
        {
            return base.Convert<Visibility>(Visibility.Collapsed, Visibility.Visible, value, targetType, parameter);
        }
    }

    public class DefaultVisibleXaml: XamlValueConverter<DefaultToVisibilityConverter>
    {
    }

    public class DefaultToHiddenConverter : DefaultConverter
    {
        public override object Convert(object value, Type targetType, object parameter)
        {
            return base.Convert<Visibility>(Visibility.Visible, Visibility.Collapsed, value, targetType, parameter);
        }
    }

    public class DefaultHiddenXaml : XamlValueConverter<DefaultToHiddenConverter>
    {
    }
}
