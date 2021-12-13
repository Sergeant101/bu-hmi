using System;
using System.Timers;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using System.Windows.Shapes;

namespace NGO.Elements.Special.NGOComboBox
{
    public class NGOComboBox : ComboBox
    {

        static NGOComboBox()
        {
            DefaultStyleKeyProperty.OverrideMetadata(typeof(NGOComboBox), new FrameworkPropertyMetadata(typeof(NGOComboBox)));
        }

        Rectangle EmptyRect = new Rectangle();

        public NGOComboBox()
        {
            timer.Elapsed += Timer_Elapsed;
            timer.Interval = 1000 * 40; // 0.5 minute
            timer.Start();
        }

        Timer timer = new Timer();

        private void Timer_Elapsed(object sender, ElapsedEventArgs e)
        {
	    timer.Stop();
            IsEnabledChanged += NGOComboBox_IsEnabledChanged;
        }

        private void NGOComboBox_IsEnabledChanged(object sender, DependencyPropertyChangedEventArgs e)
        {
            if (!(bool)e.NewValue)
                WriteIndex = 0;
        }

        private bool _read = false;
        private UIElement _clonedElement;

        public static readonly DependencyProperty ReadIndexProperty = DependencyProperty.Register(
            "ReadIndex", typeof(int), typeof(NGOComboBox), new PropertyMetadata(-1, ReadIndexChanged, coerce), validateValueCallback);

        private static bool validateValueCallback(object o)
        {
            return ((int)o) >= -1;
        }

        private static object coerce(DependencyObject d, object value)
        {
            var cb = (NGOComboBox)d;
            var candidate = (int)value;

            if ((value is int) && (int)value >= cb.Items.Count)
            {
                cb._read = false;
                cb.SelectedIndex = 0;

                if (cb._clonedElement != null)
                {
                    cb._clonedElement.LayoutUpdated -= cb.CloneLayoutUpdated;
                    cb._clonedElement = null;
                }

                cb._clonedElement = cb.EmptyRect;
                cb.HmiSelectedItem = cb.EmptyRect;

                if (cb.Items.Count > 0)
                {
                    return 0;
                }

                return DependencyProperty.UnsetValue;
            }

            if (candidate != cb.WriteIndex)
                cb._read = true;

            cb.SelectedIndex = candidate;

            return value;
        }

