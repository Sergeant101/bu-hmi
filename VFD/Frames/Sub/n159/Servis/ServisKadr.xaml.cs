using System.Windows;
using System.Windows.Controls;

namespace VFD.Frames.Sub.n159.Servis
{
    /// <summary>
    /// Interaction logic for ServisKadr.xaml
    /// </summary>
    public partial class ServisKadr : UserControl
    {
        public ServisKadr()
        {
            InitializeComponent();
        }

        private void ezhesmennoe_click(object sender, RoutedEventArgs e)
        {
            var wnd = new EzhesmennoeWindowServisButton() { WindowStartupLocation=WindowStartupLocation.CenterScreen };
            wnd.ShowDialog();
        }

        private void ezhenedelnoe_click(object sender, RoutedEventArgs e)
        {
            var wnd = new EzhenedelnoeWindowServisButton() { WindowStartupLocation = WindowStartupLocation.CenterScreen };
            wnd.ShowDialog();
        }


        private void ezhemesyachnoe_click(object sender, RoutedEventArgs e)
        {
            var wnd = new EzhemesyachnoeWindowServisButton() { WindowStartupLocation = WindowStartupLocation.CenterScreen };
            wnd.ShowDialog();
        }

        private void ezhegodnoe_click(object sender, RoutedEventArgs e)
        {
            var wnd = new EzhegodnoeWindowServisButton() { WindowStartupLocation = WindowStartupLocation.CenterScreen };
            wnd.ShowDialog();
        }

        private void ezhednevnoe_ktu_click(object sender, RoutedEventArgs e)
        {
            //var wnd = new KTU.EzhednevnoeWindowServisButton() { WindowStartupLocation = WindowStartupLocation.CenterScreen };
            //wnd.ShowDialog();
        }

        private void ezhenedelnoe_ktu_click(object sender, RoutedEventArgs e)
        {
            //var wnd = new KTU.EzhenedelnoeWindowServisButton() { WindowStartupLocation = WindowStartupLocation.CenterScreen };
            //wnd.ShowDialog();
        }

        private void ezhemesyachnoe_ktu_click(object sender, RoutedEventArgs e)
        {
            //var wnd = new KTU.EzhemesyachnoeWindowServisButton() { WindowStartupLocation = WindowStartupLocation.CenterScreen };
            //wnd.ShowDialog();
        }

        private void ezhegodnoe_ktu_click(object sender, RoutedEventArgs e)
        {
            //var wnd = new KTU.EzhegodnoeWindowServisButton() { WindowStartupLocation = WindowStartupLocation.CenterScreen };
            //wnd.ShowDialog();
        }


        private void ezhednevnoe_reph_ktu_click(object sender, RoutedEventArgs e)
        {
            //var wnd = new KTU.REPH.EzhednevnoeWindowServisButton() { WindowStartupLocation = WindowStartupLocation.CenterScreen };
            //wnd.ShowDialog();
        }

        private void ezhenedelnoe_reph_ktu_click(object sender, RoutedEventArgs e)
        {
            //var wnd = new KTU.REPH.EzhenedelnoeWindowServisButton() { WindowStartupLocation = WindowStartupLocation.CenterScreen };
            //wnd.ShowDialog();
        }

        private void ezhemesyachnoe_reph_ktu_click(object sender, RoutedEventArgs e)
        {
            //var wnd = new KTU.REPH.EzhemesyachnoeWindowServisButton() { WindowStartupLocation = WindowStartupLocation.CenterScreen };
            //wnd.ShowDialog();
        }

        private void ezhegodnoe_reph_ktu_click(object sender, RoutedEventArgs e)
        {
            //var wnd = new KTU.REPH.EzhegodnoeWindowServisButton() { WindowStartupLocation = WindowStartupLocation.CenterScreen };
            //wnd.ShowDialog();
        }
    }
}
