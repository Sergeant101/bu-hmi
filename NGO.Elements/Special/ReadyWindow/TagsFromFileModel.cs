using System;
using System.Collections.ObjectModel;
using System.Collections.Generic;
using System.ComponentModel;
using System.IO;
using System.Text;
using System.Windows;
using System.Windows.Input;
using Monokot.Hmi.Core.Framework.Runtime;
using Monokot.Hmi.Core.Framework.Runtime.Tree;
using Monokot.Hmi.Core.Tags;
using Monokot.Hmi.Core.Utils;
using Monokot.Hmi.Wpf.Utils;
using NGO.Elements.Annotations;
using Monokot.Hmi.Core.Framework.Trees;
using Monokot.Hmi.Core.Fundamental;
using Monokot.Hmi.Core.Utils.Log;
using NGO.Elements.Special.IO;
using Monokot.ScadaExtension.Opc.Da;
using Monokot.Hmi.StdProviders.Modbus;

namespace NGO.Elements.Special.ReadyWindow
{
    public class TagsFromFileModel : DependencyObject, INotifyPropertyChanged
    {
        public TagsFromFileModel(RuntimeTagsTree tags_tree)
        {
            _tagsTree = tags_tree;
            //getItemsCommand = new RelayCommand(getItems);
        }


        private void createDiscreteModule(IList<string> lines, TagsHmiNode node, IHmiDataProvider provider, ICollection<InputOutput> temp_io)
        {
            // парсим файл
            // для дискретных выходов/выходов

            //311002
            //8
            //lang|ru|0 (:2 ) Отжата кнопка "Стоп насоса 1" на пульте бурильщика
            //lang|en|0 (:2 ) Not push button "Stop mud pump 1" on drillers console

            // setup.LangCol

            for (var i = 0; i < lines.Count; i += 5)
            {
                try
                {
                    // Адрес для считывания
                    var word = Convert.ToInt32(lines[i]);
                    var bit = Convert.ToUInt16(lines[i + 1]);

                    var messagesCollection = new List<string>
                    {
                        lines[i + 2].Replace("lang|RU|", ""),
                        lines[i + 3].Replace("lang|EN|", ""),
                        lines[i + 4].Replace("lang|IR|", "")
                    };

                    var factory = new ModbusDataProviderAddressFactory();
                    var opcDaDataAddress = factory.ThrowOrParseAddress(+word + "." + bit);

                    var id = "modbus_" + word + "_" + bit;

                    var tag = new HmiTag(id, node)
                    {
                        DataProvider = provider,
                        UpdateRate = 500,
                        DataAddress = opcDaDataAddress
                    };

                    var path = HmiUtils.GetNodePath<TagsHmiNode, string, HmiTag>(node);

                    if (!_tagsTree.Node[path + "view.io"].Items.ContainsKey(id))
                        _tagsTree.Node[path + "view.io"].Items.Add(id, tag);
                    else
                        tag = _tagsTree.Node[path + "view.io"].Items[id];


                    var io = new InputOutput { tag = tag, messages = messagesCollection };

                    temp_io.Add(io);
                }
                catch (Exception x)
                {
                    LogUitls.Report(this, "АВАРИИ/ГОТОВНОСТИ СПИСКОМ :", x.Message);
                }
            }
        }

        public event PropertyChangedEventHandler PropertyChanged;
        private ObservableCollection<InputOutput> _items = new ObservableCollection<InputOutput>();
        private string _relativeFilePath = @"\data\readies\P1_Emer.txt";
        private readonly RuntimeTagsTree _tagsTree;

        public bool isReadies { get; set; }


        public string relativeFilePath
        {
            get { return _relativeFilePath; }
            set { _relativeFilePath = value; }
        }


        public ObservableCollection<InputOutput> items
        {
            get { return _items; }
            set
            {
                _items = value;
                onPropertyChanged("items");
            }
        }

        public void loadItem(TagsHmiNode node, string file_location)
        {
            _items.Clear();
            var uri = new Uri(System.Reflection.Assembly.GetExecutingAssembly().GetName().CodeBase);
            var path = (Path.GetDirectoryName(uri.LocalPath.ToLower()));

            if (!File.Exists(path + file_location))
            {
                LogUitls.Report(this, "СООБЩЕНИЯ АВАРИЙ СПИСКОМ", "Файл " + file_location + "не существует", MessageType.Error);
                return;
            }

            if (null == _tagsTree)
            {
                LogUitls.Report(this, "СООБЩЕНИЯ АВАРИЙ СПИСКОМ", "RuntimeTagsTree = null!", MessageType.Fatal);
                return;
            }

            var lines = File.ReadAllLines(path + file_location, Encoding.Default);


            try
            {
                var provider = RuntimeManager.Manager.GetDataProvider("ModbusDriver");
                createDiscreteModule(lines, node, provider, items);

            }
            catch (Exception exception)
            {
                LogUitls.Report(this, "СООБЩЕНИЯ АВАРИЙ СПИСКОМ", exception.Message, MessageType.Fatal);
            }
        }

        //public void getItems(object o)
        //{
        //    loadItem(tagDataPath, relativeFilePath);
        //}

        protected virtual void onPropertyChanged(string property_name)
        {
            var handler = PropertyChanged;
            if (handler != null) handler(this, new PropertyChangedEventArgs(property_name));
        }

        public ICommand getItemsCommand { get; set; }
    }

    public class ListItem : INotifyPropertyChanged
    {
        private HmiTag _scadaTag;
        private string _message;

        public HmiTag scadaTag
        {
            get { return _scadaTag; }
            set
            {
                if (Equals(value, _scadaTag)) return;
                _scadaTag = value;
                onPropertyChanged("scadaTag");
            }
        }

        public string message
        {
            get { return _message; }
            set
            {
                if (value == _message) return;
                _message = value;
                onPropertyChanged("message");
            }
        }

        public event PropertyChangedEventHandler PropertyChanged;

        [NotifyPropertyChangedInvocator]
        protected virtual void onPropertyChanged(string property_name)
        {
            var handler = PropertyChanged;
            if (handler != null) handler(this, new PropertyChangedEventArgs(property_name));
        }
    }
}

