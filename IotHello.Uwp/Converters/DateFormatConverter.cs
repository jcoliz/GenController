using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Windows.UI.Xaml.Data;

namespace IotHello.Uwp.Converters
{
    public class DateFormatConverter: IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, string culture)
        {
            String format = null;
            if (parameter != null && (String)parameter != null)
                format = (String)parameter;

            String result = "---";
            if (value is DateTime?)
            {
                var input = value as DateTime?;

                if (input.HasValue)
                {
                    if (format != null)
                        result = input.Value.ToString(format);
                    else
                        result = input.Value.ToString();
                }

            }
            else if (value is TimeSpan?)
            {
                var input = value as TimeSpan?;

                if (input.HasValue)
                {
                    if (format != null)
                        result = input.Value.ToString(format);
                    else
                        result = input.Value.ToString();
                }
            }
            return result;
        }

        public object ConvertBack(object value, Type targetType, object parameter, string culture)
        {
            throw new NotImplementedException();
        }
    }
}
