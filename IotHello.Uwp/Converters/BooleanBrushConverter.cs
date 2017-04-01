using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Windows.UI.Xaml.Data;
using Windows.UI.Xaml.Media;

namespace IotHello.Uwp.Converters
{
    class BooleanBrushConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, string language)
        {
            // Paramter should be a comma-separated set of strings containing values in the Windows.UI.Colors enumeration
            if (parameter == null)
                throw new ArgumentNullException(nameof(parameter));

            var parameter_string = parameter as string;
            var parameter_array = parameter_string.Split(',');
    
            if (parameter_array.Count() != 2)
                throw new ArgumentException("Expected: 'AARRGGBB,AARRGGBB'",nameof(parameter));

            if (!(value is Boolean))
                throw new ArgumentException("Expected: Boolean", nameof(value));

            var value_bool = (Boolean)value;

            string which_parameter = parameter_array[1];
            if (value_bool)
                which_parameter = parameter_array[0];

            var argb = System.Convert.ToUInt32(which_parameter,16);
            var color = Windows.UI.ColorHelper.FromArgb((byte)(argb >> 24),(byte)((argb >> 16) & 0xff),(byte)((argb >> 8) & 0xff),(byte)(argb & 0xff));
            var brush = new SolidColorBrush(color);

            if (targetType != typeof(Brush))
                throw new ArgumentException("Expected: Brush", nameof(targetType));

            return brush;
        }

        public object ConvertBack(object value, Type targetType, object parameter, string language)
        {
            throw new NotImplementedException();
        }
    }
}
