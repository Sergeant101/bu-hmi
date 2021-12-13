using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.IO;
using System.Linq;
using System.Text;
using System.Windows;
using System.Windows.Data;
using System.Windows.Input;
using Monokot.Hmi.Core.Framework.Runtime;
using Monokot.Hmi.Core.Framework.Runtime.Tree;
using Monokot.Hmi.Core.Framework.Trees;
using Monokot.Hmi.Core.Fundamental;
using Monokot.Hmi.Core.Messages;
using Monokot.Hmi.Core.Tags;
using Monokot.Hmi.Core.Tags.Settings;
using Monokot.Hmi.Core.Utils;
using Monokot.Hmi.Core.Utils.Log;
using Monokot.Hmi.Wpf.Bindings;
using Monokot.Hmi.Wpf.Utils;
using Monokot.ScadaExtension.Opc.Da;
using NGO.Elements.Annotations;

namespace NGO.Elements.Special.IO
{
    public class Backplane : INotifyPropertyChanged
    {



        public Backplane(RuntimeTagsTree tagsTree)
        {
            _runtimeTagsTree = tagsTree;
            showInputs = new RelayCommand(getInputs);
            showOutputs = new RelayCommand(getOutputs);
        }

        private ObservableCollection<PlcModule> _ios = new ObservableCollection<PlcModule>();
        private ObservableCollection<PlcModule> _inputsOutputs = new ObservableCollection<PlcModule>();
        private PlcModule _selectedPlcModule;
        private string _name;
        private string _location;
        private readonly RuntimeTagsTree _runtimeTagsTree;
        private bool _showModules;
        public ICommand showInputs { get; set; }
        public ICommand showOutputs { get; set; }

        public PlcModule selectedPlcModule
        {
            get { return _selectedPlcModule; }
            set
            {
                if (Equals(value, _selectedPlcModule)) return;
                _selectedPlcModule = value;
                onPropertyChanged("selectedPlcModule");
            }
        }

        public ObservableCollection<PlcModule> ios
        {
            get { return _ios; }
            set
            {
                if (Equals(value, _ios)) return;
                _ios = value;
                onPropertyChanged("ios");
            }
        }

        public string name
        {
            get { return _name; }
            set
            {
                if (value == _name) return;
                _name = value;
                onPropertyChanged("name");
            }
        }
        public string location
        {
            get { return _location; }
            set
            {
                if (value == _location) return;
                _location = value;
                onPropertyChanged("location");
            }
        }
        public ObservableCollection<PlcModule> inputsOutputs
        {
            get { return _inputsOutputs; }
            private set
            {
                if (Equals(value, _inputsOutputs)) return;
                _inputsOutputs = value;
                onPropertyChanged("inputsOutputs");
            }
        }

        public bool showModules
        {
            get { return _showModules; }
            set
            {
                if (value.Equals(_showModules)) return;
                _showModules = value;
                onPropertyChanged("showModules");
            }
        }

        public void createIO(string path, string module_name)
        {
            var files = Directory.GetFiles(path);
            var plcModuleType = PLCModuleType.Input;
            var isDiscreteModule = true;
            var descript = "Модуль дискретных входов";

            foreach (var f in files.Select(file => new FileInfo(file)))
            {

                if ((f.Name.IndexOf("AI", StringComparison.Ordinal) > 0) ||
                    (f.Name.IndexOf("FM", StringComparison.Ordinal) > 0) ||
                    (f.Name.IndexOf("IE", StringComparison.Ordinal) > 0) ||
                    (f.Name.IndexOf("VHSC", StringComparison.Ordinal) > 0) ||
                    (f.Name.IndexOf("IF", StringComparison.Ordinal) > 0))
                {
                    isDiscreteModule = false;
                    plcModuleType = PLCModuleType.Input;
                    descript = "Модуль аналоговых входов";
                }
                if ((f.Name.IndexOf("DI", StringComparison.Ordinal) > 0) ||
                    (f.Name.IndexOf("IB", StringComparison.Ordinal) > 0) ||
                    (f.Name.IndexOf("IW", StringComparison.Ordinal) > 0) ||
                    (f.Name.IndexOf("IM", StringComparison.Ordinal) > 0))
                {
                    isDiscreteModule = true;
                    plcModuleType = PLCModuleType.Input;
                    descript = "Модуль дискретных входов";
                }
                if ((f.Name.IndexOf("AO", StringComparison.Ordinal) > 0) ||
                    (f.Name.IndexOf("EO", StringComparison.Ordinal) > 0) ||
                    (f.Name.IndexOf("OE", StringComparison.Ordinal) > 0) ||
                    (f.Name.IndexOf("OF", StringComparison.Ordinal) > 0))
                {
                    isDiscreteModule = false;
                    plcModuleType = PLCModuleType.Output;
                    descript = "Модуль аналоговых выходов";
                }
                if ((f.Name.IndexOf("DO", StringComparison.Ordinal) > 0) ||
                    (f.Name.IndexOf("OB", StringComparison.Ordinal) > 0) ||
                    (f.Name.IndexOf("OW", StringComparison.Ordinal) > 0))
                {
                    isDiscreteModule = true;
                    plcModuleType = PLCModuleType.Output;
                    descript = "Модуль дискретных выходов";
                }

                if (_runtimeTagsTree == null)
                {
                    LogUitls.Report(this, "ВХОДЫ/ВЫХОДЫ", "Ошибка при создании топологии! Указанный RuntimeTagsTree == null");
                    return;
                }

                if (f.Extension != ".txt") continue;

                var module = new PlcModule(_runtimeTagsTree)
                {
                    name = f.Name.Replace(f.Extension, ""),
                    file = f.FullName,
                    isDiscreteIo = isDiscreteModule,
                    plcModuleType = plcModuleType,
                    description = descript,
                    subDescription = module_name
                };

                module.initialize();
                ios.Add(module);
            }
        }

