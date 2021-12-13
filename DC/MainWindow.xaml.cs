using System;
using System.Windows;
using Monokot.Hmi.Core.Framework.Runtime;
using Monokot.Hmi.Core.Framework.Trees;
using Monokot.Hmi.Core.Fundamental;
using Monokot.Hmi.Core.Utils;
using Monokot.ScadaExtension.Wpf.Controls;
using System.Diagnostics;
using System.Linq;
using NGO.Elements.ZeroHour;
using NGO.Elements.Special.Complectation;
using System.Reflection;
using System.IO;
using Newtonsoft.Json;

namespace DC
{
    public partial class MainWindow : SEWindow
    {
        public const string RELATIVE_ZAKAZ_TYPE_FILE_PATH = @"\data\zakaz.txt";

        public MainWindow()
        {
            InitializeComponent();

            Context = new ZeroHourViewModel();

            BeforeInitialization += MainWindow_beforeInitialization;

            var uri = new Uri(Assembly.GetExecutingAssembly().GetName().CodeBase);
            var file = Path.GetDirectoryName(uri.LocalPath.ToLower()) + RELATIVE_ZAKAZ_TYPE_FILE_PATH;
           

            try
            {
                ZakazType = JsonConvert.DeserializeObject<ZakazType>(File.ReadAllText(file));
            }
            catch (Exception x)
            {
                ZakazType = ZakazType.zakaz54038;
            }
        }

        void MainWindow_beforeInitialization(object sender, BeforeInitializationEventArgs e)
        {


            RuntimeManager.Manager.File.AlarmsModule.AlarmHistorians.Clear();
            RuntimeManager.Manager.File.TrendsModule.TrendHistorians.Clear();

            var analogAlarmsRootNode = RuntimeManager.Manager.AnalogAlarms.GetRootNode();

            HmiUtils.RecursiveNodeAction<AnalogAlarmsHmiNode, Int32, IHmiAnalogAlarm>(analogAlarmsRootNode, node =>
            {
                foreach (var analogAlarm in node.Items)
                {
                    analogAlarm.AlarmClass.HistorianProvider = null;
                }
            });


            var discreteAlarmsRootNode = RuntimeManager.Manager.DiscreteAlarms.GetRootNode();
            HmiUtils.RecursiveNodeAction<DiscreteAlarmsHmiNode, Int32, IHmiDiscreteAlarm>(discreteAlarmsRootNode, node =>
            {
                foreach (var analogAlarm in node.Items)
                {
                    analogAlarm.AlarmClass.HistorianProvider = null;
                }
            });

            var trendsRootNode = RuntimeManager.Manager.Trends.GetRootNode();
            HmiUtils.RecursiveNodeAction<TrendsHmiNode, Int32, IHmiTrend>(trendsRootNode, node =>
            {
                foreach (var trend in node.Items)
                {
                    trend.HistorianProvider = null;
                }
            });



        }

        private void ScadaButton_Click(object sender, RoutedEventArgs e)
        {
            Wash.Visibility = Visibility.Visible;
        }

        private void HmiButton_Click(object sender, RoutedEventArgs e)
        {
            if (
                MessageBox.Show("Вы действительно хотите открыть просмотр экрана компьютера в модуле ЧП?", "Внимание!",
                    MessageBoxButton.YesNo, MessageBoxImage.Question) == MessageBoxResult.Yes)
            {
                if (Process.GetProcessesByName("tvnviewer.exe").Any() != true)
                {
                    var processes = Process.GetProcessesByName("tvnviewer.exe");
                    foreach (var p in processes)
                        p.Kill();
                }

                Process.Start(@"C:\Program Files\TightVNC\tvnviewer.exe", "-host=192.168.2.3 -password=qwertyuiop");

            }
        }

        public static readonly DependencyProperty ContextProperty = DependencyProperty.Register(
           "Context", typeof(ZeroHourViewModel), typeof(MainWindow), new PropertyMetadata(default(ZeroHourViewModel)));

        public ZeroHourViewModel Context
        {
            get { return (ZeroHourViewModel)GetValue(ContextProperty); }
            set { SetValue(ContextProperty, value); }
        }

        public static DependencyProperty ZakazTypeProperty =
                DependencyProperty.Register("ZakazType", typeof(ZakazType), typeof(MainWindow), new FrameworkPropertyMetadata(ZakazType.zakaz54038));

        public ZakazType ZakazType
        {
            get { return (ZakazType)GetValue(ZakazTypeProperty); }
            set { SetValue(ZakazTypeProperty, value); }
        }
    }
}
