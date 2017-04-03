using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace IotHello.Uwp.Converters
{
    public abstract class DefaultConverter : ManiaLabs.Portable.Base.ValueConverters.IBaseValueConverter
    {
        protected T Convert<T>(T yes, T no, object value, Type targetType, object parameter)
        {
            if (typeof(T) != targetType)
            {
                throw new ArgumentException("Expected type: " + typeof(T).Name, nameof(targetType));
            }

            bool invert = (parameter?.GetType() == typeof(string) && ((parameter as string) == "invert" || (parameter as string) == "false"));

            bool isdefault;

            var t = value?.GetType();

            if (t == typeof(bool))
            {
                isdefault = (bool)value == false;
            }
            else if (t == typeof(int) || t == typeof(double))
            {
                isdefault = (System.Convert.ToDouble(value) == 0.0);
            }
            else if (t == typeof(DateTime))
            {
                isdefault = (DateTime)value == DateTime.MinValue;
            }
            else if (t == typeof(string))
            {
                isdefault = value == null || (value as string).Length == 0;
            }
            else
            {
                isdefault = value == null;
            }

            return isdefault ^ invert ? yes : no;
        }

        public abstract object Convert(object value, Type targetType, object parameter);

        public object ConvertBack(object value, Type targetType, object parameter)
        {
            throw new NotImplementedException();
        }
    }


}
