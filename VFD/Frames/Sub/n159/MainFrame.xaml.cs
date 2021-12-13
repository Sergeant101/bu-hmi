using System.Windows;
using System.Windows.Controls;
using System.Reflection;
using Path = System.IO.Path;

namespace VFD.Frames.Sub.n159
{
    /// <summary>
    /// Interaction logic for MainFrame.xaml
    /// </summary>
    public partial class MainFrame : UserControl
    {
        public MainFrame()
        {
            InitializeComponent();
        }

        private void show_KB(object sender, RoutedEventArgs e)
        {
            var uri = new System.Uri(Assembly.GetExecutingAssembly().GetName().CodeBase);
            var location = Path.GetDirectoryName(uri.LocalPath.ToLower());
            System.Diagnostics.Process.Start(location + "\\Console_osk.exe");
        }

        private void save(object sender, RoutedEventArgs e)
        {
            //var ID = new Id();
            //ID.occurrence = OccurrenceTextBox.Text;
            //ID.well_pad = WellPadTextBox.Text;
            //ID.data = DataTextBox.Text;
            ////ID.power = ((TextBlock)PowerComboBox.SelectedItem).Text;
            ////ID.tds = ((TextBlock)TDSComboBox.SelectedItem).Text;
            //ID.serialNumber = txtSerialNumber.Text;

            //var uri = new Uri(Assembly.GetExecutingAssembly().GetName().CodeBase);
            //var location = Path.GetDirectoryName(uri.LocalPath.ToLower()) + @"\data\info";
            //var output = JsonConvert.SerializeObject(ID);

            //try
            //{
            //    File.WriteAllText(location, output);
            //}
            //catch (Exception x)
            //{
            //    Monokot.Hmi.Core.Utils.Log.LogUitls.Report(this, "MainWindow", x.Message, Monokot.Hmi.Core.Utils.Log.MessageType.Developer);
            //}
        }

    }
}
