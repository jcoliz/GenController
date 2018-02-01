using System;

namespace Commonality.Converters
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

}
