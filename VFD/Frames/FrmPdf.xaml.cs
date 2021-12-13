using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.IO;
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
using NGO.Elements.Special.Pdf;

namespace VFD.Frames
{
    /// <summary>
    /// Логика взаимодействия для FrmPdf.xaml
    /// </summary>
    public partial class FrmPdf : UserControl
    {
        public FrmPdf()
        {
            InitializeComponent();
            if (DesignerProperties.GetIsInDesignMode(this))
            {
                return;
            }

            var uri = new Uri(System.Reflection.Assembly.GetExecutingAssembly().GetName().CodeBase);
            var tmp = Path.GetDirectoryName(uri.LocalPath);
            var location = tmp + @"\data\pdf\";
            var lst = new List<PdfHelper>() {new PdfHelper(Pdf) {path = location}};
            DataContext = lst;
        }

        private bool _treeIsHide = true;

        private void Button_Click_2(object sender, RoutedEventArgs e)
        {
            var image = btnHideCatalogs.Content as Image;

            switch (_treeIsHide)
            {
                case true:
                    CatalogsView.Visibility = Visibility.Collapsed;
                    if (image != null)
                        image.Source = new BitmapImage(new Uri(@"pack://application:,,,/NGO.Elements;component/Content/Icon/arrow (right).png"));
                    break;
                default:
                    CatalogsView.Visibility = Visibility.Visible;
                    if (image != null)
                        image.Source = new BitmapImage(new Uri(@"pack://application:,,,/NGO.Elements;component/Content/Icon/arrow (left).png"));
                    break;
            }
            _treeIsHide = !_treeIsHide;
        }
    }
}
