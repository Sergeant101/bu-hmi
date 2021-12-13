using System.Windows;
using System.Windows.Controls;

namespace VFD.Frames.Sub.SNG_Obuchenie
{
    public partial class FrmSettingsWeight : UserControl
    {
        public FrmSettingsWeight()
        {
            InitializeComponent();
        }

        //private void ComboBox_SelectionChanged(object sender, SelectionChangedEventArgs e)
        //{
        //    var result = MessageBox.Show("ВНИМАНИЕ!", "ВЫ УВЕРЕНЫ ЧТО ХОТИТЕ ИЗМЕНИТЬ ДАТЧИК ВЕСА?", MessageBoxButton.YesNo, MessageBoxImage.Question);
        //    if (result == MessageBoxResult.Yes)
        //    {
        //    }
        //}

        //private void ComboBox_Unloaded(object sender, RoutedEventArgs e)
        //{
        //    weight_sensor_type.SelectionChanged -= ComboBox_SelectionChanged;
        //}
    }
}
