using System.ComponentModel;
using System.Windows;
using System.Windows.Controls;

namespace NGO.Elements
{
    /// <summary>
    /// Логика взаимодействия для loginControl.xaml
    /// </summary>
    public partial class LoginControl : UserControl
    {
        public LoginControl()
        {
            InitializeComponent();
        }

       // private PermissionPlugin _plug;


        public static DependencyProperty moduleNameProperty =
            DependencyProperty.Register("moduleName", typeof(string), typeof(LoginControl),
                                         new FrameworkPropertyMetadata(string.Empty, FrameworkPropertyMetadataOptions.AffectsArrange, callback));

        [Category("SCADAExtension")]
        public string moduleName
        {
            get { return (string)GetValue(moduleNameProperty); }
            set { SetValue(moduleNameProperty, value); }
        }

        public static DependencyProperty passTextProperty =
            DependencyProperty.Register("passText", typeof(string), typeof(LoginControl), new FrameworkPropertyMetadata("Password", FrameworkPropertyMetadataOptions.AffectsRender));

        [Category("SCADAExtension")]
        public string passText
        {
            get { return (string)GetValue(passTextProperty); }
            set { SetValue(passTextProperty, value); }
        }


        public static DependencyProperty usrTextProperty =
             DependencyProperty.Register("usrText", typeof(string), typeof(LoginControl), new FrameworkPropertyMetadata("User", FrameworkPropertyMetadataOptions.AffectsRender));

        [Category("SCADAExtension")]
        public string usrText
        {
            get { return (string)GetValue(usrTextProperty); }
            set { SetValue(usrTextProperty, value); }
        }

        public static DependencyProperty loginBtnTextProperty =
           DependencyProperty.Register("loginBtnText", typeof(string), typeof(LoginControl), new FrameworkPropertyMetadata("Login", FrameworkPropertyMetadataOptions.AffectsRender));

        [Category("SCADAExtension")]
        public string loginBtnText
        {
            get { return (string)GetValue(loginBtnTextProperty); }
            set { SetValue(loginBtnTextProperty, value); }
        }

        public static DependencyProperty logoutBtnTextProperty =
            DependencyProperty.Register("logoutBtnText", typeof(string), typeof(LoginControl), new FrameworkPropertyMetadata("Logout", FrameworkPropertyMetadataOptions.AffectsRender));

        [Category("SCADAExtension")]
        public string logoutBtnText
        {
            get { return (string)GetValue(logoutBtnTextProperty); }
            set { SetValue(logoutBtnTextProperty, value); }
        }

        public static DependencyProperty buttonStyleProperty =
            DependencyProperty.Register("buttonStyle", typeof(Style), typeof(LoginControl), new FrameworkPropertyMetadata(null, FrameworkPropertyMetadataOptions.AffectsRender, styleCallback));

        [Category("SCADAExtension")]
        public Style buttonStyle
        {
            get { return (Style)GetValue(buttonStyleProperty); }
            set { SetValue(buttonStyleProperty, value); }
        }


        public static DependencyProperty comboboxStyleProperty =
            DependencyProperty.Register("comboboxStyle", typeof(Style), typeof(LoginControl), new FrameworkPropertyMetadata(null, FrameworkPropertyMetadataOptions.AffectsRender, styleCallback));

        [Category("SCADAExtension")]
        public Style checkboxStyle
        {
            get { return (Style)GetValue(comboboxStyleProperty); }
            set { SetValue(comboboxStyleProperty, value); }
        }

        private static void styleCallback(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            var loginControl = (LoginControl)d;
            if (loginControl == null) return;


            if (e.Property.Name == "buttonStyle")
            {
                var style = (Style)e.NewValue;
                loginControl.loginButton.Style = style;
                loginControl.logoutButton.Style = style;
            }

            if (e.Property.Name == "comboboxStyle")
            {
                var style = (Style)e.NewValue;
                loginControl.combo.Style = style;
            }
        }

        private static void callback(DependencyObject dep_obj, DependencyPropertyChangedEventArgs event_args)
        {
            //var moduleName = event_args.NewValue.ToString();
            //var module = (HmiProjectPluginModule)HmiManager.manager.integratedProject.modules[PluginModule.MODULE_ID];

            //if (!module.objects.ContainsKey(moduleName)) return;

            //var loginControl = (LoginControl)dep_obj;
            //loginControl._plug = (PermissionPlugin)module.objects[moduleName];
            //loginControl.combo.ItemsSource = loginControl._plug.users;
            //loginControl.combo.DisplayMemberPath = "userName";
        }

        private void PasswordBox_OnPasswordChanged(object sender, RoutedEventArgs e)
        {
            //if (_plug != null)
            //{
            //    _plug.password = pwd.Password;
            //}
        }

        private void combo_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            //var cmbbx = (ComboBox)sender;
            //var sel = cmbbx.SelectedItem as PermissionPlugin.User;
            //if (sel != null)
            //{
            //    _plug.userName = sel.userName;
            //}
        }
    }
}
