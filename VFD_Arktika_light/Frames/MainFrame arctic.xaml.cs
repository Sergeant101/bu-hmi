﻿using Newtonsoft.Json;
using NGO.Elements.Special.Complectation;
using System;
using System.Diagnostics;
using System.IO;
using System.Reflection;
using System.Timers;
using System.Windows;
using System.Windows.Controls;

namespace VFD_Arktika_light.Frames
{
    /// <summary>
    /// Interaction logic for MainFrame.xaml
    /// </summary>
    public partial class MainFrame : UserControl
    {
        public const string RELATIVE_COMPLECTATION_FILE_PATH = @"\data\complect.txt";
        public const string RELATIVE_ZAKAZ_TYPE_FILE_PATH = @"\data\zakaz.txt";

        public const string MESSAGE_TEXT = "Для того чтобы изменения вступили в силу, компьютер будет перезугружен. Вы действительно хотите обновить конфигурацию?";
        public const string MESSAGE_TITLE = "Внимание!";
        public const string RESTART_COMMAND = "shutdown";
        public const string RESTART_ARGS = "/s /t 0";


        public MainFrame()
        {
            InitializeComponent();

            var uri = new Uri(Assembly.GetExecutingAssembly().GetName().CodeBase);
            var file = Path.GetDirectoryName(uri.LocalPath.ToLower()) + RELATIVE_ZAKAZ_TYPE_FILE_PATH;
            var complect_file = Path.GetDirectoryName(uri.LocalPath.ToLower()) + RELATIVE_COMPLECTATION_FILE_PATH;

            showMessage = false;

            try
            {
                ZakazType = JsonConvert.DeserializeObject<ZakazType>(File.ReadAllText(file));
            }
            catch (Exception x)
            {
                ZakazType = ZakazType.zakaz54038;
            }

            try
            {
                Complectation = JsonConvert.DeserializeObject<Complectations>(File.ReadAllText(complect_file));
            }
            catch (Exception x)
            {
                Complectation = Complectations.KTU1_KTU2;
            }

            showMessage = true;

            timer.Elapsed += Timer_Elapsed;
            timer.Interval = 1000 * 30; // 1 minute
            timer.Start();
        }

        Timer timer = new Timer();

        private void Timer_Elapsed(object sender, ElapsedEventArgs e)
        {
            Dispatcher.Invoke(new Action(() =>
            {
                var uri = new Uri(Assembly.GetExecutingAssembly().GetName().CodeBase);
                var location = Path.GetDirectoryName(uri.LocalPath.ToLower()) + @"\data\info";

                try
                {
                    var ID = new Id();
                    ID = JsonConvert.DeserializeObject<Id>(File.ReadAllText(location));

                    //OccurrenceTextBox.Text = ID.occurrence;
                    //DataTextBox.Text = ID.data;
                    //WellPadTextBox.Text = ID.well_pad;
                    //txtSerialNumber.Text = ID.serialNumber;
                }
                catch (Exception x)
                {
                    Monokot.Hmi.Core.Utils.Log.LogUitls.Report(this, "MainWindow", x.Message, Monokot.Hmi.Core.Utils.Log.MessageType.Developer);
                    return;
                }

                timer.Enabled = false;
            }));
        }

        private void show_KB(object sender, RoutedEventArgs e)
        {
            var uri = new Uri(Assembly.GetExecutingAssembly().GetName().CodeBase);
            var location = Path.GetDirectoryName(uri.LocalPath.ToLower());
            Process.Start(location + "\\Console_osk.exe");
        }


        private static bool showMessage = true;


        public static DependencyProperty ZakazTypeProperty =
                DependencyProperty.Register("ZakazType", typeof(ZakazType), typeof(MainFrame), new FrameworkPropertyMetadata(ZakazType.zakaz54038, callback));

        public ZakazType ZakazType
        {
            get { return (ZakazType)GetValue(ZakazTypeProperty); }
            set { SetValue(ZakazTypeProperty, value); }
        }

