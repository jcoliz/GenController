using System;
using Commonality.Converters;
using Windows.UI.Xaml.Data;

namespace GenController.Uwp.Converters
{
    /// <summary>
    /// Generic class to convert Portable valueconverters into Xaml-specific valueconverters
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
