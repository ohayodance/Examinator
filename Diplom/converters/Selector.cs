using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Data;
using System.Windows.Media;
using Diplom.mvvm.models.subModels;

namespace Diplom.converters
{
    class Selector : IMultiValueConverter
    {
        public object Convert(object[] vls, Type targetType, object parameter, CultureInfo culture)
        {
            if((bool)vls[1])
                return new SolidColorBrush(Colors.Blue);
            if ((bool)vls[0])
                return new SolidColorBrush(Colors.Gray);
            return new SolidColorBrush(Colors.DodgerBlue);
        }

        public object[] ConvertBack(object value, Type[] targetTypes, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}
