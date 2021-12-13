using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using Monokot.ScadaExtension.WpfComponents.Utils;
using OxyPlot;

namespace NGO.Elements.Special.Calibration
{
    public partial class Calibration : UserControl
    {
        public Calibration()
        {
            InitializeComponent();
            _calibrationModel = new CalibrationViewModel();
            root.DataContext = _calibrationModel;
            SetValue(pointsProperty, new PropertyChangedObservableCollection<CalibrationPoint>());
        }

        public static DependencyProperty pointsProperty =
            DependencyProperty.Register("points", typeof(PropertyChangedObservableCollection<CalibrationPoint>), typeof(Calibration), new FrameworkPropertyMetadata(null, FrameworkPropertyMetadataOptions.AffectsRender, propertyChangedCallback));

        
        public PropertyChangedObservableCollection<CalibrationPoint> points
        {
            get { return (PropertyChangedObservableCollection<CalibrationPoint>)GetValue(pointsProperty); }
            set { SetValue(pointsProperty, value); }
        }


        public static DependencyProperty titleAxisXProperty =
            DependencyProperty.Register("titleAxisX", typeof(string), typeof(Calibration), new FrameworkPropertyMetadata("Title X", FrameworkPropertyMetadataOptions.AffectsRender, propertyChangedCallback));

        public string titleAxisX
        {
            get { return (string)GetValue(titleAxisXProperty); }

            set { SetValue(titleAxisXProperty, value); }
        }

        public static DependencyProperty titleAxisYProperty =
            DependencyProperty.Register("titleAxisY", typeof(string), typeof(Calibration), new FrameworkPropertyMetadata("Title Y", FrameworkPropertyMetadataOptions.AffectsRender, propertyChangedCallback));

        public string titleAxisY
        {
            get { return (string)GetValue(titleAxisYProperty); }
            set { SetValue(titleAxisYProperty, value); }
        }

        public static DependencyProperty maximumXProperty =
            DependencyProperty.Register("maximumX", typeof(double), typeof(Calibration), new FrameworkPropertyMetadata(20d, FrameworkPropertyMetadataOptions.AffectsRender, propertyChangedCallback));

        public double maximumX
        {
            get { return (double)GetValue(maximumXProperty); }

            set { SetValue(maximumXProperty, value); }
        }

        public static DependencyProperty minimumXProperty =
            DependencyProperty.Register("minimumX", typeof(double), typeof(Calibration), new FrameworkPropertyMetadata(0d, FrameworkPropertyMetadataOptions.AffectsRender, propertyChangedCallback));

        public double minimumX
        {
            get { return (double)GetValue(minimumXProperty); }

            set { SetValue(minimumXProperty, value); }
        }

        public static DependencyProperty maximumYProperty =
            DependencyProperty.Register("maximumY", typeof(double), typeof(Calibration), new FrameworkPropertyMetadata(100d, FrameworkPropertyMetadataOptions.AffectsRender, propertyChangedCallback));

        public double maximumY
        {
            get { return (double)GetValue(maximumYProperty); }

            set { SetValue(maximumYProperty, value); }
        }

        public static DependencyProperty minimumYProperty =
            DependencyProperty.Register("minimumY", typeof(double), typeof(Calibration), new FrameworkPropertyMetadata(0d, FrameworkPropertyMetadataOptions.AffectsRender, propertyChangedCallback));

        public double minimumY
        {
            get { return (double)GetValue(minimumYProperty); }

            set { SetValue(minimumYProperty, value); }
        }


        public static DependencyProperty majorStepXProperty =
            DependencyProperty.Register("majorStepX", typeof(double), typeof(Calibration), new FrameworkPropertyMetadata(5d, FrameworkPropertyMetadataOptions.AffectsRender, propertyChangedCallback));

        public double majorStepX
        {
            get { return (double)GetValue(majorStepXProperty); }

            set { SetValue(majorStepXProperty, value); }
        }

        public static DependencyProperty majorStepYProperty =
            DependencyProperty.Register("majorStepY", typeof(double), typeof(Calibration), new FrameworkPropertyMetadata(10d, FrameworkPropertyMetadataOptions.AffectsRender, propertyChangedCallback));

        public double majorStepY
        {
            get { return (double)GetValue(majorStepYProperty); }

            set { SetValue(majorStepYProperty, value); }
        }



        public static DependencyProperty axisXvisibilityProperty =
            DependencyProperty.Register("axisXvisibility", typeof(Visibility), typeof(Calibration), new FrameworkPropertyMetadata(Visibility.Visible, FrameworkPropertyMetadataOptions.AffectsRender, propertyChangedCallback));

        public Visibility axisXvisibility
        {
            get { return (Visibility) GetValue(axisXvisibilityProperty); }
            set { SetValue(axisXvisibilityProperty, value); }
        }

        public static DependencyProperty axisYvisibilityProperty =
           DependencyProperty.Register("axisYvisibility", typeof(Visibility), typeof(Calibration), new FrameworkPropertyMetadata(Visibility.Visible, FrameworkPropertyMetadataOptions.AffectsRender, propertyChangedCallback));

        public Visibility axisYvisibility
        {
            get { return (Visibility)GetValue(axisYvisibilityProperty); }
            set { SetValue(axisYvisibilityProperty, value); }
        }



