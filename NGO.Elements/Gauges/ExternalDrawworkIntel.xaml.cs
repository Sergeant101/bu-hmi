using System.Windows;
using System.Windows.Controls;

namespace NGO.Elements.Gauges
{
    /// <summary>
    /// Interaction logic for ExternalDrawworkIntel.xaml
    /// </summary>
    public partial class ExternalDrawworkIntel : UserControl
    {
        public ExternalDrawworkIntel()
        {
            InitializeComponent();
        }

        public static readonly DependencyProperty ButtonsRowHeightProperty =
          DependencyProperty.Register(
              nameof(ButtonsRowHeight), 
              typeof(GridLength), 
              typeof(ExternalDrawworkIntel), 
              new FrameworkPropertyMetadata(new GridLength(120, GridUnitType.Pixel))
              );


        public GridLength ButtonsRowHeight
        {
            get { return (GridLength)GetValue(ButtonsRowHeightProperty); }
            set { SetValue(ButtonsRowHeightProperty, value); }
        }

        public static readonly DependencyProperty ButtonsRowVisibilityProperty =
          DependencyProperty.Register(nameof(ButtonsRowVisibility), typeof(Visibility), typeof(ExternalDrawworkIntel), new FrameworkPropertyMetadata(Visibility.Visible));


        public Visibility ButtonsRowVisibility
        {
            get { return (Visibility)GetValue(ButtonsRowVisibilityProperty); }
            set { SetValue(ButtonsRowVisibilityProperty, value); }
        }
    }
}