        private static void ReadIndexChanged(DependencyObject d, DependencyPropertyChangedEventArgs args)
        {
            var cb = ((NGOComboBox)d);

            // propagate the new selected item to the SelectionBoxItem property;
            // this displays it in the selection box
            object item = cb.Items[(int)args.NewValue];
            DataTemplate itemTemplate = cb.ItemTemplate;
            string stringFormat = cb.ItemStringFormat;

            // if Items contains an explicit ContentControl, use its content instead
            // (this handles the case of ComboBoxItem)
            ContentControl contentControl = item as ContentControl;

            if (contentControl != null)
            {
                item = contentControl.Content;
                itemTemplate = contentControl.ContentTemplate;
                stringFormat = contentControl.ContentStringFormat;
            }

            if (cb._clonedElement != null)
            {
                cb._clonedElement.LayoutUpdated -= cb.CloneLayoutUpdated;
                cb._clonedElement = null;
            }

            if (itemTemplate == null && cb.ItemTemplateSelector == null && stringFormat == null)
            {
                // if the item is a logical element it cannot be displayed directly in
                // the selection box because it already belongs to the tree (in the dropdown box).
                // Instead, try to extract some useful text from the visual.
                DependencyObject logicalElement = item as DependencyObject;

                if (logicalElement != null)
                {
                    // If the item is a UIElement, create a copy using a visual brush
                    cb._clonedElement = logicalElement as UIElement;

                    if (cb._clonedElement != null)
                    {
                        // Create visual copy of selected element
                        var visualBrush = new VisualBrush(cb._clonedElement);
                        visualBrush.Stretch = Stretch.None;

                        //Set position and dimension of content
                        visualBrush.ViewboxUnits = BrushMappingMode.Absolute;
                        visualBrush.Viewbox = new Rect(cb._clonedElement.RenderSize);

                        //Set position and dimension of tile
                        visualBrush.ViewportUnits = BrushMappingMode.Absolute;
                        visualBrush.Viewport = new Rect(cb._clonedElement.RenderSize);

                        // We need to check if the item acquires a mirror transform through the visual tree
                        // below the ComboBox. If it does then the same mirror transform needs to be applied
                        // to the VisualBrush so that the item shows identically with the selection box as it does
                        // within the dropdown. Eg.
                        // ComboBox - LTR
                        //      |                \
                        //      |          ComboBoxItem-RTL
                        //      |                /
                        // TextBlock (item) - LTR
                        // This TextBlock (item) will need to be mirrored through the VisualBrush, to appear the
                        // same as it does through the ComboBoxItem's mirror transform.
                        //
                        DependencyObject parent = VisualTreeHelper.GetParent(cb._clonedElement);
                        FlowDirection parentFD = parent == null ? FlowDirection.LeftToRight : (FlowDirection)parent.GetValue(FlowDirectionProperty);
                        if (cb.FlowDirection != parentFD)
                        {
                            visualBrush.Transform = new MatrixTransform(new Matrix(-1.0, 0.0, 0.0, 1.0, cb._clonedElement.RenderSize.Width, 0.0));
                        }

                        // Apply visual brush to a rectangle
                        var rect = new Rectangle();
                        rect.Fill = visualBrush;
                        rect.Width = cb._clonedElement.RenderSize.Width;
                        rect.Height = cb._clonedElement.RenderSize.Height;

                        cb._clonedElement.LayoutUpdated += cb.CloneLayoutUpdated;

                        item = rect;
                        itemTemplate = null;
                    }
                    else
                    {
                        //item = cb.ExtractString(logicalElement);
                        //itemTemplate = ContentPresenter.StringContentTemplate;
                    }
                }
            }

            // display a null item by an empty string
            if (item == null)
            {
                item = string.Empty;
                //itemTemplate = ContentPresenter.StringContentTemplate;
            }

            cb.HmiSelectedItem = item;
            //SelectionBoxItem = item;
            //SelectionBoxItemTemplate = itemTemplate;
            //SelectionBoxItemStringFormat = stringFormat;
        }


        private void CloneLayoutUpdated(object sender, EventArgs e)
        {
            Rectangle rect = (Rectangle)HmiSelectedItem;
            rect.Width = _clonedElement.RenderSize.Width;
            rect.Height = _clonedElement.RenderSize.Height;
            rect.HorizontalAlignment = HorizontalAlignment.Center;
            rect.VerticalAlignment = VerticalAlignment.Center;

            VisualBrush visualBrush = (VisualBrush)rect.Fill;
            visualBrush.Viewbox = new Rect(_clonedElement.RenderSize);
            visualBrush.Viewport = new Rect(_clonedElement.RenderSize);
        }


        public int ReadIndex
        {
            get { return (int)GetValue(ReadIndexProperty); }
            set { SetValue(ReadIndexProperty, value); }
        }

        public static readonly DependencyProperty WriteIndexProperty = DependencyProperty.Register(
            "WriteIndex", typeof(int), typeof(NGOComboBox), new PropertyMetadata(default(int)));

        public int WriteIndex
        {
            get { return (int)GetValue(WriteIndexProperty); }
            set { SetValue(WriteIndexProperty, value); }
        }

        public static readonly DependencyProperty HmiSelectedItemProperty = DependencyProperty.Register(
            "HmiSelectedItem", typeof(object), typeof(NGOComboBox), new PropertyMetadata(default(object)));

        public object HmiSelectedItem
        {
            get { return (object)GetValue(HmiSelectedItemProperty); }
            set { SetValue(HmiSelectedItemProperty, value); }
        }


        protected override void OnSelectionChanged(SelectionChangedEventArgs e)
        {
            base.OnSelectionChanged(e);

            if (Items != null && e.AddedItems.Count > 0 && Items.IndexOf(e.AddedItems[0]) != 0)
                _read = false;

            if (!_read)
            {
                if (e.AddedItems == null || e.AddedItems.Count == 0)
                    return;

                var index = Items.IndexOf(e.AddedItems[0]);

                if (index != 0 && IsEnabled)
                    WriteIndex = index;
            }

            _read = false;
            SelectedIndex = 0;
            //var h = HmiSelectedItem;
        }
    }
}
