using System.Windows;
using System.Windows.Controls;

namespace NGO.Elements.Special.ConfigurateSituation
{
    /// <summary>
    /// Логика взаимодействия для ConfigureControl.xaml
    /// </summary>
    public partial class ConfigureItemControl : UserControl
    {
        public ConfigureItemControl()
        {
            InitializeComponent();
        }

        public static readonly DependencyProperty textProperty = DependencyProperty.Register(
            "text", typeof(string), typeof(ConfigureItemControl), new PropertyMetadata(string.Empty));

        public string text
        {
            get { return (string)GetValue(textProperty); }
            set { SetValue(textProperty, value); }
        }

        public static readonly DependencyProperty currentValueProperty = DependencyProperty.Register(
            "currentValue", typeof(double), typeof(ConfigureItemControl), new PropertyMetadata(default(double)));

        public double currentValue
        {
            get { return (double) GetValue(currentValueProperty); }
            set { SetValue(currentValueProperty, value); }
        }

        public static readonly DependencyProperty startProperty = DependencyProperty.Register(
            "start", typeof(double), typeof(ConfigureItemControl), new PropertyMetadata(default(double)));

        public double start
        {
            get { return (double) GetValue(startProperty); }
            set { SetValue(startProperty, value); }
        }

        public static readonly DependencyProperty endProperty = DependencyProperty.Register(
            "end", typeof(double), typeof(ConfigureItemControl), new PropertyMetadata(default(double)));

        public double end
        {
            get { return (double) GetValue(endProperty); }
            set { SetValue(endProperty, value); }
        }

        public static readonly DependencyProperty deltaProperty = DependencyProperty.Register(
            "delta", typeof(double), typeof(ConfigureItemControl), new PropertyMetadata(default(double)));

        public double delta
        {
            get { return (double) GetValue(deltaProperty); }
            set { SetValue(deltaProperty, value); }
        }

        public static readonly DependencyProperty startCalcValueProperty = DependencyProperty.Register(
            "startCalcValue", typeof(double), typeof(ConfigureItemControl), new PropertyMetadata(default(double)));

        public double startCalcValue
        {
            get { return (double) GetValue(startCalcValueProperty); }
            set { SetValue(startCalcValueProperty, value); }
        }

        public static readonly DependencyProperty endCalcValueProperty = DependencyProperty.Register(
            "endCalcValue", typeof(double), typeof(ConfigureItemControl), new PropertyMetadata(default(double)));

        public double endCalcValue
        {
            get { return (double) GetValue(endCalcValueProperty); }
            set { SetValue(endCalcValueProperty, value); }
        }

        public static readonly DependencyProperty deltaCalcValueProperty = DependencyProperty.Register(
            "deltaCalcValue", typeof(double), typeof(ConfigureItemControl), new PropertyMetadata(default(double)));

        public double deltaCalcValue
        {
            get { return (double) GetValue(deltaCalcValueProperty); }
            set { SetValue(deltaCalcValueProperty, value); }
        }

        public static readonly DependencyProperty textColumnWidthProperty = DependencyProperty.Register(
            "textColumnWidth", typeof(GridLength), typeof(ConfigureItemControl), new PropertyMetadata(new GridLength(200, GridUnitType.Pixel)));

        public GridLength textColumnWidth
        {
            get { return (GridLength)GetValue(textColumnWidthProperty); }
            set { SetValue(textColumnWidthProperty, value); }
        }
    }
}
