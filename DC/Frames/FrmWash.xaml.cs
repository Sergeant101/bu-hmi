using System;
using System.Diagnostics;
using System.Windows;
using System.Windows.Input;

namespace DC.Frames
{
    /// <summary>
    /// Логика взаимодействия для FrmWash.xaml
    /// </summary>
    public partial class FrmWash
    {
        public FrmWash()
        {
            InitializeComponent();

            _timer = new System.Timers.Timer(1000);
            _sec = 8;
            _timer.Elapsed += _timer_Elapsed;
        }



        public event EventHandler timeIsOutCallback;

        void _timer_Elapsed(object sender, System.Timers.ElapsedEventArgs e)
        {
            Dispatcher.Invoke(new System.Threading.ThreadStart(
            delegate
            {
                if (System.ComponentModel.DesignerProperties.GetIsInDesignMode(this)) return;

                labMessage.Text = string.Format("ЧТОБЫ ВЕРНУТЬСЯ НЕ ПРИКАСАЙТЕСЬ К ЭКРАНУ \n{0} СЕКУНД", _sec);
                if (_sec == 0)
                {
                    Visibility = Visibility.Hidden;
                }
            }));
            _sec--;
        }

        private readonly System.Timers.Timer _timer;
        private int _sec = 8;
        private void UserControl_IsVisibleChanged(object sender, DependencyPropertyChangedEventArgs e)
        {
            var v = (bool)e.NewValue;
            if (v)
            {
                _sec = 8;
                _timer.Start();
                labMessage.Text = string.Format("ЧТОБЫ ВЕРНУТЬСЯ НЕ ПРИКАСАЙТЕСЬ К ЭКРАНУ \n{0} СЕКУНД", _sec);

            }
            else
            {
                if (timeIsOutCallback != null) timeIsOutCallback(this, EventArgs.Empty);
                _timer.Stop();
            }
        }

        private void Grid_PreviewMouseMove(object sender, MouseEventArgs e)
        {
            _sec = 8;
        }

        private void ImageButton_PreviewMouseUp(object sender, MouseButtonEventArgs e)
        {
            if (MessageBox.Show("Вы действительно хотите выключить пульт бурильщика?", "Внимание!", MessageBoxButton.YesNo, MessageBoxImage.Question) == MessageBoxResult.Yes)
                Process.Start("shutdown", "/s /t 0");
        }

        private void UIElement_OnPreviewMouseUp(object sender, MouseButtonEventArgs e)
        {
            if (MessageBox.Show("Вы действительно хотите перезагрузить пульт бурильщика?", "Внимание!", MessageBoxButton.YesNo, MessageBoxImage.Question) == MessageBoxResult.Yes)
                Process.Start("shutdown", "/r /t 0");
        }
    }
}
