using System.Windows;
using System.Windows.Controls;

namespace NGO.Elements
{
    /// <summary>
    /// Логика взаимодействия для passwordControl.xaml
    /// </summary>
    public partial class passwordControl : UserControl
    {
        public passwordControl()
        {
            InitializeComponent();

        }

        //private PermissionPlugin pp = null;

        public static DependencyProperty moduleNameProperty =
            DependencyProperty.Register("moduleName", typeof(string), typeof(passwordControl), 
                                         new FrameworkPropertyMetadata(string.Empty,FrameworkPropertyMetadataOptions.AffectsArrange, callback ));

        private static void callback(DependencyObject dep_obj, DependencyPropertyChangedEventArgs event_args)
        {
            //var moduleItemName = event_args.NewValue.ToString();
            //var module = (HmiProjectPluginModule) HmiManager.manager.integratedProject.modules[PluginModule.MODULE_ID];
            //if (module.objects.ContainsKey(moduleItemName))
            //{
            //    var ctrl = (passwordControl)dep_obj;
            //    ctrl.pp = (PermissionPlugin)module.objects[moduleItemName];
            //}


        }

        public string moduleName
        {
            get { return (string) GetValue(moduleNameProperty); }
            set { SetValue(moduleNameProperty, value); }
        }

        private void PasswordBox_OnPasswordChanged(object sender, RoutedEventArgs e)
        {
            //if (pp!=null)
            //{
            //    pp.password = pwd.Password;
            //    //pwd.Password = string.Empty;
            //}
        }
    }
}
