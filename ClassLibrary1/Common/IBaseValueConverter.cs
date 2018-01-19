using System;

namespace Common.Converters
{
    /// <summary>
    /// This interface is implemented by value converters which are intended to be used
    /// across all platforms. On a given platform, we can programatically convert them
    /// to a value converter of the correct platform-specific interface
    /// </summary>
    public interface IBaseValueConverter
    {
        object Convert(object value, Type targetType, object parameter);

        object ConvertBack(object value, Type targetType, object parameter);
    }
}
