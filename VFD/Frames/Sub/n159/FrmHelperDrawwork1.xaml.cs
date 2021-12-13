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
    /// Логика взаимодействия для HelperDrawworkSNG.xaml
    /// </summary>
    public partial class FrmHelperDrawwork1 : UserControl
    {
        public FrmHelperDrawwork1()
        {
            InitializeComponent();
        }

        private void button1_Click(object sender, RoutedEventArgs e)
        {
            var wnd = new WndSettingsJoystick() { WindowStartupLocation = WindowStartupLocation.CenterScreen };
            wnd.ShowDialog();
        }
    }
}
