using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace NGO.Elements.MSAB
{
    /// <summary>
    /// Логика взаимодействия для GasView.xaml
    /// </summary>
    public partial class GasView : UserControl
    {
        public GasView()
        {
            InitializeComponent();
        }

        public static DependencyProperty valueProperty =
            DependencyProperty.Register("value", typeof(double), typeof(GasView), new FrameworkPropertyMetadata(0d, FrameworkPropertyMetadataOptions.AffectsRender));

        //[Category(Helper.SE_CATEGORY_ATTRIBUTE)]
        public double value
        {
            get { return (double)GetValue(valueProperty); }
            set { SetValue(valueProperty, value); }
        }

        public static DependencyProperty maximumProperty =
            DependencyProperty.Register("maximum", typeof(double), typeof(GasView), new FrameworkPropertyMetadata(100d, FrameworkPropertyMetadataOptions.AffectsRender));

        //[Category(Helper.SE_CATEGORY_ATTRIBUTE)]
        public double maximum
        {
            get { return (double)GetValue(maximumProperty); }
            set { SetValue(maximumProperty, value); }
        }

        public static DependencyProperty minimumProperty =
            DependencyProperty.Register("minimum", typeof(double), typeof(GasView), new FrameworkPropertyMetadata(0d, FrameworkPropertyMetadataOptions.AffectsRender));

        //[Category(Helper.SE_CATEGORY_ATTRIBUTE)]
        public double minimum
        {
            get { return (double)GetValue(minimumProperty); }
            set { SetValue(minimumProperty, value); }
        }

        public static DependencyProperty majorTickCostProperty =
            DependencyProperty.Register("majorTickCost", typeof(double), typeof(GasView), new FrameworkPropertyMetadata(10d, FrameworkPropertyMetadataOptions.AffectsRender));

        //[Category(Helper.SE_CATEGORY_ATTRIBUTE)]
        public double majorTickCost
        {
            get { return (double)GetValue(majorTickCostProperty); }
            set { SetValue(majorTickCostProperty, value); }
        }

        public static DependencyProperty minorTickCostProperty =
            DependencyProperty.Register("minorTickCost", typeof(double), typeof(GasView), new FrameworkPropertyMetadata(5d, FrameworkPropertyMetadataOptions.AffectsRender));

        //[Category(Helper.SE_CATEGORY_ATTRIBUTE)]
        public double minorTickCost
        {
            get { return (double)GetValue(minorTickCostProperty); }
            set { SetValue(minorTickCostProperty, value); }
        }

        public static DependencyProperty valueFillProperty =
            DependencyProperty.Register("valueFill", typeof(Brush), typeof(GasView), new FrameworkPropertyMetadata(Brushes.CadetBlue, FrameworkPropertyMetadataOptions.AffectsRender));

        //[Category(Helper.SE_CATEGORY_ATTRIBUTE)]

        public Brush valueFill
        {
            get { return (Brush)GetValue(valueFillProperty); }
            set { SetValue(valueFillProperty, value); }
        }

        public static DependencyProperty unitsProperty =
            DependencyProperty.Register("units", typeof(string), typeof(GasView), new FrameworkPropertyMetadata("%", FrameworkPropertyMetadataOptions.AffectsRender));

        //[Category(Helper.SE_CATEGORY_ATTRIBUTE)]

        public string units
        {
            get { return (string)GetValue(unitsProperty); }
            set { SetValue(unitsProperty, value); }
        }
        
    }
}