        private void getInputs(object o)
        {
            var l = new ObservableCollection<PlcModule>();
            foreach (var io in ios.Where(io => io.plcModuleType == PLCModuleType.Input))
                l.Add(io);

            inputsOutputs = l;
            if (inputsOutputs.Count > 0)
                selectedPlcModule = inputsOutputs.First();
            showModules = true;
        }

        private void getOutputs(object o)
        {
            var l = new ObservableCollection<PlcModule>();
            foreach (var io in ios.Where(io => io.plcModuleType == PLCModuleType.Output))
                l.Add(io);

            inputsOutputs = l;
            if (inputsOutputs.Count > 0)
                selectedPlcModule = inputsOutputs.First();
            showModules = true;
        }


        public event PropertyChangedEventHandler PropertyChanged;

        protected virtual void onPropertyChanged(string property_name)
        {
            var handler = PropertyChanged;
            if (handler != null) handler(this, new PropertyChangedEventArgs(property_name));
        }
    }


    public class PlcModule
    {
        const string provider_name = "ModbusDriver";
        const string view_node_name = "view";
        const string io_node_name = "io";
        const string address_preffix = "";
        const string float_postffix = "@Float";

        private ObservableCollection<InputOutput> _ioCollection = new ObservableCollection<InputOutput>();
        private readonly Dictionary<string, HmiTag> _tagsForAdd = new Dictionary<string, HmiTag>();

        private PLCModuleType _plcModuleType;
        public string file;
        private readonly RuntimeTagsTree _tagsModule;

        public string ioProviderName { get; set; }
        public string subIoProviderName { get; set; }
        public string ioDataAddress { get; set; }
        public string subIoDataAddress { get; set; }

        public string name { get; set; }
        public string description { get; set; }
        public string subDescription { get; set; }


        public bool? isDiscreteIo { get; set; }

        public PLCModuleType plcModuleType
        {
            get { return _plcModuleType; }
            set
            {
                if (value.Equals(_plcModuleType)) return;
                _plcModuleType = value;
            }
        }

        public ObservableCollection<InputOutput> ioCollection
        {
            get { return _ioCollection; }
            set { _ioCollection = value; }
        }

        public PlcModule(RuntimeTagsTree tags_module)
        {
            _tagsModule = tags_module;
        }

        public void initialize()
        {
            if (_tagsModule == null)
            {
                LogUitls.Report(this, "ВХОДЫ/ВЫХОДЫ", "Ошибка при создании топологии! Указанный TagsModule == null ");
                return;
            }

            ioProviderName = provider_name;
            subIoProviderName = provider_name;

            var provider = RuntimeManager.Manager.Providers.Node[""].Items[ioProviderName];

            var node = new TagsHmiNode(io_node_name, new TagsHmiNode(view_node_name, new TagsHmiNode("root", null)));
            var nodePath = HmiUtils.GetNodePath<TagsHmiNode, string, HmiTag>(node);


            var lines = File.ReadAllLines(file, Encoding.Default);
            var tempIO = new ObservableCollection<InputOutput>();

            switch (isDiscreteIo)
            {
                case true:
                    createDiscreteModule(lines, node, provider, tempIO);
                    break;
                case false:
                    createAnalogModule(lines, node, provider, tempIO);
                    break;
                default:
                    createCounterModule(lines, node, provider, tempIO);
                    break;
            }

            //foreach (var hmiTag in _tagsForAdd.Values.Where(hmi_tag => !RuntimeManager.Manager.Tags.Node[nodePath].Items.ContainsKey(hmi_tag.ID)))
            //{

            //    RuntimeManager.Manager.AddTags(new[] { hmiTag });
            //}

            ioCollection = tempIO;
        }

