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
using System.Windows.Shapes;
using NGO.Elements.Special.ReadyWindow;

namespace NGO.Elements.Special.ReadyWindow
{
    /// <summary>
    /// Логика взаимодействия для ListWindow.xaml
    /// </summary>
    public partial class ListWindow
    {
        public ListWindow()
        {
            InitializeComponent();
        }

        public static readonly DependencyProperty readiesProperty = DependencyProperty.Register(
            "readies", typeof(TagsFromFileModel), typeof(ListWindow), new PropertyMetadata(default(TagsFromFileModel)));

        public TagsFromFileModel readies
        {
            get { return (TagsFromFileModel)GetValue(readiesProperty); }
            set { SetValue(readiesProperty, value); }
        }

        public static readonly DependencyProperty emergencyProperty = DependencyProperty.Register(
            "emergency", typeof(TagsFromFileModel), typeof(ListWindow), new PropertyMetadata(default(TagsFromFileModel)));

        public TagsFromFileModel emergency
        {
            get { return (TagsFromFileModel)GetValue(emergencyProperty); }
            set { SetValue(emergencyProperty, value); }
        }

        public static readonly DependencyProperty stopsProperty = DependencyProperty.Register(
            "stops", typeof(TagsFromFileModel), typeof(ListWindow), new PropertyMetadata(default(TagsFromFileModel)));

        public TagsFromFileModel stops
        {
            get { return (TagsFromFileModel)GetValue(stopsProperty); }
            set { SetValue(stopsProperty, value); }
        }

        public static readonly DependencyProperty emergenciesVisibleProperty = DependencyProperty.Register(
            "emergenciesVisible", typeof(bool), typeof(ListWindow), new PropertyMetadata(default(bool)));

        public bool emergenciesVisible
        {
            get { return (bool)GetValue(emergenciesVisibleProperty); }
            set { SetValue(emergenciesVisibleProperty, value); }
        }

        public static readonly DependencyProperty readiesVisibleProperty = DependencyProperty.Register(
            "readiesVisible", typeof(bool), typeof(ListWindow), new PropertyMetadata(default(bool)));

        public bool readiesVisible
        {
            get { return (bool)GetValue(readiesVisibleProperty); }
            set { SetValue(readiesVisibleProperty, value); }
        }

        public static readonly DependencyProperty stopsVisibleProperty = DependencyProperty.Register(
            "stopsVisible", typeof(bool), typeof(ListWindow), new PropertyMetadata(default(bool)));

        public bool stopsVisible
        {
            get { return (bool)GetValue(stopsVisibleProperty); }
            set { SetValue(stopsVisibleProperty, value); }
        }

        private void Button_Click(object sender, RoutedEventArgs e)
        {
            Visibility = Visibility.Collapsed;
        }
    }
}
