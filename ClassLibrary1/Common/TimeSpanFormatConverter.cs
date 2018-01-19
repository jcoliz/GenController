using System;

namespace Common.Converters
{
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

}
