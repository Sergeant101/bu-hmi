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

namespace VFD.Frames.Sub.n159
{
    /// <summary>
    /// Interaction logic for FrmSetup.xaml
    /// </summary>
    public partial class FrmSetup : UserControl
    {
        public FrmSetup()
        {
            InitializeComponent();
        }

        private void Button_Click(object sender, RoutedEventArgs e)
        {
            var wnd = new NGO.Elements.Special.Calibration.CalibrationInput0() { WindowStartupLocation = WindowStartupLocation.CenterScreen };
            wnd.ShowDialog();
        }
    }
}
