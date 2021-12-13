using System.Windows;
using System.Windows.Controls;

namespace NGO.Elements.Special.Permissions
{
    public partial class PermissionButton : UserControl
    {
        public PermissionButton()
        {
            InitializeComponent();
        }

        public static DependencyProperty loginTextProperty =
            DependencyProperty.Register("loginText", typeof (string), typeof (PermissionButton),
                new FrameworkPropertyMetadata("ВВЕСТИ ПАРОЛЬ..."));

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

        //private PermissionPlugin _plugin;

        public string userName { get; set; }

        private void Button_Click(object sender, RoutedEventArgs e)
        {
            //if (_plugin == null)
            //{
            //    _plugin =
            //        (PermissionPlugin)
            //            ((HmiProjectMapModule<string, IHmiPlugin>) HmiManager.manager.integratedProject.modules[PluginModule.MODULE_ID]).objects
            //                .Values.FirstOrDefault(o => o.name == permissionPluginName);
            //}

            //DataContext = _plugin;

            //if (_plugin != null)
            //{
            //    var wndPasswordKeypad = new WndPasswordKeypad();

            //    wndPasswordKeypad.Title = "ВВЕДИТЕ ПАРОЛЬ";

            //    wndPasswordKeypad.ShowDialog();

            //    if (wndPasswordKeypad.DialogResult == true)
            //    {
            //        _plugin.userName = userName;

            //        try
            //        {
            //            _plugin.password = wndPasswordKeypad.result;
            //            ((RelayCommand)_plugin.loginCommand).Execute(null);
            //        }
            //        catch (Exception x)
            //        {
            //            //Helper.exceptionReport(x);
            //        }
            //    }
            //}
            //else
            //{
            //   // Helper.diagnosticReport("PermissionButton : Error! Did not to find permission plugin with name " + permissionPluginName);
            //}
        }

        private void Logout_click(object sender, RoutedEventArgs e)
        {
            //if (_plugin != null)
            //    ((RelayCommand)_plugin.logoutCommand).Execute(null);
        }
    }
}
