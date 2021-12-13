using System.Windows;
using System.Windows.Controls;

namespace NGO.Elements.Special.BrakeScheme
{
    /// <summary>
    /// Логика взаимодействия для hydroSwitcher.xaml
    /// </summary>
    public partial class HydroControl1 : UserControl
    {
        public HydroControl1()
        {
            InitializeComponent();
        }

        public static DependencyProperty openProperty =
            DependencyProperty.Register("open", typeof(bool), typeof(HydroControl1), new FrameworkPropertyMetadata(false, FrameworkPropertyMetadataOptions.AffectsRender, callback));

        private static void callback(DependencyObject dep_obj, DependencyPropertyChangedEventArgs dp_event_args)
        {
            var val = (bool)dp_event_args.NewValue;
            var cntrl = (HydroControl1)dep_obj;
            if (val != null)
            {
                if (val)
                {
                    cntrl.on.Visibility = Visibility.Visible;
                    cntrl.off.Visibility = Visibility.Hidden;

                }
                else
                {
                    cntrl.on.Visibility = Visibility.Hidden;
                    cntrl.off.Visibility = Visibility.Visible;

                }
            }
        }

        public bool open
        {
            get { return (bool)GetValue(openProperty); }
            set { SetValue(openProperty, value); }
        }
    }
}