        private static void callback(DependencyObject d, DependencyPropertyChangedEventArgs args)
        {
            var wm = (MainFrame)d;

            var uri = new Uri(Assembly.GetExecutingAssembly().GetName().CodeBase);
            var file = Path.GetDirectoryName(uri.LocalPath.ToLower()) + MainFrame.RELATIVE_ZAKAZ_TYPE_FILE_PATH;

            var value = wm.ZakazType;

            try
            {
                var json = JsonConvert.SerializeObject(value);
                File.WriteAllText(file, json);

                if (showMessage)
                    if (MessageBox.Show(MESSAGE_TEXT, MESSAGE_TITLE, MessageBoxButton.YesNo, MessageBoxImage.Question) == MessageBoxResult.Yes)
                    {
                        Process.Start(RESTART_COMMAND, RESTART_ARGS);
                    }
            }
            catch (Exception x)
            {
                MessageBox.Show(x.Message);
            }
        }

        private void save(object sender, RoutedEventArgs e)
        {
            var ID = new Id();
            //ID.occurrence = OccurrenceTextBox.Text;
            //ID.well_pad = WellPadTextBox.Text;
            //ID.data = DataTextBox.Text;
            //ID.serialNumber = txtSerialNumber.Text;

            var uri = new Uri(Assembly.GetExecutingAssembly().GetName().CodeBase);
            var location = Path.GetDirectoryName(uri.LocalPath.ToLower()) + @"\data\info";
            var output = JsonConvert.SerializeObject(ID);

            try
            {
                File.WriteAllText(location, output);
            }
            catch (Exception x)
            {
                Monokot.Hmi.Core.Utils.Log.LogUitls.Report(this, "MainWindow", x.Message, Monokot.Hmi.Core.Utils.Log.MessageType.Developer);
            }
        }

        public static DependencyProperty ComplectationProperty =
                DependencyProperty.Register("Complectation", typeof(Complectations), typeof(MainFrame), new FrameworkPropertyMetadata(Complectations.KTU1_KTU2, complectation_callback));

        public Complectations Complectation
        {
            get { return (Complectations)GetValue(ComplectationProperty); }
            set { SetValue(ComplectationProperty, value); }
        }


        private static void complectation_callback(DependencyObject d, DependencyPropertyChangedEventArgs args)
        {
            var wm = (MainFrame)d;

            var uri = new Uri(Assembly.GetExecutingAssembly().GetName().CodeBase);
            var file = Path.GetDirectoryName(uri.LocalPath.ToLower()) + MainFrame.RELATIVE_COMPLECTATION_FILE_PATH;

            var value = wm.Complectation;

            try
            {
                var json = JsonConvert.SerializeObject(value);
                File.WriteAllText(file, json);

                if (showMessage)
                    if (MessageBox.Show(MESSAGE_TEXT, MESSAGE_TITLE, MessageBoxButton.YesNo, MessageBoxImage.Question) == MessageBoxResult.Yes)
                    {
                        Process.Start(RESTART_COMMAND, RESTART_ARGS);
                    }
               
            }
            catch (Exception x)
            {
                MessageBox.Show(x.Message);
            }
        }

        private void ComboBox_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            var result = MessageBox.Show("ВНИМАНИЕ!", "ВЫ УВЕРЕНЫ ЧТО ХОТИТЕ ИЗМЕНИТЬ ДАТЧИК ВЕСА?", MessageBoxButton.YesNo, MessageBoxImage.Question);
            if (result == MessageBoxResult.Yes)
            {
            }
        }

        private void ComboBox_Unloaded(object sender, RoutedEventArgs e)
        {
            //weight_sensor_type.SelectionChanged -= ComboBox_SelectionChanged;
        }

        public class Id
        {
            public string occurrence { get; set; }
            public string data { get; set; }
            public string power { get; set; }
            public string tds { get; set; }
            public string well_pad { get; set; }
            public string serialNumber { get; set; }

        }
    }
}
