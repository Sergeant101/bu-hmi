using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using Monokot.Hmi.Wpf.Utils;
using Monokot.ScadaExtension.Wpf.Controls.Buttons;

namespace NGO.Elements.Special.Keypad
{
    public class ArrayKeypadButton : Button
    {
        private bool _keypadIsOpen;
        private ArrayWndKeypad _window = null;

        static ArrayKeypadButton()
        {
            DefaultStyleKeyProperty.OverrideMetadata(typeof(KeypadButton),
                new FrameworkPropertyMetadata(typeof(KeypadButton)));
        }

        public ArrayKeypadButton()
        {
            SetValue(RulesProperty, new KeypadValidationRuleCollection());
            Click += KeypadButton_Click;
        }

        #region Свойства

        [Browsable(false)] public bool KeypadDialogResult { get; set; }

        //public static readonly DependencyProperty ValueProperty =
        //    DependencyProperty.Register("Value", typeof(double), typeof(KeypadButton),
        //        new FrameworkPropertyMetadata(0d, FrameworkPropertyMetadataOptions.BindsTwoWayByDefault));

        //[Category(Categories.GENERAL_CATEGORY)]
        //public double Value
        //{
        //    get { return (double) GetValue(ValueProperty); }
        //    set { SetValue(ValueProperty, value); }
        //}

        public static readonly DependencyProperty IsIntegerProperty =
            DependencyProperty.Register("IsInteger", typeof(bool), typeof(KeypadButton),
                new FrameworkPropertyMetadata(false));

        [Category(Categories.GENERAL_CATEGORY)]
        public bool IsInteger
        {
            get { return (bool) GetValue(IsIntegerProperty); }
            set { SetValue(IsIntegerProperty, value); }
        }

        public static readonly DependencyProperty IsPositiveProperty =
            DependencyProperty.Register("IsPositive", typeof(bool), typeof(KeypadButton),
                new FrameworkPropertyMetadata(false));

        [Category(Categories.GENERAL_CATEGORY)]
        public bool IsPositive
        {
            get { return (bool) GetValue(IsPositiveProperty); }
            set { SetValue(IsPositiveProperty, value); }
        }

        public static readonly DependencyProperty TitleProperty =
            DependencyProperty.Register("Title", typeof(string), typeof(KeypadButton),
                new FrameworkPropertyMetadata("Window title"));

        [Category(Categories.GENERAL_CATEGORY)]
        public string Title
        {
            get { return (string) GetValue(TitleProperty); }
            set { SetValue(TitleProperty, value); }
        }

        public static readonly DependencyProperty MessageProperty =
            DependencyProperty.Register("Message", typeof(string), typeof(KeypadButton),
                new FrameworkPropertyMetadata("Window message"));

        [Category(Categories.GENERAL_CATEGORY)]
        public string Message
        {
            get { return (string) GetValue(MessageProperty); }
            set { SetValue(MessageProperty, value); }
        }

        public static readonly DependencyProperty RulesProperty =
            DependencyProperty.Register("Rules", typeof(KeypadValidationRuleCollection), typeof(KeypadButton),
                new FrameworkPropertyMetadata(null));

        [Category(Categories.GENERAL_CATEGORY)]
        [Obsolete("Legacy. Do not use this property")]
        public KeypadValidationRuleCollection Rules
        {
            get { return (KeypadValidationRuleCollection) GetValue(RulesProperty); }
            set { SetValue(RulesProperty, value); }
        }

        public static readonly DependencyProperty IDProperty = DependencyProperty.Register(
            "ID", typeof(float), typeof(ArrayKeypadButton), new PropertyMetadata(1f));

        public float ID
        {
            get { return (float) GetValue(IDProperty); }
            set { SetValue(IDProperty, value); }
        }

        public static readonly DependencyProperty ArrayProperty = DependencyProperty.Register(
            "Array", typeof(Array), typeof(ArrayKeypadButton), new PropertyMetadata(default(Array)));

        public Array Array
        {
            get { return (Array) GetValue(ArrayProperty); }
            set { SetValue(ArrayProperty, value); }
        }

        #endregion

        #region Методы

        void KeypadButton_Click(object sender, RoutedEventArgs e)
        {

            //var bindingExpression = BindingOperations.GetBindingExpression(this, KeypadButton.valueProperty);
            //if (bindingExpression != null)
            //{
            //    var binding = new Binding(bindingExpression.ParentBinding.Path.Path)
            //    {
            //        Source = bindingExpression.DataItem,
            //        Mode = BindingMode.TwoWay,
            //        UpdateSourceTrigger = UpdateSourceTrigger.Explicit

            //    };
            //    BindingOperations.SetBinding(_keypdad, CtrlKeypad.valueProperty, binding);
            //}

            //_keypdad.multiplyFactor = multyplyFactor;
            //_keypdad.isPositive = isPositive;
            //_keypdad.isInteger = isInteger;
            //_keypdad.windowTitle = windowTitle;
            //_keypdad.windowMessage = windowMessage;

            //if (validationRules != null)
            //    _keypdad.useValidationRule = true;
            //_keypdad.validationRules = validationRules;

            //_keypdad.showKeypad = true;



            //var expression = BindingOperations.GetBindingExpression(_keypdad, CtrlKeypad.valueProperty);
            ////Проверяем что окно закрылось клавишей ввода
            //if (expression != null && _keypdad.keypadDialogResult)
            //    expression.UpdateSource();

            //if (validationRules != null)
            //    _keypdad.useValidationRule = true;
            //_keypdad.validationRules = validationRules;

            if (!_keypadIsOpen)
            {
                if (_window == null)
                {
                    object value = 0;

                    if (Array != null && Array.Length > 2)
                        value = Array.GetValue(1);

                    _window = new ArrayWndKeypad
                    {

                        Title = Title,
                        //txtMessage = { Text = Message },
                        Numbers = string.Format(CultureInfo.CurrentCulture, "{0:0.###}", value),
                        CanAddSeparator = true,
                        validationRules = Rules

                    };

                    if (string.IsNullOrEmpty(Message))
                        _window.Visibility = Visibility.Collapsed;

                    _window.IsPositive = IsPositive;
                    _window.IsInteger = IsInteger;
                    _window.Message = Message;

                    //if (IsPositive)
                    //    _window.plusminus.Visibility = Visibility.Hidden;

                    //if (IsInteger)
                    //    _window.point.Visibility = Visibility.Hidden;

                    if (_window.ShowDialog() != true)
                    {
                        _window = null;
                        _keypadIsOpen = false;
                        KeypadDialogResult = false;

                        return;
                    }

                    var result = _window.Result;
                    var resetFlag = 0f;

                    if (Math.Abs(result) < 0.000001)
                        resetFlag = 1f;

                    Array = new float[] {ID, (float) result, resetFlag};

                    _window = null;
                    _keypadIsOpen = false;
                    KeypadDialogResult = true;
                }
            }
        }

        #endregion
    }
}
