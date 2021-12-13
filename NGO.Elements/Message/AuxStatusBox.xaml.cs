using Monokot.Hmi.Core.Framework.Runtime;
using NGO.Elements.Special.ReadyWindow;
using System;
using System.Collections.Generic;
using System.ComponentModel;
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

namespace NGO.Elements.Message
{
    public partial class AuxStatusBox : UserControl
    {
        private const string STRING_NAME_FOR_SEARCH = "AuxStatusBox_list";

        public AuxStatusBox()
        {
            InitializeComponent();

            if (DesignerProperties.GetIsInDesignMode(this))
                return;

            var readies = new TagsFromFileModel(RuntimeManager.Manager.Tags);
            var emergencies = new TagsFromFileModel(RuntimeManager.Manager.Tags);
            var stops = new TagsFromFileModel(RuntimeManager.Manager.Tags);

            readies.loadItem(RuntimeManager.Manager.Tags.GetRootNode(), @"\data\readies\AD_Ready.csv");
            emergencies.loadItem(RuntimeManager.Manager.Tags.GetRootNode(), @"\data\readies\AD_Emer.csv");
            stops.loadItem(RuntimeManager.Manager.Tags.GetRootNode(), @"\data\readies\AD_Stop.csv");
        }

        private void AuxDriveStatusBox_OnPreviewMouseUp(object sender, MouseButtonEventArgs e)
        {
            if (!hasAlarm && !hasEmergency && !hasStop) return;

            var button = sender as AuxStatusBox;

            if (button != null)
            {
                var panel = VisualTreeHelper.GetParent(button) as Panel;

                if (panel != null && !panel.Children.OfType<ListWindow>().Select(child => child.Name).Contains(STRING_NAME_FOR_SEARCH))
                {
                    var readies = new TagsFromFileModel(RuntimeManager.Manager.Tags);
                    var emergencies = new TagsFromFileModel(RuntimeManager.Manager.Tags);
                    var stops = new TagsFromFileModel(RuntimeManager.Manager.Tags);

                    readies.loadItem(RuntimeManager.Manager.Tags.GetRootNode(), @"\data\readies\AD_Ready.csv");
                    emergencies.loadItem(RuntimeManager.Manager.Tags.GetRootNode(), @"\data\readies\AD_Emer.csv");
                    stops.loadItem(RuntimeManager.Manager.Tags.GetRootNode(), @"\data\readies\AD_Stop.csv");

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
            DependencyProperty.Register("hasEmergency", typeof(bool), typeof(AuxStatusBox), new FrameworkPropertyMetadata(false, FrameworkPropertyMetadataOptions.BindsTwoWayByDefault));

        public bool hasEmergency
        {
            get { return (bool)GetValue(hasEmergencyProperty); }
            set { SetValue(hasEmergencyProperty, value); }
        }

        public static readonly DependencyProperty hasAlarmProperty =
            DependencyProperty.Register("hasAlarm", typeof(bool), typeof(AuxStatusBox), new FrameworkPropertyMetadata(false, FrameworkPropertyMetadataOptions.BindsTwoWayByDefault));

        public bool hasAlarm
        {
            get { return (bool)GetValue(hasAlarmProperty); }
            set { SetValue(hasAlarmProperty, value); }
        }

        public static readonly DependencyProperty hasStopProperty =
          DependencyProperty.Register("hasStop", typeof(bool), typeof(AuxStatusBox), new FrameworkPropertyMetadata(false, FrameworkPropertyMetadataOptions.BindsTwoWayByDefault));


        public bool hasStop
        {
            get { return (bool)GetValue(hasStopProperty); }
            set { SetValue(hasStopProperty, value); }
        }
    }
}
