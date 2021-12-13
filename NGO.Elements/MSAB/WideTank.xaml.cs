using System.ComponentModel;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;

namespace NGO.Elements.MSAB
{
    public partial class WideTank : UserControl
    {
        public WideTank()
        {
            InitializeComponent();
            SetValue(WidthProperty, 230d);
            SetValue(HeightProperty, 350d);
        }

        public static DependencyProperty levelValueProperty =
            DependencyProperty.Register("levelValue", typeof(string), typeof(WideTank), new FrameworkPropertyMetadata("0", FrameworkPropertyMetadataOptions.AffectsRender, tankPropertyCallback));

        private static void tankPropertyCallback(DependencyObject dep_obj, DependencyPropertyChangedEventArgs dependency_property_changed_event_args)
        {
        }

        public string levelValue
        {
            get { return (string)GetValue(levelValueProperty); }
            set { SetValue(levelValueProperty, value); }
        }

        public static DependencyProperty volumeValueProperty =
            DependencyProperty.Register("volumeValue", typeof(string), typeof(WideTank), new FrameworkPropertyMetadata("0", FrameworkPropertyMetadataOptions.AffectsRender, tankPropertyCallback));

        public double volumeValue
        {
            get { return (double)GetValue(volumeValueProperty); }
            set { SetValue(volumeValueProperty, value); }
        }

        public static DependencyProperty levelMaxProperty =
            DependencyProperty.Register("levelMax", typeof(double), typeof(WideTank), new FrameworkPropertyMetadata(2d, FrameworkPropertyMetadataOptions.AffectsRender, tankPropertyCallback));

        public double levelMax
        {
            get { return (double)GetValue(levelMaxProperty); }

            set { SetValue(levelMaxProperty, value); }
        }

        public static DependencyProperty levelMinProperty =
            DependencyProperty.Register("levelMin", typeof(double), typeof(WideTank), new FrameworkPropertyMetadata(0d, FrameworkPropertyMetadataOptions.AffectsRender, tankPropertyCallback));

        public double levelMin
        {
            get { return (double)GetValue(levelMinProperty); }

            set { SetValue(levelMinProperty, value); }
        }

        public static DependencyProperty volumeMaxProperty =
            DependencyProperty.Register("volumeMax", typeof(double), typeof(WideTank), new FrameworkPropertyMetadata(100d, FrameworkPropertyMetadataOptions.AffectsRender, tankPropertyCallback));

        public double volumeMax
        {
            get { return (double)GetValue(volumeMaxProperty); }
            set { SetValue(volumeMaxProperty, value); }
        }

        public static DependencyProperty volumeMinProperty =
            DependencyProperty.Register("volumeMin", typeof(double), typeof(WideTank), new FrameworkPropertyMetadata(0d, FrameworkPropertyMetadataOptions.AffectsRender, tankPropertyCallback));

        public double volumeMin
        {
            get { return (double)GetValue(volumeMinProperty); }
            set { SetValue(volumeMinProperty, value); }
        }


        public static DependencyProperty levelMajorTickCostProperty =
            DependencyProperty.Register("levelMajorTickCost", typeof(double), typeof(WideTank), new FrameworkPropertyMetadata(1d, FrameworkPropertyMetadataOptions.AffectsRender, tankPropertyCallback));

        public double levelMajorTickCost
        {
            get { return (double)GetValue(levelMajorTickCostProperty); }
            set { SetValue(levelMajorTickCostProperty, value); }
        }

        public static DependencyProperty levelMinorTickCostProperty =
            DependencyProperty.Register("levelMinorTickCost", typeof(double), typeof(WideTank), new FrameworkPropertyMetadata(0.25d, FrameworkPropertyMetadataOptions.AffectsRender, tankPropertyCallback));

        public double levelMinorTickCost
        {
            get { return (double)GetValue(levelMinorTickCostProperty); }
            set { SetValue(levelMinorTickCostProperty, value); }
        }

        public static DependencyProperty volumeMajorTickCostProperty =
            DependencyProperty.Register("volumeMajorTickCost", typeof(double), typeof(WideTank), new FrameworkPropertyMetadata(50d, FrameworkPropertyMetadataOptions.AffectsRender, tankPropertyCallback));

        public double volumeMajorTickCost
        {
            get { return (double)GetValue(volumeMajorTickCostProperty); }
            set { SetValue(volumeMajorTickCostProperty, value); }
        }

        public static DependencyProperty volumeMinorTickCostProperty =
            DependencyProperty.Register("volumeMinorTickCost", typeof(double), typeof(WideTank), new FrameworkPropertyMetadata(10d, FrameworkPropertyMetadataOptions.AffectsRender, tankPropertyCallback));

        public double volumeMinorTickCost
        {
            get { return (double)GetValue(volumeMinorTickCostProperty); }
            set { SetValue(volumeMinorTickCostProperty, value); }
        }

        public static DependencyProperty valueFillProperty =
            DependencyProperty.Register("valueFill", typeof(Brush), typeof(WideTank), new FrameworkPropertyMetadata(Brushes.CadetBlue, FrameworkPropertyMetadataOptions.AffectsRender, tankPropertyCallback));

        public Brush valueFill
        {
            get { return (Brush)GetValue(valueFillProperty); }
            set { SetValue(valueFillProperty, value); }
        }

        public static DependencyProperty alarmMinProperty =
           DependencyProperty.Register("alarmMin", typeof(double), typeof(WideTank), new FrameworkPropertyMetadata(0d, FrameworkPropertyMetadataOptions.AffectsRender, tankPropertyCallback));

        public double alarmMin
        {
            get { return (double)GetValue(alarmMinProperty); }
            set { SetValue(alarmMinProperty, value); }
        }

        public static DependencyProperty alarmMaxProperty =
           DependencyProperty.Register("alarmMax", typeof(double), typeof(WideTank), new FrameworkPropertyMetadata(0d, FrameworkPropertyMetadataOptions.AffectsRender, tankPropertyCallback));

        public double alarmMax
        {
            get { return (double)GetValue(alarmMaxProperty); }
            set { SetValue(alarmMaxProperty, value); }
        }

        public static DependencyProperty faultMinProperty =
           DependencyProperty.Register("faultMin", typeof(double), typeof(WideTank), new FrameworkPropertyMetadata(0d, FrameworkPropertyMetadataOptions.AffectsRender, tankPropertyCallback));

        public double faultMin
        {
            get { return (double)GetValue(faultMinProperty); }
            set { SetValue(faultMinProperty, value); }
        }

        public static DependencyProperty faultMaxProperty =
           DependencyProperty.Register("faultMax", typeof(double), typeof(WideTank), new FrameworkPropertyMetadata(0d, FrameworkPropertyMetadataOptions.AffectsRender, tankPropertyCallback));

        public double faultMax
        {
            get { return (double)GetValue(faultMaxProperty); }
            set { SetValue(faultMaxProperty, value); }
        }
    }
}
