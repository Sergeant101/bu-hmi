using System;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.IO;
using System.Reflection;
using System.Windows;
using System.Windows.Input;
using Monokot.Hmi.Wpf.Utils;
using Monokot.ScadaExtension.WpfComponents.Utils;
using Newtonsoft.Json;
using NGO.Elements.Annotations;
using OxyPlot;
using OxyPlot.Axes;
using OxyPlot.Series;

namespace NGO.Elements.Special.Calibration
{
    public class CalibrationViewModel : INotifyPropertyChanged
    {
        public CalibrationViewModel()
        {
            plotModel = new PlotModel();
            var axisX = new LinearAxis { Position =AxisPosition.Bottom, MajorGridlineStyle = LineStyle.Solid, MinorGridlineStyle = LineStyle.Dot, Title = titeX, IntervalLength = 80, Maximum = maxX, Minimum = minX, TitleFontSize = 16d, TitleFont = "Tahoma", FontSize = 16d, TitleColor = xTextColor, TextColor = xTextColor };
            plotModel.Axes.Add(axisX);
            var axisY = new LinearAxis { Position = AxisPosition.Left, MajorGridlineStyle = LineStyle.Solid, MinorGridlineStyle = LineStyle.Dot, Title = titeY, Maximum = maxY, Minimum = minY, TitleFontSize = 0d, TitleFont = "Tahoma", FontSize = 16d, TitleColor = yTextColor, TextColor = yTextColor };
            plotModel.Axes.Add(axisY);

            plotModel.Series.Add(_oxyLineSeries);

            saveFileCmd = new RelayCommand(savePreferenceFile);
            loadFileCmd = new RelayCommand(loadPreferenceFile);

            //writeCmd = new RelayCommand(write);
        }

        private string _filename = "wdef0.txt";
        private string _titeX = "Ток, мА";
        private string _titeY = "Вес, т";
        private double _maxY = 100d;
        private double _minY = 0d;
        private double _maxX = 20d;
        private double _minX = 0d;
        private double _majorStepX = 5d;
        private double _majorStepY = 10d;

        private OxyColor _xTextColor = OxyColors.Black;
        private OxyColor _yTextColor = OxyColors.Black;


        private readonly LineSeries _oxyLineSeries = new LineSeries { Color = OxyColors.Red, StrokeThickness = 2 };
        private PropertyChangedObservableCollection<CalibrationPoint> _points = new PropertyChangedObservableCollection<CalibrationPoint>();

        //private double _mulfactorX = 1d;
        //private double _mulfactorY = 1d;
        //public ICommand writeCmd { get; set; }
        //public ICommand writeValuesCmd { get; set; }
        public ICommand saveFileCmd { get; set; }
        public ICommand loadFileCmd { get; set; }
        //private object _feedbackArray;
        private PlotModel _plotModel;
        public event PropertyChangedEventHandler PropertyChanged;


        public PropertyChangedObservableCollection<CalibrationPoint> points
        {
            get { return _points; }
            set
            {
                if (Equals(value, _points)) return;

                _points.ItemPropertyChanged -= point_PropertyChanged;
                _points = value;
                _points.ItemPropertyChanged += point_PropertyChanged;
                onPropertyChanged("points");
            }
        }

        void point_PropertyChanged(object sender, PropertyChangedEventArgs e)
        {
            refreshPlot();
        }

        public PlotModel plotModel
        {
            get { return _plotModel; }
            set
            {
                if (Equals(value, _plotModel)) return;
                _plotModel = value;
                onPropertyChanged("plotModel");
            }
        }

        public string filename
        {
            get { return _filename; }
            set { _filename = value; }
        }

        public string titeX
        {
            get { return _titeX; }
            set
            {
                if (value.Equals(_titeX)) return;
                _titeX = value;
                plotModel.Axes[0].Title = value;
                onPropertyChanged("titeX");
                plotModel.InvalidatePlot(true);
            }
        }

        public string titeY
        {
            get { return _titeY; }
            set
            {
                if (value.Equals(_titeY)) return;
                _titeY = value;
                plotModel.Axes[1].Title = value;
                onPropertyChanged("titeY");
                plotModel.InvalidatePlot(true);
            }
        }


