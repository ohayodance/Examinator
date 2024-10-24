using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Data;
using Diplom.mvvm.models.subModels;

namespace Diplom.converters
{
    public class Booler : IMultiValueConverter
    {

        /// <param name="values">Binding values</param>
        /// <param name="targetType">Merged value</param>
        /// <param name="parameter"></param>
        /// <param name="culture"></param>
        /// <returns></returns>
        public object Convert(object[] values, Type targetType, object parameter,
            CultureInfo culture)
        {
            bool listBoxValue = (bool)values[0];
            bool initializedValue = (bool)values[1];
            ((AModel) values[2]).IsSelected = listBoxValue || initializedValue;


            return listBoxValue || initializedValue;
        }

        /// <param name="value"></param>
        /// <param name="targetTypes"></param>
        /// <param name="parameter"></param>
        /// <param name="culture"></param>
        /// <returns></returns>
        public object[] ConvertBack(object value, Type[] targetTypes, object parameter,
            CultureInfo culture)
        {
            bool isChecked = (bool) value;
            object[] resultObjects=new object[2];
            resultObjects[0] = isChecked;
            resultObjects[1] = isChecked;
            return resultObjects;
        }
    }
}