        public static DependencyProperty axisXcolorProperty =
            DependencyProperty.Register("axisXcolor", typeof(Color), typeof(Calibration), new FrameworkPropertyMetadata(Colors.Black, FrameworkPropertyMetadataOptions.AffectsRender, propertyChangedCallback));

        public Color axisXcolor
        {
            get { return (Color)GetValue(axisXcolorProperty); }
            set { SetValue(axisXcolorProperty, value); }
        }


        public static DependencyProperty axisYcolorProperty =
           DependencyProperty.Register("axisYcolor", typeof(Color), typeof(Calibration), new FrameworkPropertyMetadata(Colors.Black, FrameworkPropertyMetadataOptions.AffectsRender, propertyChangedCallback));

        public Color axisYcolor
        {
            get { return (Color)GetValue(axisYcolorProperty); }
            set { SetValue(axisYcolorProperty, value); }
        }


        //public static DependencyProperty writeCommandProperty =
        //    DependencyProperty.Register("writeCommand", typeof(ICommand), typeof(Calibration), new FrameworkPropertyMetadata(null, propertyChangedCallback));

        //[Category(Helper.SE_CATEGORY_ATTRIBUTE)]
        //public ICommand writeCommand
        //{
        //    get { return (ICommand)GetValue(writeCommandProperty); }
        //    set { SetValue(writeCommandProperty, value); }
        //}

        //public static DependencyProperty mulfactorXProperty =
        //    DependencyProperty.Register("mulfactorX", typeof(double), typeof(Calibration), new FrameworkPropertyMetadata(1d, propertyChangedCallback));

        //[Category(Helper.SE_CATEGORY_ATTRIBUTE)]
        //public double mulfactorX
        //{
        //    get { return (double)GetValue(mulfactorXProperty); }
        //    set { SetValue(mulfactorXProperty, value); }
        //}

        //public static DependencyProperty mulfactorYProperty =
        //    DependencyProperty.Register("mulfactorY", typeof(double), typeof(Calibration), new FrameworkPropertyMetadata(1d, propertyChangedCallback));

        //[Category(Helper.SE_CATEGORY_ATTRIBUTE)]
        //public double mulfactorY
        //{
        //    get { return (double)GetValue(mulfactorYProperty); }
        //    set { SetValue(mulfactorYProperty, value); }
        //}

        private readonly CalibrationViewModel _calibrationModel;

        public static DependencyProperty filenameProperty =
            DependencyProperty.Register("filename", typeof(string), typeof(Calibration), new FrameworkPropertyMetadata("wdef0.txt", propertyChangedCallback));

        public string filename
        {
            get { return (string)GetValue(filenameProperty); }
            set { SetValue(filenameProperty, value); }
        }

        private static void propertyChangedCallback(DependencyObject dep_obj, DependencyPropertyChangedEventArgs dp_args)
        {
            var calibration = (Calibration)dep_obj;
            if (calibration._calibrationModel == null) return;
            var newVal = dp_args.NewValue;

            if (dp_args.Property.Name=="points")
                if (newVal is PropertyChangedObservableCollection<CalibrationPoint>)
                    calibration._calibrationModel.points = newVal as PropertyChangedObservableCollection<CalibrationPoint>;

            
            if (dp_args.Property.Name == "filename")
                calibration._calibrationModel.filename = newVal.ToString();

            if (dp_args.Property.Name == "titleAxisX")
                calibration._calibrationModel.titeX = newVal.ToString();
            if (dp_args.Property.Name == "titleAxisY")
                calibration._calibrationModel.titeY = newVal.ToString();

            if (dp_args.Property.Name == "maximumX")
                if (newVal is double)
                    calibration._calibrationModel.maxX = (double)newVal;
            if (dp_args.Property.Name == "minimumX")
                if (newVal is double)
                    calibration._calibrationModel.minX = (double)newVal;
            if (dp_args.Property.Name == "maximumY")
                if (newVal is double)
                    calibration._calibrationModel.maxY = (double)newVal;
            if (dp_args.Property.Name == "minimumY")
                if (newVal is double)
                    calibration._calibrationModel.minY = (double)newVal;
            if (dp_args.Property.Name == "majorStepX")
                if (newVal is double)
                    calibration._calibrationModel.majorStepX = (double)newVal;
            if (dp_args.Property.Name == "majorStepY")
                if (newVal is double)
                    calibration._calibrationModel.majorStepY = (double)newVal;
            if (dp_args.Property.Name == "axisYcolor")
                if (newVal is Color)
                    calibration._calibrationModel.yTextColor = OxyColor.Parse(newVal.ToString());
            if (dp_args.Property.Name == "axisXcolor")
                if (newVal is Color)
                    calibration._calibrationModel.xTextColor = OxyColor.Parse(newVal.ToString());
        }

        private void WriteToPLCButton_Click(object sender, RoutedEventArgs e)
        {
            var w = Window.GetWindow(this);
            if (w != null) w.Close();
        }

        private void CancelButton_Click(object sender, RoutedEventArgs e)
        {
            var w = Window.GetWindow(this);
            if (w != null) w.Close();
        }

        private void _uc_Loaded(object sender, RoutedEventArgs e)
        {
            //foreach (var point in points)
            //    point.DataContext = DataContext;
        }
    }
}