        public double maxX
        {
            get { return _maxX; }
            set
            {
                if (value.Equals(maxX)) return;
                _maxX = value;
                plotModel.Axes[0].Maximum = value;
                onPropertyChanged("maxX");
                plotModel.InvalidatePlot(true);
            }
        }

        public double minX
        {
            get { return _minX; }
            set
            {
                if (value.Equals(minX)) return;
                _maxX = value;
                plotModel.Axes[0].Minimum = value;
                onPropertyChanged("minX");
                plotModel.InvalidatePlot(true);
            }
        }

        public double maxY
        {
            get { return _maxY; }
            set
            {
                if (value.Equals(maxY)) return;
                _maxY = value;
                plotModel.Axes[1].Maximum = value;
                onPropertyChanged("maxY");
                plotModel.InvalidatePlot(true);
            }
        }

        public double minY
        {
            get { return _minY; }
            set
            {

                if (value.Equals(minY)) return;
                _minY = value;
                plotModel.Axes[1].Minimum = value;
                onPropertyChanged("minY");
                plotModel.InvalidatePlot(true);

            }
        }

        public double majorStepX
        {
            get { return _majorStepX; }
            set
            {
                if (value.Equals(majorStepX)) return;
                _majorStepX = value;
                plotModel.Axes[0].MajorStep = value;
                onPropertyChanged("majorStepX");
                plotModel.InvalidatePlot(true);
            }
        }

        public double majorStepY
        {
            get { return _majorStepY; }
            set
            {
                if (value.Equals(majorStepY)) return;
                _majorStepY = value;
                plotModel.Axes[1].MajorStep = value;
                onPropertyChanged("majorStepY");
                plotModel.InvalidatePlot(true);
            }
        }

        public OxyColor xTextColor
        {
            get { return _xTextColor; }
            set
            {
                if (value.Equals(xTextColor)) return;
                _xTextColor = value;
                plotModel.Axes[0].TextColor = value;
                plotModel.Axes[0].TitleColor= value;
                onPropertyChanged("xTextColor");
                plotModel.InvalidatePlot(true);

            }
        }

        public OxyColor yTextColor
        {
            get { return _yTextColor; }
            set
            {
                if (value.Equals(yTextColor)) return;
                _yTextColor = value;
                plotModel.Axes[1].TextColor = value;
                plotModel.Axes[1].TitleColor= value;
                onPropertyChanged("yTextColor");
                plotModel.InvalidatePlot(true);


            }
        }

        public void savePreferenceFile(object o)
        {

            var p = new ObservableCollection<Point>();
            foreach (var point in points)
                p.Add(new Point(point.x * point.xMulFactor, point.y * point.yMulFactor));

            var output = JsonConvert.SerializeObject(p);
            var location = Path.GetDirectoryName(Assembly.GetEntryAssembly().Location) + @"\data\" + filename;

            try
            {
                File.WriteAllText(location, output);
            }
            catch (Exception x)
            {
                //Helper.exceptionReport(x);
            }
        }

        public void loadPreferenceFile(object o)
        {
            ObservableCollection<Point> loadedPoints;
            var location = Path.GetDirectoryName(Assembly.GetEntryAssembly().Location) + @"\data\" + filename;

            try
            {
                loadedPoints = JsonConvert.DeserializeObject<ObservableCollection<Point>>(File.ReadAllText(location));
            }
            catch (Exception x)
            {
                //Helper.exceptionReport(x);
                return;
            }

            if (loadedPoints.Count < points.Count)
            {
                //Helper.diagnosticReport("В файле " + location + " допущена ошибка при формировании настроек, пересохраните файл!");
                return;
            }

            for (var i = 0; i < loadedPoints.Count; i++)
            {
                if (points[i].xWrite != null)
                    points[i].xWrite.Execute(loadedPoints[i].X);
                if (points[i].yWrite != null)
                    points[i].yWrite.Execute(loadedPoints[i].Y);
            }
        }

        //public void write(object o)
        //{
        //    if (writeValuesCmd == null || !writeValuesCmd.CanExecute(null)) return;

