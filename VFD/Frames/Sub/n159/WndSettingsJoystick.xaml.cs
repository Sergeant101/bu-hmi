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

namespace VFD.Frames.Sub.n159
{
    /// <summary>
    /// Interaction logic for WndSettingsJoystick.xaml
    /// </summary>
    public partial class WndSettingsJoystick : Window
    {
        public WndSettingsJoystick()
        {
            InitializeComponent();
        }

        private void Back_Click(object sender, RoutedEventArgs e)
        {
            Close();
        }
    }
}
