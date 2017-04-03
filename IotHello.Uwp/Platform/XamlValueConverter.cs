using ManiaLabs.Portable.Base.ValueConverters;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Windows.UI.Xaml.Data;

namespace IotHello.Uwp.Platform
{
    /// <summary>
    /// Generic class to convert Portable.Base valueconverters into Xaml-specific valueconverters
    /// </summary>
    /// <typeparam name="T"></typeparam>
    public class XamlValueConverter<T> : IValueConverter where T : IBaseValueConverter, new()
    {
        T converter = new T();

        object IValueConverter.Convert(object value, Type targetType, object parameter, string culture)
        {
            return converter.Convert(value, targetType, parameter);
        }

        object IValueConverter.ConvertBack(object value, Type targetType, object parameter, string culture)
        {
            return converter.ConvertBack(value, targetType, parameter);
        }
    }

}
