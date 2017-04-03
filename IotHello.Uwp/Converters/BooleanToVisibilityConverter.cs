using ManiaLabs.Portable.Base.ValueConverters;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Data;

namespace IotHello.Uwp.Converters
{
    public class DefaultToVisibilityConverter : DefaultConverter
    {
        public override object Convert(object value, Type targetType, object parameter)
        {
            return base.Convert<Visibility>(Visibility.Collapsed, Visibility.Visible, value, targetType, parameter);
        }
    }

    public class DefaultVisibleXaml: Platform.XamlValueConverter<DefaultToVisibilityConverter>
    {
    }

    public class DefaultToHiddenConverter : DefaultConverter
    {
        public override object Convert(object value, Type targetType, object parameter)
        {
            return base.Convert<Visibility>(Visibility.Visible, Visibility.Collapsed, value, targetType, parameter);
        }
    }

    public class DefaultHiddenXaml : Platform.XamlValueConverter<DefaultToHiddenConverter>
    {
    }

}
