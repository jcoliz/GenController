﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Data;

namespace IotHello.Uwp.Converters
{
    /// <summary>
    /// Converts a Boolean into a Visibility.
    /// </summary>
    public class BooleanToVisibilityConverter : IValueConverter
    {
        /// <summary>
        /// If set to True, conversion is reversed: True will become Collapsed.
        /// </summary>
        public bool IsReversed { get; set; }

        public object Convert(object value, Type typeName, object reversed, string language)
        {
            var val = System.Convert.ToBoolean(value);
            if (reversed != null && System.Convert.ToBoolean(reversed) == true)
            {
                val = !val;
            }

            if (val)
            {
                return Visibility.Visible;
            }

            return Visibility.Collapsed;
        }

        public object ConvertBack(object value, Type typeName, object parameter, string language)
        {
            throw new NotImplementedException();
        }
    }
}