using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows;
using System.Windows.Input;
using System.Collections.ObjectModel;
using Monokot.ScadaExtension.Wpf.Controls.Buttons;

namespace NGO.Elements.Special.ConfigurateSituation
{
    public class ConfigureSituationButton : HmiButton
    {
        public ConfigureSituationButton()
        {
            Click += ConfigureSituationButton_Click;
            SetValue(uiElementsProperty, new ObservableCollection<UIElement>());
        }

        void ConfigureSituationButton_Click(object sender, RoutedEventArgs e)
        {
            var wnd = new ConfigurationSituationWnd() { 
                startCommand = startCommand, 
                stopCommand = stopCommand, 
                code = situationCode, 
                uiElements=uiElements,
                modelEnable = modelEnable,
                situationText = text,
                WindowStartupLocation = WindowStartupLocation.CenterScreen, 
                SizeToContent=SizeToContent.WidthAndHeight };

            wnd.InvalidateVisual();
            wnd.ShowDialog();
        }

        public static DependencyProperty situationCodeProperty =
           DependencyProperty.Register("situationCode", typeof(int), typeof(ConfigureSituationButton), new PropertyMetadata(0));

        public int situationCode
        {
            get { return (int)GetValue(situationCodeProperty); }
            set { SetValue(situationCodeProperty, value); }
        }

        public static DependencyProperty startCommandProperty =
           DependencyProperty.Register("startCommand", typeof(ICommand), typeof(ConfigureSituationButton), null);

        public ICommand startCommand
        {
            get { return (ICommand)GetValue(startCommandProperty); }
            set { SetValue(startCommandProperty, value); }
        }

        public static DependencyProperty stopCommandProperty =
           DependencyProperty.Register("stopCommand", typeof(ICommand), typeof(ConfigureSituationButton), null);

        public ICommand stopCommand
        {
            get { return (ICommand)GetValue(stopCommandProperty); }
            set { SetValue(stopCommandProperty, value); }
        }

        public static DependencyProperty uiElementsProperty =
           DependencyProperty.Register("uiElements", typeof(ObservableCollection<UIElement>), typeof(ConfigureSituationButton), new FrameworkPropertyMetadata(new ObservableCollection<UIElement>()));

        public ObservableCollection<UIElement> uiElements
        {
            get { return (ObservableCollection<UIElement>)GetValue(uiElementsProperty); }
            set { SetValue(uiElementsProperty, value); }
        }

        public static DependencyProperty modelEnableProperty =
          DependencyProperty.Register("modelEnable", typeof(bool), typeof(ConfigureSituationButton), null);

        public bool modelEnable
        {
            get { return (bool)GetValue(modelEnableProperty); }
            set { SetValue(modelEnableProperty, value); }
        }

        public static readonly DependencyProperty textProperty = DependencyProperty.Register(
            "text", typeof(string), typeof(ConfigureSituationButton), new PropertyMetadata(string.Empty));

        public string text
        {
            get { return (string)GetValue(textProperty); }
            set { SetValue(textProperty, value); }
        }
        
    }
}
