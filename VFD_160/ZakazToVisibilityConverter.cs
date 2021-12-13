using System;
using System.Globalization;
using System.Windows;
using System.Windows.Data;

namespace VFD_160
{
    public class ZakazToVisibilityConverter : IValueConverter
    {
        private Visibility _trueVisibilityValue = Visibility.Visible;
        private Visibility _falseVisibilityValue = Visibility.Hidden;

        public Visibility TrueVisibilityValue { get { return _trueVisibilityValue; } set { _trueVisibilityValue = value; } }
        public Visibility FalseVisibilityValue { get { return _falseVisibilityValue; } set { _falseVisibilityValue = value; } }

        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (value is ZakazType && parameter is ZakazType)
            {
                var zakaz = (ZakazType)value;
                var zakaz_parameter = (ZakazType)parameter;

                if (zakaz == zakaz_parameter)
                    return TrueVisibilityValue;
            }

            return FalseVisibilityValue;
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}
