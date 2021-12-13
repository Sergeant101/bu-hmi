using System.ComponentModel;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media;
using NGO.Elements.Special.ReadyWindow;
using Monokot.Hmi.Core.Framework.Runtime;

namespace NGO.Elements.Message
{
    /// <summary>
    /// Логика взаимодействия для Pump3StatusBox.xaml
    /// </summary>
    public partial class Pump3StatusBox : UserControl
    {
        public Pump3StatusBox()
        {
            InitializeComponent();

            if (DesignerProperties.GetIsInDesignMode(this))
                return;

            var readies = new TagsFromFileModel(RuntimeManager.Manager.Tags);
            var emergencies = new TagsFromFileModel(RuntimeManager.Manager.Tags);
            var stops = new TagsFromFileModel(RuntimeManager.Manager.Tags);

            readies.loadItem(RuntimeManager.Manager.Tags.GetRootNode(), @"\data\readies\P3_Ready.csv");
            stops.loadItem(RuntimeManager.Manager.Tags.GetRootNode(), @"\data\readies\P3_Ready.csv");
            emergencies.loadItem(RuntimeManager.Manager.Tags.GetRootNode(), @"\data\readies\P3_Emer.csv");
        }

        private const string STRING_NAME_FOR_SEARCH = "Pump3StatusBox_list";

        private void Pump3StatusBox_OnPreviewMouseUp(object sender, MouseButtonEventArgs e)
        {
            if (!hasAlarm && !hasEmergency && !hasStop) return;

            var button = sender as Pump3StatusBox;

            if (button != null)
            {
                var panel = VisualTreeHelper.GetParent(button) as Panel;

                if (panel != null && !panel.Children.OfType<ListWindow>().Select(child => child.Name).Contains(STRING_NAME_FOR_SEARCH))
                {
                    var readies = new TagsFromFileModel(RuntimeManager.Manager.Tags);
                    var emergencies = new TagsFromFileModel(RuntimeManager.Manager.Tags);
                    var stops = new TagsFromFileModel(RuntimeManager.Manager.Tags);

                    readies.loadItem(RuntimeManager.Manager.Tags.GetRootNode(), @"\data\readies\P3_Ready.csv");
                    emergencies.loadItem(RuntimeManager.Manager.Tags.GetRootNode(), @"\data\readies\P3_Emer.csv");
                    stops.loadItem(RuntimeManager.Manager.Tags.GetRootNode(), @"\data\readies\P3_Stop.csv");

                    bool parameter = hasAlarm;

                    var wnd = new ListWindow { readies = readies, emergency = emergencies, stops = stops, stopsVisible = hasStop, emergenciesVisible = hasEmergency, readiesVisible = hasAlarm, Name = STRING_NAME_FOR_SEARCH, Height = panel.ActualHeight, Width = panel.ActualWidth };
                    panel.Children.Add(wnd);
                }
                else
                {
                    if (panel != null)
                    {
                        var exist = panel.Children.OfType<ListWindow>().FirstOrDefault(child => child.Name == STRING_NAME_FOR_SEARCH);
                        if (exist != null)
                        {
                            exist.readiesVisible = hasAlarm;
                            exist.emergenciesVisible = hasEmergency;
                            exist.stopsVisible = hasStop;
                            exist.Visibility = Visibility.Visible;

                        }
                    }
                }
            }
        }

        public static readonly DependencyProperty hasEmergencyProperty =
            DependencyProperty.Register("hasEmergency", typeof(bool), typeof(Pump3StatusBox), new FrameworkPropertyMetadata(false, FrameworkPropertyMetadataOptions.BindsTwoWayByDefault));

        public bool hasEmergency
        {
            get { return (bool)GetValue(hasEmergencyProperty); }
            set { SetValue(hasEmergencyProperty, value); }
        }

        public static readonly DependencyProperty hasAlarmProperty =
            DependencyProperty.Register("hasAlarm", typeof(bool), typeof(Pump3StatusBox), new FrameworkPropertyMetadata(false, FrameworkPropertyMetadataOptions.BindsTwoWayByDefault));

        public bool hasAlarm
        {
            get { return (bool)GetValue(hasAlarmProperty); }
            set { SetValue(hasAlarmProperty, value); }
        }

        public static readonly DependencyProperty hasStopProperty =
            DependencyProperty.Register("hasStop", typeof(bool), typeof(Pump3StatusBox), new FrameworkPropertyMetadata(false, FrameworkPropertyMetadataOptions.BindsTwoWayByDefault));


        public bool hasStop
        {
            get { return (bool)GetValue(hasStopProperty); }
            set { SetValue(hasStopProperty, value); }
        }

    }
}