        private void createAnalogModule(IList<string> lines, TagsHmiNode node, IHmiDataProvider provider, ICollection<InputOutput> io_collection)
        {
            // 311187
            // 311537
            // lang|ru|(:3, 4 ) Давление масла в рабочих тормозах (4...20 мA = 0...250 атм)
            // {0:0.###} мА = {1:0.#} атм.

            for (var i = 0; i < lines.Count; i += 6)
            {
                try
                {
                    var mainTagWord = Convert.ToInt32(lines[i]);
                    //var mainTagDataAddress = new OpcDaDataAddress { ItemID = address_preffix + mainTagWord + float_postffix };
                    var id = "modbus_" + mainTagWord;

                    var factory = new Monokot.Hmi.StdProviders.Modbus.ModbusDataProviderAddressFactory();

                    var mainTagDataAddress = factory.ThrowOrParseAddress(address_preffix + mainTagWord + float_postffix) as Monokot.Hmi.StdProviders.Modbus.ModbusDataAddress;

                    //mainTagDataAddress.AmountOfElements = 1;
                    //mainTagDataAddress.DataTypeInfo = Monokot.Hmi.StdProviders.Modbus.Utils.ModbusAddressDataTypeInfo.Float;
                    //mainTagDataAddress.StartAddress = mainTagWord - 300000;

                    var subTagWord = Convert.ToInt32(lines[i + 1]);
                    //var subTagDataAddress = new OpcDaDataAddress { ItemID = address_preffix + subTagWord + float_postffix };

                    var subTagDataAddress = factory.ThrowOrParseAddress(address_preffix + subTagWord + float_postffix) as Monokot.Hmi.StdProviders.Modbus.ModbusDataAddress;

                    //subTagDataAddress.AmountOfElements = 1;
                    //subTagDataAddress.DataTypeInfo = Monokot.Hmi.StdProviders.Modbus.Utils.ModbusAddressDataTypeInfo.Float;
                    //subTagDataAddress.StartAddress = subTagWord - 300000;

                    var messages_collection = new List<string>();
                    messages_collection.Add(lines[i + 2].Replace("lang|RU|", ""));
                    messages_collection.Add(lines[i + 4].Replace("lang|EN|", ""));

                    var templates_collection = new List<string>();
                    templates_collection.Add(lines[i + 3].Replace("lang|RU|", ""));
                    templates_collection.Add(lines[i + 5].Replace("lang|EN|", ""));

                    var subtag = new HmiTag(id + "_subtag_" + subTagWord, node)
                    {
                        DataProvider = provider,
                        UpdateRate = 1000,
                        DataAddress = subTagDataAddress
                    };

                    var tag = new HmiTag(id, node)
                    {
                        DataProvider = provider,
                        DataAddress = mainTagDataAddress,
                        UpdateRate = 1000
                    };


                    var path = HmiUtils.GetNodePath<TagsHmiNode, string, HmiTag>(node);

                    if (!_tagsModule.Node[path].Items.ContainsKey(id))
                    {
                        //_tagsForAdd.Add(id, tag);
                    }
                    else
                        tag = _tagsModule.Node[path].Items[id];

                    if (!_tagsModule.Node[path].Items.ContainsKey(id + "_subtag_" + subTagWord))
                    {
                        //_tagsForAdd.Add(id + "_subtag_" + subTagWord, subtag);
                    }
                    else
                        subtag = _tagsModule.Node[path].Items[id + "_subtag_" + subTagWord];


                    io_collection.Add(new InputOutput
                    {
                        tag = tag,
                        stringTemplates = templates_collection,
                        messages = messages_collection,
                        viewTemplateType = ViewTemplateType.Analog,
                        subTag = subtag
                    });
                }
                catch (Exception x)
                {
                    LogUitls.Report(this, "ВХОДЫ/ВЫХОДЫ :" + file + name, x.Message);
                }
            }
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

            for (var i = 0; i < lines.Count; i += 4)
            {
                try
                {
                    // Адрес для считывания
                    var word = Convert.ToInt32(lines[i]);
                    var bit = Convert.ToUInt16(lines[i + 1]);

                    var messages_collection = new List<string>();
                    messages_collection.Add(lines[i + 2].Replace("lang|RU|", ""));
                    messages_collection.Add(lines[i + 3].Replace("lang|EN|", ""));

                    var factory = new Monokot.Hmi.StdProviders.Modbus.ModbusDataProviderAddressFactory();

                    var mb = factory.ThrowOrParseAddress(address_preffix + word + "." + bit) as Monokot.Hmi.StdProviders.Modbus.ModbusDataAddress;


                    //var opcDaDataAddress = new OpcDaDataAddress { ItemID = address_preffix + word + "." + bit };

                    //mb.AmountOfElements = 1;
                    //mb.DataTypeInfo = Monokot.Hmi.StdProviders.Modbus.Utils.ModbusAddressDataTypeInfo.Boolean;
                    //mb.StartAddress = word - 300000;
                    //mb.BitIndex = bit;

                    var id = "modbus_" + word + "_" + bit;

                    var tag = new HmiTag(id, node)
                    {
                        DataProvider = provider,
                        UpdateRate = 1000,
                        DataAddress = mb
                    };

                    var path = HmiUtils.GetNodePath<TagsHmiNode, string, HmiTag>(node);

                    if (!_tagsModule.Node[path].Items.ContainsKey(id))
                    {
                        //_tagsForAdd.Add(id, tag);
                    }
                    else
                        tag = _tagsModule.Node[path].Items[id];


                    var io = new InputOutput { tag = tag, messages = messages_collection, viewTemplateType = ViewTemplateType.Discrete };

                    temp_io.Add(io);
                }
                catch (Exception x)
                {
                    LogUitls.Report(this, "ВХОДЫ/ВЫХОДЫ :" + file + name, x.Message);
                }
            }
        }

