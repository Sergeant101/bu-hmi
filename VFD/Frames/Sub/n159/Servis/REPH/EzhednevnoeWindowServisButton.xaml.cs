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
using VFD.Frames.Sub.n159.Servis;
using System.Windows.Shapes;

namespace VFD.Frames.Sub.n159.Servis.REPH
{
    /// <summary>
    /// Interaction logic for WindowServisButton.xaml
    /// </summary>
    public partial class EzhednevnoeWindowServisButton
    {
        public EzhednevnoeWindowServisButton()
        {
            InitializeComponent();
        }

        private void Back_Click(object sender, RoutedEventArgs e)
        {
            Close();
        }

        private void Button_Click(object sender, RoutedEventArgs e)
        {
            var wnd = new Journal(Journal.ServisoJurnalTime.Day);
            wnd.ShowDialog();
        }
    }
}
