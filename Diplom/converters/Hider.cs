﻿using System;
using System.Globalization;
using System.Windows;
using System.Windows.Data;

namespace Diplom.converters
{
    public class Hider : IValueConverter
    {

        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            var flag = false;
            if (value is bool)
            {
                flag = (bool)value;
            }
            return (flag ? Visibility.Visible : Visibility.Collapsed);
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {

            return true;
        }
    }
}
