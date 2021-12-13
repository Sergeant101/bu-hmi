using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;
using Monokot.Hmi.Core.Framework.Runtime;
using Monokot.Hmi.Wpf.Rules;
using NGO.Elements.Annotations;

namespace NGO.Elements.Special.Keypad
{
    public partial class ArrayWndKeypad : Window, INotifyPropertyChanged
    {
        #region FIELDS

        private const int MAX_LENGHT_INTEGRALPART = 10;
        private const int MAX_LENGHT_FLOATINGPART = 5;

        private bool _isFloat;
        private double _result;
        private string _numbers;
        private bool _canAddSeparator;

        public List<IKeypadValidationRule> validationRules = new List<IKeypadValidationRule>();
        private string _validationErrorText = "ENTER INVALID VALUE! PLEASE CHECK VALUE!";

        public event PropertyChangedEventHandler PropertyChanged;

        #endregion

        #region PROPERTIES

        public string ValidationErrorText
        {
            get { return _validationErrorText; }
            set
            {
                if (value == _validationErrorText) return;
                _validationErrorText = value;
                OnPropertyChanged("ValidationErrorText");
            }
        }

        public double Result
        {
            get { return _result; }
            private set
            {
                _result = value;
                OnPropertyChanged("Result");
            }
        }

        public bool IsInteger
        {
            get { return point.Visibility == Visibility.Visible; }
            set { point.Visibility = value ? Visibility.Hidden : Visibility.Visible; }
        }

        public bool IsPositive
        {
            get { return plusminus.Visibility == Visibility.Visible; }
            set { plusminus.Visibility = value ? Visibility.Hidden : Visibility.Visible; }
        }

        public string Numbers
        {
            get { return _numbers; }
            set
            {

                if (_numbers == value) return;
                _numbers = value;
                OnPropertyChanged("Numbers");

            }
        }

        public bool CanAddSeparator
        {
            get { return _canAddSeparator; }
            set
            {
                if (value.Equals(_canAddSeparator)) return;
                _canAddSeparator = value;
                OnPropertyChanged("CanAddSeparator");
            }
        }

        public static readonly DependencyProperty MessageProperty =
            DependencyProperty.Register("Message", typeof(string), typeof(ArrayWndKeypad), new FrameworkPropertyMetadata(string.Empty));

        public string Message
        {
            get { return (string)GetValue(MessageProperty); }
            set { SetValue(MessageProperty, value); }
        }

        #endregion

        public ArrayWndKeypad()
        {
            InitializeComponent();
        }

        #region METHODS

        private void button_Click(object sender, RoutedEventArgs e)
        {
            var button = sender as Button;

            _isFloat = txtNumber.Text.IndexOf(NumberFormatInfo.CurrentInfo.CurrencyDecimalSeparator, StringComparison.Ordinal) != (-1);



            // ReSharper disable once PossibleNullReferenceException
            switch (button.CommandParameter.ToString())
            {
                case "ESC":

                    DialogResult = false;
                    break;

                case "RETURN":

                    double value;

                    if (double.TryParse(Numbers, NumberStyles.Any, CultureInfo.CurrentCulture, out value))
                    {
                        if (validationRules != null)
                        {
                            var isValid = true;
                            var errorMessage = string.Empty;

                            //Проверить на нескольких не валидных правилах
                            foreach (var ruleResult in validationRules.Select(rule => rule.Validate(value, CultureInfo.CurrentCulture))
                                .Where(validationResult => !validationResult.IsValid))
                            {
                                isValid = false;
                                errorMessage = ruleResult.ErrorContent != null ? ruleResult.ErrorContent.ToString() : string.Empty;
                            }

                            if (isValid)
                            {
                                Result = value;
                            }
                            else
                            {
                                RuntimeManager.Manager.ShowMessage("Warning", errorMessage,
                                    Monokot.Hmi.Core.Framework.Platform.MessageBox.MessageBoxButton.OK,
                                    Monokot.Hmi.Core.Framework.Platform.MessageBox.MessageBoxImage.Warning);
                                return;
                            }
                        }

                        DialogResult = true;
                    }
                    else
                    {
                        DialogResult = false;
                    }

                    break;

                case "BACK":

                    if (!string.IsNullOrEmpty(Numbers))
                    {

                        var regexp = new Regex(@"-0" + NumberFormatInfo.CurrentInfo.CurrencyDecimalSeparator);

                        if (regexp.Match(Numbers).Value.Length == Numbers.Length ||
                             (Numbers.IndexOf("-", StringComparison.Ordinal) == 0 && Numbers.Length == 2) ||
                                Numbers.Length == 1)
                        {
                            Numbers = "0";
                            break;
                        }

                        Numbers = Numbers.Remove(Numbers.Length - 1);
                    }

                    CanAddSeparator = false;

                    break;

                case ".":

                    if (CanAddSeparator)
                    {
                        Numbers = "0" + NumberFormatInfo.CurrentInfo.CurrencyDecimalSeparator;
                        CanAddSeparator = false;
                    }
                    else
                    {
                        if (!_isFloat)
                        {
                            if (Numbers.Length == 0)
                            {
                                Numbers += "0";
                            }

                            Numbers += NumberFormatInfo.CurrentInfo.CurrencyDecimalSeparator;
                            _isFloat = true;
                        }

                    }


                    break;

                case "PLUSMINUS":

                    double d;

                    if (double.TryParse(Numbers, NumberStyles.Any, CultureInfo.CurrentCulture, out d))
                    {
                        d *= -1;
                        Numbers = d.ToString(CultureInfo.CurrentCulture);
                    }
                    CanAddSeparator = false;

                    break;

                case "CLEAR":

                    Numbers = "0";
                    _isFloat = false;
                    CanAddSeparator = false;

                    break;

                default:
                    {
                        if (CanAddSeparator)
                        {
                            Numbers = string.Empty;
                            CanAddSeparator = false;
                        }

                        if (Numbers.IndexOf("0", StringComparison.Ordinal) == 0 && Numbers.Length <= 1)
                        {
                            Numbers = button.Content.ToString();
                            break;
                        }

                        var s = Numbers + button.Content;

                        //var g = LenghtFloatingPoint(s);
                        //var h = LenghtIntegralPart(s);
                        //Trace.WriteLine(string.Format("число {0}; длина дробной части {1}; длина целой части {2}", s, g, h));

                        if (LenghtFloatingPoint(s) <= MAX_LENGHT_FLOATINGPART &&
                            LenghtIntegralPart(s) <= MAX_LENGHT_INTEGRALPART)
                        {
                            Numbers += button.Content.ToString();
                        }

                        break;
                    }

            }

            txtNumber.Text = Numbers;
        }

        private int LenghtFloatingPoint(string s)
        {
            if (s.IndexOf(NumberFormatInfo.CurrentInfo.CurrencyDecimalSeparator, StringComparison.Ordinal) != -1)
            {
                return s.Length - s.IndexOf(NumberFormatInfo.CurrentInfo.CurrencyDecimalSeparator, StringComparison.Ordinal) - 1;
            }

            return -1;
        }

        private int LenghtIntegralPart(string s)
        {
            double d;
            double.TryParse(s, NumberStyles.Any, CultureInfo.CurrentCulture, out d);
            var integralpart = Math.Floor(Math.Abs(d));

            return integralpart.ToString(CultureInfo.CurrentCulture).Length;
        }

        [NotifyPropertyChangedInvocator]
        protected virtual void OnPropertyChanged(string propertyName)
        {
            var handler = PropertyChanged;
            if (handler != null) handler(this, new PropertyChangedEventArgs(propertyName));
        }

        #endregion

    }    
}
