using System.Windows;
using System.Windows.Controls;
//using TrainingDay.Parameters.View;

namespace VFD.Frames.Sub.SNG_Obuchenie
{
    public partial class MainFrame : UserControl
    {
        public MainFrame()
        {
            InitializeComponent();
        }

        private void Button_Click(object sender, RoutedEventArgs e)
        {
            MessageBox.Show("!");
        }

        private void ProcessParameterCollectionControl_Loaded(object sender, RoutedEventArgs e)
        {
            //var cast = (ProcessParameterCollectionControl) sender;
            //cast.Context = App.trainingContext;
        }
    }
}
