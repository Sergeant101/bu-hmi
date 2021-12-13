using System;
using System.Globalization;
using System.Windows;
using System.Windows.Data;

namespace NGO.Elements.Special.Complectation
{
    public class ComplectationToVisibilityConverter : IValueConverter
    {
        private Visibility _trueVisibilityValue = Visibility.Visible;
        private Visibility _falseVisibilityValue = Visibility.Hidden;

        public Visibility TrueVisibilityValue { get { return _trueVisibilityValue; } set { _trueVisibilityValue = value; } }
        public Visibility FalseVisibilityValue { get { return _falseVisibilityValue; } set { _falseVisibilityValue = value; } }

        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (value is Complectations && parameter is Complectations)
            {
                var complection = (Complectations)value;
                var complection_parameter = (Complectations)parameter;

                if (complection == complection_parameter)
                    return TrueVisibilityValue;
            }

            return FalseVisibilityValue;
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }


    public class InvertComplectationToVisibilityConverter : IValueConverter
    {
        private Visibility _trueVisibilityValue = Visibility.Visible;
        private Visibility _falseVisibilityValue = Visibility.Hidden;

        public Visibility TrueVisibilityValue { get { return _trueVisibilityValue; } set { _trueVisibilityValue = value; } }
        public Visibility FalseVisibilityValue { get { return _falseVisibilityValue; } set { _falseVisibilityValue = value; } }

        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (value is Complectations && parameter is Complectations)
            {
                var complection = (Complectations)value;
                var complection_parameter = (Complectations)parameter;

                if (complection != complection_parameter)
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
