using System;
using System.Collections.Generic;
using System.Linq;
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

namespace NGO.Elements.Scheme
{
    /// <summary>
    /// Логика взаимодействия для QF.xaml
    /// </summary>
    public partial class QF : UserControl
    {
        public QF()
        {
            InitializeComponent();
        }

        public static DependencyProperty fillProperty =
            DependencyProperty.Register("fill", typeof(Brush), typeof(QF),
            new FrameworkPropertyMetadata(Brushes.Black, FrameworkPropertyMetadataOptions.AffectsRender, propertyChangedCallback));
        
        public Brush fill
        {
            get { return (Brush)GetValue(fillProperty); }

            set { SetValue(fillProperty, value); }
        }

        public static DependencyProperty electricTypeProperty =
            DependencyProperty.Register("electricType", typeof(ElectricType), typeof(QF),
            new FrameworkPropertyMetadata(ElectricType.QF, FrameworkPropertyMetadataOptions.AffectsRender, propertyChangedCallback));
        public ElectricType electricType
        {
            get { return (ElectricType)GetValue(electricTypeProperty); }

            set { SetValue(electricTypeProperty, value); }
        }

        public static DependencyProperty isOpenProperty =
            DependencyProperty.Register("isOpen", typeof(bool), typeof(QF),
                new FrameworkPropertyMetadata(false, FrameworkPropertyMetadataOptions.AffectsRender,
                    propertyChangedCallback));

        public bool isOpen
        {
            get { return (bool)GetValue(isOpenProperty); }
            set { SetValue(isOpenProperty, value); }
        }

        private static void propertyChangedCallback(DependencyObject dep_obj,
            DependencyPropertyChangedEventArgs dependency_property_changed_event_args)
        {
            var sw = (QF)dep_obj;
            if (sw != null)
            {


                if (sw.electricType == ElectricType.QF)
                {

                    sw.qs_close.Visibility=Visibility.Hidden;
                    sw.qs_open.Visibility = Visibility.Hidden;
                    
                    if (sw.isOpen)
                    {
                        sw.qf_open.Visibility = Visibility.Visible;
                        sw.qf_close.Visibility = Visibility.Hidden;

                    }
                    else
                    {
                        sw.qf_open.Visibility = Visibility.Hidden;
                        sw.qf_close.Visibility = Visibility.Visible;
                    }
                }

                if (sw.electricType == ElectricType.QS)
                {
                    sw.qf_close.Visibility = Visibility.Hidden;
                    sw.qf_open.Visibility = Visibility.Hidden;

                    if (sw.isOpen)
                    {
                        sw.qs_open.Visibility = Visibility.Visible;
                        sw.qs_close.Visibility = Visibility.Hidden;

                    }
                    else
                    {
                        sw.qs_open.Visibility = Visibility.Hidden;
                        sw.qs_close.Visibility = Visibility.Visible;
                    }
                }
            }
        }

        public enum ElectricType
        {
            QF,
            QS
        };
    }
}