        private void createCounterModule(IList<string> lines, TagsHmiNode node, IHmiDataProvider provider, ICollection<InputOutput> io_collection)
        {
            // 311187
            // 311537
            // lang|ru|(:3, 4 ) Давление масла в рабочих тормозах (4...20 мA = 0...250 атм)
            // {0:0.###} мА = {1:0.#} атм.

            for (var i = 0; i < lines.Count; i += 6)
            {
                try
                {
                    var mainTagWord = Convert.ToInt32(lines[i]);
                    var mainTagDataAddress = new OpcDaDataAddress { ItemID = address_preffix + mainTagWord + float_postffix };
                    var id = "mb.plc_" + mainTagWord;


                    var subTagWord = Convert.ToInt32(lines[i + 1]);
                    var subTagDataAddress = new OpcDaDataAddress { ItemID = address_preffix + subTagWord + float_postffix };

                    var messages_collection = new List<string>();
                    messages_collection.Add(lines[i + 2].Replace("lang|RU|", ""));
                    messages_collection.Add(lines[i + 4].Replace("lang|EN|", ""));

                    var templates_collection = new List<string>();
                    templates_collection.Add(lines[i + 3].Replace("lang|RU|", ""));
                    templates_collection.Add(lines[i + 5].Replace("lang|EN|", ""));

                    var subtag = new HmiTag(id + "_subtag_" + subTagWord, node)
                    {
                        DataProvider = provider,
                        UpdateRate = 1000,
                        DataAddress = subTagDataAddress
                    };

                    var tag = new HmiTag(id, node)
                    {
                        DataProvider = provider,
                        DataAddress = mainTagDataAddress,
                        UpdateRate = 1000
                    };


                    var path = HmiUtils.GetNodePath<TagsHmiNode, string, HmiTag>(node);

                    if (!_tagsModule.Node[path].Items.ContainsKey(id))
                        _tagsForAdd.Add(id, tag);
                    else
                        tag = _tagsModule.Node[path].Items[id];

                    if (!_tagsModule.Node[path].Items.ContainsKey(id + "_subtag_" + subTagWord))
                        _tagsForAdd.Add(id + "_subtag_" + subTagWord, subtag);
                    else
                        tag = _tagsModule.Node[path].Items[id + "_subtag_" + subTagWord];


                    io_collection.Add(new InputOutput
                    {
                        tag = tag,
                        stringTemplates = templates_collection,
                        messages = messages_collection,
                        viewTemplateType = ViewTemplateType.Counter,
                        subTag = subtag
                    });
                }
                catch (Exception x)
                {
                    LogUitls.Report(this, "ВХОДЫ/ВЫХОДЫ :" + file + name, x.Message);
                }
            }
        }
    }