        //    var array = new List<double>();
        //    foreach (var point in points)
        //    {
        //        array.Add(point.x * mulfactorX);
        //        array.Add(point.y * mulfactorY);
        //    }
        //    writeValuesCmd.Execute(array.ToArray());
        //}

        private void refreshPlot()
        {
            _oxyLineSeries.Points.Clear();
            plotModel.Series.Clear();
            foreach (var point in points)
                _oxyLineSeries.Points.Add(new DataPoint(point.x, point.y));

            plotModel.Series.Add(_oxyLineSeries);
            plotModel.InvalidatePlot(true);
        }

        [NotifyPropertyChangedInvocator]
        protected virtual void onPropertyChanged(string property_name)
        {
            PropertyChangedEventHandler handler = PropertyChanged;
            if (handler != null) handler(this, new PropertyChangedEventArgs(property_name));
        }
    }

    public class CalibrationPoint : FrameworkElement, INotifyPropertyChanged
    {
        public static DependencyProperty xProperty =
            DependencyProperty.Register("x", typeof(double), typeof(CalibrationPoint), new FrameworkPropertyMetadata(0d));

        public double x
        {
            get { return (double)GetValue(xProperty); }
            set { SetValue(xProperty, value); }
        }

        public static DependencyProperty yProperty =
            DependencyProperty.Register("y", typeof(double), typeof(CalibrationPoint), new FrameworkPropertyMetadata(0d));

        public double y
        {
            get { return (double)GetValue(yProperty); }
            set { SetValue(yProperty, value); }
        }

        public static DependencyProperty xEditorVisibleProperty =
            DependencyProperty.Register("xEditorVisible", typeof(Visibility), typeof(CalibrationPoint), new FrameworkPropertyMetadata(Visibility.Visible));

        protected override void OnPropertyChanged(DependencyPropertyChangedEventArgs e)
        {
            base.OnPropertyChanged(e);
            onPropertyChanged(e.Property.Name);
        }

        public Visibility xEditorVisible
        {
            get { return (Visibility)GetValue(xEditorVisibleProperty); }
            set { SetValue(xEditorVisibleProperty, value); }
        }

        public static DependencyProperty yEditorVisibleProperty =
            DependencyProperty.Register("yEditorVisible", typeof(Visibility), typeof(CalibrationPoint), new FrameworkPropertyMetadata(Visibility.Visible));

        public Visibility yEditorVisible
        {
            get { return (Visibility)GetValue(yEditorVisibleProperty); }
            set { SetValue(yEditorVisibleProperty, value); }
        }

        public static DependencyProperty xWriteProperty =
            DependencyProperty.Register("xWrite", typeof(ICommand), typeof(CalibrationPoint), null);
        public ICommand xWrite
        {
            get { return (ICommand)GetValue(xWriteProperty); }
            set { SetValue(xWriteProperty, value); }
        }


        public static DependencyProperty yWriteProperty =
            DependencyProperty.Register("yWrite", typeof(ICommand), typeof(CalibrationPoint), null);

        public ICommand yWrite
        {
            get { return (ICommand)GetValue(yWriteProperty); }
            set { SetValue(yWriteProperty, value); }
        }


        public static DependencyProperty xMulFactorProperty =
            DependencyProperty.Register("xMulFactor", typeof(double), typeof(CalibrationPoint), new FrameworkPropertyMetadata(1d));

        public double xMulFactor
        {
            get { return (double)GetValue(xMulFactorProperty); }
            set { SetValue(xMulFactorProperty, value); }
        }

        public static DependencyProperty yMulFactorProperty =
            DependencyProperty.Register("yMulFactor", typeof(double), typeof(Calibration), new FrameworkPropertyMetadata(1d));

        public double yMulFactor
        {
            get { return (double)GetValue(yMulFactorProperty); }
            set { SetValue(yMulFactorProperty, value); }
        }

        public event PropertyChangedEventHandler PropertyChanged;

        [NotifyPropertyChangedInvocator]
        protected virtual void onPropertyChanged(string property_name)
        {
            var handler = PropertyChanged;
            if (handler != null) handler(this, new PropertyChangedEventArgs(property_name));
        }
    }
}
