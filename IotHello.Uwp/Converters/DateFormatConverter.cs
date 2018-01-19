using IotHello.Uwp.Platform;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Windows.UI.Xaml.Data;

namespace IotHello.Uwp.Converters
{
    public class DateFormatConverter : IBaseValueConverter
    {
        public object Convert(object value, Type targetType, object parameter)
        {
            String format = null;
            if (parameter != null && (String)parameter != null)
                format = (String)parameter;

            DateTime? input = value as DateTime?;

            String result = "---";
            if (input.HasValue)
            {
                if (format != null)
                    result = input.Value.ToString(format);
                else
                    result = input.Value.ToString();
            }

            return result;

        }

        public object ConvertBack(object value, Type targetType, object parameter)
        {
            throw new NotImplementedException();
        }
    }

    public class TimeSpanFormatConverter : IBaseValueConverter
    {
        public object Convert(object value, Type targetType, object parameter)
        {
            String format = null;
            if (parameter != null && (String)parameter != null)
                format = ((String)parameter).Replace(":", "\\:");

            TimeSpan? input = value as TimeSpan?;

            String result = "---";
            if (input.HasValue)
            {
                if (format != null)
                    result = input.Value.ToString(format);
                else
                    result = input.Value.ToString();
            }
            return result;
        }

        public object ConvertBack(object value, Type targetType, object parameter)
        {
            throw new NotImplementedException();
        }
    }

    public class DateFormatConverterXaml : XamlValueConverter<DateFormatConverter>
    {
    }
    public class TimeSpanFormatConverterXaml : XamlValueConverter<TimeSpanFormatConverter>
    {
    }
}


