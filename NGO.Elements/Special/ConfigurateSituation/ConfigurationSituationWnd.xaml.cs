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
using System.Windows.Shapes;
using System.Collections.ObjectModel;

namespace NGO.Elements.Special.ConfigurateSituation
{
    /// <summary>
    /// Логика взаимодействия для ConfigurationSituationWnd.xaml
    /// </summary>
    public partial class ConfigurationSituationWnd : Window
    {
        public ConfigurationSituationWnd()
        {
            InitializeComponent();
        }

        private void HmiButton_Click(object sender, RoutedEventArgs e)
        {
            DialogResult = true;
            Close();
        }

        public static DependencyProperty startCommandProperty =
           DependencyProperty.Register("startCommand", typeof(ICommand), typeof(ConfigurationSituationWnd), null);
        
        public ICommand startCommand
        {
            get { return (ICommand)GetValue(startCommandProperty); }
            set { SetValue(startCommandProperty, value); }
        }

        public static DependencyProperty stopCommandProperty =
           DependencyProperty.Register("stopCommand", typeof(ICommand), typeof(ConfigurationSituationWnd), null);

        public ICommand stopCommand
        {
            get { return (ICommand)GetValue(stopCommandProperty); }
            set { SetValue(stopCommandProperty, value); }
        }

        public static DependencyProperty codeProperty =
           DependencyProperty.Register("code", typeof(int), typeof(ConfigurationSituationWnd), new PropertyMetadata(0));

        public int code
        {
            get { return (int)GetValue(codeProperty); }
            set { SetValue(codeProperty, value); }
        }

        public static DependencyProperty uiElementsProperty =
           DependencyProperty.Register("uiElements", typeof(ObservableCollection<UIElement>), typeof(ConfigurationSituationWnd), new FrameworkPropertyMetadata(new ObservableCollection<UIElement>()));

        public ObservableCollection<UIElement> uiElements
        {
            get { return (ObservableCollection<UIElement>)GetValue(uiElementsProperty); }
            set { SetValue(uiElementsProperty, value); }
        }

        public static DependencyProperty modelEnableProperty =
          DependencyProperty.Register("modelEnable", typeof(bool), typeof(ConfigurationSituationWnd), new PropertyMetadata(false));

        public bool modelEnable
        {
            get { return (bool)GetValue(modelEnableProperty); }
            set { SetValue(modelEnableProperty, value); }
        }


        public static DependencyProperty situationTextProperty =
          DependencyProperty.Register("situationText", typeof(string), typeof(ConfigurationSituationWnd), new PropertyMetadata(string.Empty));

        public string situationText
        {
            get { return (string)GetValue(situationTextProperty); }
            set { SetValue(situationTextProperty, value); }
        }

        
    }
}
