using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Windows;
using System.Windows.Controls;

namespace NGO.Elements.Special.IndustrialSwitchButton
{
    public class IndustrialSwitchButton : ItemsControl
    {
        static IndustrialSwitchButton()
        {
            DefaultStyleKeyProperty.OverrideMetadata(typeof(IndustrialSwitchButton), new FrameworkPropertyMetadata(typeof(IndustrialSwitchButton)));
        }

        protected override void OnInitialized(EventArgs e)
        {
            base.OnInitialized(e);

            foreach (var button in Items.OfType<RadioButton>())
                button.Checked += button_Checked;
        }

        void button_Checked(object sender, RoutedEventArgs e)
        {
            update();

            //var button = ((RadioButton)e.OriginalSource);
            //if (button != null && button.IsChecked == true)
            //    SetValue(BackgroundProperty, button.Background);
        }

        void update()
        {
            var s = Items.OfType<RadioButton>().FirstOrDefault(x=> x.IsChecked == true);
            if (s != null)
                SetValue(BackgroundProperty, s.Background);
        }

        public static readonly DependencyProperty footerHeightProperty = DependencyProperty.Register(
            "footerHeight", typeof(GridLength), typeof(IndustrialSwitchButton), new PropertyMetadata(new GridLength(0.3d, GridUnitType.Star)));


        public GridLength footerHeight
        {
            get { return (GridLength)GetValue(footerHeightProperty); }
            set { SetValue(footerHeightProperty, value); }
        }
    }
}
