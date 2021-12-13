using System;
using System.Windows;
using System.Windows.Controls;
using Monokot.Hmi.Core.Framework.Runtime;
using NGO.Elements.Special.Permissions.Model;
using MessageBoxButton = Monokot.Hmi.Core.Framework.Platform.MessageBox.MessageBoxButton;
using MessageBoxImage = Monokot.Hmi.Core.Framework.Platform.MessageBox.MessageBoxImage;
using System.Windows.Markup;
using Monokot.ScadaExtension.WpfComponents.Keypad;


[assembly: XmlnsDefinition("http://schemas.uralmash.com/ngo/", "NGO.Elements.Special.Permissions.View")]
namespace NGO.Elements.Special.Permissions.View
{
    public partial class PermissionButton : UserControl
    {
        public PermissionButton()
        {
            InitializeComponent();
        }

        public static DependencyProperty loginTextProperty =
            DependencyProperty.Register("loginText", typeof (string), typeof (PermissionButton),
                new FrameworkPropertyMetadata("ВВЕСТИ ПАРОЛЬ"));

        public string loginText
        {
            get { return (string) GetValue(loginTextProperty); }
            set { SetValue(loginTextProperty, value); }
        }

        public static DependencyProperty logoutTextProperty =
            DependencyProperty.Register("logoutText", typeof (string), typeof (PermissionButton),
                new FrameworkPropertyMetadata("ВЫЙТИ"));

        private string _permissionPluginName = "PermissionPlugin";

        public string logoutText
        {
            get { return (string) GetValue(logoutTextProperty); }
            set { SetValue(logoutTextProperty, value); }
        }

        public string permissionPluginName
        {
            get { return _permissionPluginName; }
            set { _permissionPluginName = value; }
        }

        public string userName { get; set; }

        private void Button_Click(object sender, RoutedEventArgs e)
        {
            var wndPasswordKeypad = new WndPasswordKeypad();

            wndPasswordKeypad.Title = "ВВЕДИТЕ ПАРОЛЬ";

            wndPasswordKeypad.ShowDialog();

            if (wndPasswordKeypad.DialogResult == true)
            {
                NgoAuth.auth.login = userName;

                try
                {
                    NgoAuth.auth.password = wndPasswordKeypad.Result;
                    NgoAuth.auth.loginCommand.Execute(null);
                }
                catch (Exception x)
                {
                    RuntimeManager.Manager.ShowMessage("Error", x.Message, MessageBoxButton.OK, MessageBoxImage.Error);
                }
            }
        }

        private void Logout_click(object sender, RoutedEventArgs e)
        {
            NgoAuth.auth.logoutCommand.Execute(null);
        }
    }
}
