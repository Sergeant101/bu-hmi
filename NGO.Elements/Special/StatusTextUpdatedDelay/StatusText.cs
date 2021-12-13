using Monokot.Hmi.Core.Framework.Runtime;
using Monokot.Hmi.Core.Framework.Trees;
using Monokot.Hmi.Core.Fundamental;
using Monokot.Hmi.Core.Utils;
using Monokot.Hmi.Core.Utils.Log;
using Monokot.Hmi.Wpf.Utils;
using System;
using System.ComponentModel;
using System.Diagnostics;
using System.Globalization;
using System.Linq;
using System.Windows;
using System.Windows.Controls;

namespace NGO.Elements.Special.StatusTextUpdatedDelay
{
    public class StatusText : TextBlock
    {
        private const string ITEM_NOT_FOUND = "Message not found. Code:";

        private MessagesHmiNode _messagesNode;

        private HmiNodeDescriptor _messageNode;

        private readonly object _locker = new object();

        public readonly static DependencyProperty MessageIDProperty;

        private IHmiMessage _hmiMessage;

        private long _lastupdate;
        private bool _firstUpdate = true;
        private static readonly Stopwatch _stopwatch = Stopwatch.StartNew();

        uint _delayIntervalMs = 10;
        public uint DelayIntervalMs { get { return _delayIntervalMs; } set { _delayIntervalMs = value; } }


        [Category("General")]
        public int MessageID
        {
            get
            {
                return (int)base.GetValue(StatusText.MessageIDProperty);
            }
            set
            {
                base.SetValue(StatusText.MessageIDProperty, value);
            }
        }

        [Category("General")]
        public HmiNodeDescriptor MessageNode
        {
            get
            {
                return this._messageNode;
            }
            set
            {
                this._messageNode = value;
                lock (this._locker)
                {
                    this.UpdateMessages();
                }
            }
        }

        static StatusText()
        {
            StatusText.MessageIDProperty = DependencyProperty.Register("MessageID", typeof(int), typeof(StatusText), new PropertyMetadata(default(int), new PropertyChangedCallback(StatusText.CodeChangedCallback)));
            _stopwatch = Stopwatch.StartNew();
        }

        private static void CodeChangedCallback(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            StatusText statusTextblock = (StatusText)d;
            if (!DesignerProperties.GetIsInDesignMode(d))
            {
                lock (statusTextblock._locker)
                {
                    statusTextblock.UpdateState();
                }
            }
        }

        private void MessagePropertyChanged(object sender, PropertyChangedEventArgs e)
        {
            if (!(e.PropertyName != "Text"))
            {
                lock (this._locker)
                {
                    this.UpdateState();
                }
            }
        }

        private void UpdateMessages()
        {
            if (!DesignerProperties.GetIsInDesignMode(this))
            {
                if (RuntimeManager.Manager.File != null)
                {
                    if ((this.MessageNode == null || this.MessageNode.Path == null ? false : this.MessageNode.ItemType == HmiObjectType.Message))
                    {
                        this._messagesNode = HmiUtils.FindNodeByPath<MessagesHmiNode, int, IHmiMessage>(this.MessageNode.Path, RuntimeManager.Manager.File.MessagesRoot, null, false);
                    }
                    else
                    {
                        this._messagesNode = null;
                    }
                    this.UpdateState();
                }
                else
                {
                    LogUitls.Report(this, "StatusText", "Can't initialize messages. Data file not found", MessageType.Warn, true, 1);
                }
            }
        }

        private void UpdateState()
        {
            base.Dispatcher.Invoke(new Action(() =>
            {
                if (_lastupdate + DelayIntervalMs >= _stopwatch.ElapsedMilliseconds)
                    return;

                if (!(this.MessageNode == null || this.MessageNode.Path == null ? false : this.MessageNode.ItemType == HmiObjectType.Message))
                {
                    base.Text = "Messages not set";
                }
                else if (this._messagesNode != null)
                {
                    if (this._hmiMessage != null)
                    {
                        this._hmiMessage.PropertyChanged -= new PropertyChangedEventHandler(this.MessagePropertyChanged);
                    }
                    this._hmiMessage = this._messagesNode.Items.FirstOrDefault<IHmiMessage>((IHmiMessage item) => item.ID == this.MessageID);
                    if (this._hmiMessage != null)
                    {
                        this._hmiMessage.PropertyChanged += new PropertyChangedEventHandler(this.MessagePropertyChanged);
                    }
                    base.Text = (this._hmiMessage == null ? string.Concat("Message not found. Code: ", this.MessageID.ToString(CultureInfo.InvariantCulture)) : this._hmiMessage.Text);
                }
                else
                {
                    base.Text = string.Concat("Messages '", this.MessageNode.Path, "' not found");
                }
            }), new object[0]);
        }
    }
}
