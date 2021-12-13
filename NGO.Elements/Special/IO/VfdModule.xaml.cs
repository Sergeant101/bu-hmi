using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using Monokot.Hmi.Wpf.Utils;

namespace NGO.Elements.Special.IO
{
    /// <summary>
    /// Логика взаимодействия для VfdModule.xaml
    /// </summary>
    public partial class VfdModule : UserControl
    {
        public VfdModule()
        {
            InitializeComponent();

            backCommand = new RelayCommand(click_to_back);
        }

        public static DependencyProperty moduleProperty =
            DependencyProperty.Register("module", typeof (Backplane), typeof (VfdModule), null);

        public Backplane module
        {
            get { return (Backplane) GetValue(moduleProperty); }
            set { SetValue(moduleProperty, value); }
        }

        public static readonly DependencyProperty backCommandProperty = DependencyProperty.Register(
            "backCommand", typeof (ICommand), typeof (VfdModule), new PropertyMetadata(default(ICommand)));

        public ICommand backCommand
        {
            get { return (ICommand) GetValue(backCommandProperty); }
            set { SetValue(backCommandProperty, value); }
        }

        private void click_to_back(object sender)
        {
            if (module != null)
                module.showModules = false;
        }
    }
}