    public class InputOutput : INotifyPropertyChanged
    {

        public InputOutput()
        {
            var langtag = RuntimeManager.Manager.Tags.Node[""].Items["LanguageCol"];
            if (langtag != null)
            {
                langtag.PropertyChanged += LangtagOnPropertyChanged;

                if (langtag.Value != null)
                    indexOfSelectedMessage = Convert.ToInt32(langtag.Value);
            }
        }

        private void LangtagOnPropertyChanged(object sender, PropertyChangedEventArgs event_args)
        {
            if (event_args.PropertyName != "Value") return;

            var basicTag = sender as HmiTag;

            if (basicTag == null) return;

            int index = 0;
            if (basicTag.Value != null)
            {
                int.TryParse(basicTag.Value.ToString(), out index);
            }

            indexOfSelectedMessage = index;
        }

        public event PropertyChangedEventHandler PropertyChanged;

        private List<string> _messages = new List<string>();
        private List<string> _stringTemplates = new List<string>();

        private string _selectedStringTemplate;
        private string _selectedMessage;

        private HmiTag _tag;
        private HmiTag _subTag;
        private int _indexOfSelectedMessage;

        public ViewTemplateType viewTemplateType { get; set; }

        public HmiTag tag
        {
            get { return _tag; }
            set
            {
                if (Equals(value, _tag)) return;

                if (_tag != null)
                    _tag.PropertyChanged -= tag_PropertyChanged;

                _tag = value;

                if (_tag != null)
                    _tag.PropertyChanged += tag_PropertyChanged;

                onPropertyChanged("tag");

            }
        }

        public HmiTag subTag
        {
            get { return _subTag; }
            set
            {
                if (Equals(value, _subTag)) return;

                if (_subTag != null)
                    _subTag.PropertyChanged -= tag_PropertyChanged;

                _subTag = value;

                if (_subTag != null)
                    _subTag.PropertyChanged += tag_PropertyChanged;

                onPropertyChanged("subTag");

            }
        }

        public string resultString
        {
            get
            {
                if (messages != null && messages.Any())
                    return string.Format(selectedStringTemplate, tag.Value, subTag.Value);

                return "Messages empty";
            }
        }

        public int indexOfSelectedMessage
        {
            get { return _indexOfSelectedMessage; }
            set
            {
                if (Equals(value, _indexOfSelectedMessage)) return;

                _indexOfSelectedMessage = value;

                // Обновляем выбранное сообщение
                // строковый шаблон
                // результирующую строку
                onPropertyChanged("indexOfSelectedMessage");
                onPropertyChanged("selectedMessage");
                onPropertyChanged("selectedStringTemplate");
                onPropertyChanged("resultString");
            }
        }

        public string selectedStringTemplate
        {
            get
            {
                if (stringTemplates != null && stringTemplates.Any())
                    return stringTemplates[indexOfSelectedMessage];

                return string.Empty;
            }
        }

        public string selectedMessage
        {
            get
            {
                if (messages != null && messages.Any() && messages.Count > indexOfSelectedMessage)
                    return messages[indexOfSelectedMessage];

                return string.Empty;
            }
        }

        public List<string> messages
        {
            get { return _messages; }
            set
            {
                if (Equals(value, _messages)) return;

                _messages = value;

                onPropertyChanged("messages");
            }
        }

        public List<string> stringTemplates
        {
            get { return _stringTemplates; }
            set
            {
                if (Equals(value, _stringTemplates)) return;

                _stringTemplates = value;

                onPropertyChanged("stringTemplates");
            }
        }

        protected virtual void onPropertyChanged(string property_name)
        {
            var handler = PropertyChanged;
            if (handler != null) handler(this, new PropertyChangedEventArgs(property_name));
        }

        void tag_PropertyChanged(object sender, PropertyChangedEventArgs e)
        {
            if (e.PropertyName != "Value") return;

            onPropertyChanged("resultString");
        }
    }

    public enum PLCModuleType
    {
        // Тип самого модуля, нужен для того, 
        // чтобы нажать кнопку "выходы" и показались "выходы" контроллера
        // Счетчик - это "выход",

        Input,
        Output,
        Counter
    }

    public enum ViewTemplateType
    {
        // Дискретный
        Discrete,
        // Аналоговый
        Analog,
        // Счетчик (типа импульсов)
        Counter
    }
}
