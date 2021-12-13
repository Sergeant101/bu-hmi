using System.Windows;
using Monokot.Hmi.Wpf.AttachedBehaviors;
using System.Windows.Input;

namespace NGO.Elements.Special.ClickCommandMode
{
    public class MomentaryConfirmMode : HmiClickModeBase
    {
        #region Методы


        private static void ExecuteCommand(RoutedEventArgs e, ICommand command, ActionParameter parameter)
        {
            if (command != null && command.CanExecute(null))
            {
                // Оставляем такой порядок,
                // связано с тем, что Handled должен выставиться в не зависимости от того
                // все хорошо с параметром или нет
                if (parameter != null)
                    command.Execute(parameter.GetValue());

                e.Handled = true;
            }
        }

        #endregion

        #region Свойства

        public override bool UseLostKeyboardFocus
        {
            get { return true; }
        }

        public override bool UseMouseLeave
        {
            get { return true; }
        }

        public static readonly DependencyProperty PressValueProperty = DependencyProperty.Register(
            "PressValue", typeof(ActionParameter), typeof(MomentaryConfirmMode), new PropertyMetadata(new BooleanActionParameter { Value = true }));

        public ActionParameter PressValue
        {
            get { return (ActionParameter)GetValue(PressValueProperty); }
            set { SetValue(PressValueProperty, value); }
        }

        public static readonly DependencyProperty ReleaseValueProperty = DependencyProperty.Register(
            "ReleaseValue", typeof(ActionParameter), typeof(MomentaryConfirmMode), new PropertyMetadata(new BooleanActionParameter() { Value = false }));

        public ActionParameter ReleaseValue
        {
            get { return (ActionParameter)GetValue(ReleaseValueProperty); }
            set { SetValue(ReleaseValueProperty, value); }
        }

        public static readonly DependencyProperty ConfirmWindowTextProperty = DependencyProperty.Register(
            "ConfirmWindowText", typeof(string), typeof(MomentaryConfirmMode), new PropertyMetadata("Are you sure?"));

        public string ConfirmWindowText
        {
            get { return (string)GetValue(ConfirmWindowTextProperty); }
            set { SetValue(ConfirmWindowTextProperty, value); }
        }

        public static readonly DependencyProperty ConfirmWindowCaptionProperty = DependencyProperty.Register(
            "ConfirmWindowCaption", typeof(string), typeof(MomentaryConfirmMode), new PropertyMetadata("Attention"));

        public string ConfirmWindowCaption
        {
            get { return (string)GetValue(ConfirmWindowCaptionProperty); }
            set { SetValue(ConfirmWindowCaptionProperty, value); }
        }

        public override void ExecuteDownCommand(DependencyObject sender, RoutedEventArgs e)
        {
            var command = Click.GetCommand(sender);

            var result = MessageBox.Show(ConfirmWindowText, ConfirmWindowCaption, MessageBoxButton.YesNo, MessageBoxImage.Question);
            if (result == MessageBoxResult.Yes)
            {
                ExecuteCommand(e, command, PressValue);
                Application.Current.GetType().GetMethod("zapisat_kodUstroistva_i_ostanovit_obnovlenie_pri_schitivanii", System.Reflection.BindingFlags.Static | System.Reflection.BindingFlags.Public).Invoke(Application.Current, new[] { sender });

                System.Threading.Thread.Sleep(1200);
                ExecuteCommand(e, command, ReleaseValue);

                Application.Current.GetType().GetMethod("vozobnovit_schitivanie", System.Reflection.BindingFlags.Static | System.Reflection.BindingFlags.Public).Invoke(Application.Current, new object[] { });
            }
        }

        public override void ExecuteUpCommand(DependencyObject sender, RoutedEventArgs e)
        {

        }

        #endregion
    }
}
