using System;
using System.IO;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using Monokot.Hmi.Core.Framework.Runtime;
using Monokot.Hmi.Core.Utils;
using Monokot.Hmi.Core.Utils.Log;

namespace NGO.Elements.Special.IO
{
    public partial class IOView
    {
        //private string _ioFolder = @"\data\io\";

        public IOView()
        {
            InitializeComponent();
            //createTopology();
        }


        public static DependencyProperty ioFolderProperty =
            DependencyProperty.Register("ioFolder", typeof(string), typeof(IOView), new FrameworkPropertyMetadata(@"\data\io\"));

        public string ioFolder
        {
            get { return (string)GetValue(ioFolderProperty); }
            set { SetValue(ioFolderProperty, value); }
        }


        public static readonly DependencyProperty ioProviderNameProperty = DependencyProperty.Register(
            "ioProviderName", typeof (string), typeof (IOView), new PropertyMetadata(default(string)));

        public string ioProviderName
        {
            get { return (string) GetValue(ioProviderNameProperty); }
            set { SetValue(ioProviderNameProperty, value); }
        }

        public static readonly DependencyProperty ioDataAddressProperty = DependencyProperty.Register(
            "ioDataAddress", typeof (string), typeof (IOView), new PropertyMetadata(default(string)));

        public string ioDataAddress
        {
            get { return (string) GetValue(ioDataAddressProperty); }
            set { SetValue(ioDataAddressProperty, value); }
        }

        public static readonly DependencyProperty subIoProviderNameProperty = DependencyProperty.Register(
            "subIoProviderName", typeof (string), typeof (IOView), new PropertyMetadata(default(string)));

        public string subIoProviderName
        {
            get { return (string) GetValue(subIoProviderNameProperty); }
            set { SetValue(subIoProviderNameProperty, value); }
        }

        public static readonly DependencyProperty subIoDataAddressProperty = DependencyProperty.Register(
            "subIoDataAddress", typeof (string), typeof (IOView), new PropertyMetadata(default(string)));

        public string subIoDataAddress
        {
            get { return (string) GetValue(subIoDataAddressProperty); }
            set { SetValue(subIoDataAddressProperty, value); }
        }
        

        void TopologyView_Loaded(object sender, RoutedEventArgs e)
        {
            createTopology();
        }

        public void createTopology()
        {
            try
            {
                var topology = new Topology();
                var uri = new Uri(System.Reflection.Assembly.GetExecutingAssembly().GetName().CodeBase);
                var location = Path.GetDirectoryName(Path.GetDirectoryName(uri.LocalPath.ToLower()) + ioFolder);

                topology.createModules(location, RuntimeManager.Manager.Tags);

                TopologyListBox.DataContext = topology.modules;

                if (topology.modules.Count == 0)
                    vfdModule1.Visibility = Visibility.Hidden;
            }
            catch (Exception x)
            {
                LogUitls.Report(this, "ВХОДА/ВЫХОДА", x.Message, MessageType.Warn);
            }
        }

        private void itemOnPreviewMouseUp(object sender, MouseButtonEventArgs e)
        {
            var item = sender as ListBoxItem;

            if (item == null)
            {
                return;
            }

            item.IsSelected = true;
        }
    }
}
