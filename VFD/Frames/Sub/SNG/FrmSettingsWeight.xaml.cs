using System.Diagnostics;
using System.Windows;
using System.Windows.Controls;
using NGO.Elements.Special.Calibration;

namespace VFD.Frames.Sub.SNG
{
    /// <summary>
    /// Логика взаимодействия для FrmWeightSetup.xaml
    /// </summary>
    public partial class FrmSettingsWeight : UserControl
    {
        public FrmSettingsWeight()
        {
            InitializeComponent();
        }

        //private void Button_Click(object sender, RoutedEventArgs e)
        //{
        //    var weightSetup = new CalibrationInput0 { DataContext = DataContext };

        //    weightSetup.ShowDialog();
        //    if (weightSetup.DialogResult == true)
        //        Trace.WriteLine("Произведена настройка датчика веса!");
        //}

        //private void Button_Click_1(object sender, RoutedEventArgs e)
        //{
        //    var weightSetup = new CalibrationInput1 { DataContext = DataContext };

        //    weightSetup.ShowDialog();
        //    if (weightSetup.DialogResult == true)
        //        Trace.WriteLine("Произведена настройка датчика веса!");
        //}
    }
}
