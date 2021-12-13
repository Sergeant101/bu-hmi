using System.IO;
using System.Reflection;
using Monokot.Hmi.Core.Framework.Storage;
using Monokot.ScadaExtension.Wpf.Controls;
using Newtonsoft.Json;
using System;
using System.Linq;
using Monokot.Hmi.StdProviders.Modbus;
using Monokot.Hmi.Core.Tags;
using Monokot.Hmi.Core.Framework.Trees;
using Monokot.Hmi.Core.Utils;
using Monokot.Hmi.Core.Fundamental;
using System.Collections.Generic;
using System.Text;
using NGO.Elements.Special.IO;
using Monokot.Hmi.Core.Utils.Log;
using Monokot.ScadaExtension.Opc.Da;
using System.Collections.ObjectModel;
using System.Windows.Controls;
using System.Windows;
using System.Windows.Data;
using Monokot.Hmi.Core.Framework.Runtime;
using Monokot.ScadaExtension.WpfComponents.Navigation;
using Monokot.ScadaExtension.Wpf.VisualStateCollection.VisualBox;
using Monokot.Hmi.Core.Messages;
using Monokot.Hmi.Core.Common.RPProperties;
using System.ComponentModel;
using NGO.Elements;
using NGO.Elements.Special.Complectation;
using Monokot.Hmi.Wpf.Utils;
using NGO.Elements.Special.Transliteration;
using Monokot.Hmi.StdProviders.S7;

namespace VFDApp
{
    public partial class App : SEApplication
    {
        private int DisabledTagsCount = 0;
        private int FastTagsCount = 0;
        private int StrangeTagsCount = 0;
        private int NormalTagsCount = 0;
        private List<HmiTag> FastTags = new List<HmiTag>();

        bool CREATE_TRANSLAYE_DICTIONARY = false;

        public static string PlcFilePath = @"\data\plc.txt";

        protected override void OnHmiDataFileLoaded(HmiDataFile file)
        {
            base.OnHmiDataFileLoaded(file);

            var writeNode = file.TagsRoot.Nodes.FirstOrDefault(x => string.Equals(x.ID, "write",
                StringComparison.CurrentCultureIgnoreCase));

            var NodeNamesToDisable = new string[] { "write" };

            string ip = string.Empty;
            bool firstlow = false;
            ModbusDataProvider modbus_provider = file.DataProvidersModule.DataProvidersRoot.Items["ModbusDriver"] as ModbusDataProvider;

            var workingDirectory = Path.GetDirectoryName(Assembly.GetEntryAssembly().Location);

            var location = Path.GetDirectoryName(Assembly.GetEntryAssembly().Location) + @"\data\ports";
            var ports = GetPorts(location);

            try
            {
                var provider = file.DataProvidersModule.DataProvidersRoot.Items["ModbusDriver"] as ModbusDataProvider;
                ip = provider.Host;
                firstlow = provider.FirstWordLowIn32Bit;
                modbus_provider = provider;
                provider.Port = ports[0];
            }
            catch (Exception ex)
            {

            }

            var modbus506 = new ModbusDataProvider("modbus_506", file.DataProvidersModule.DataProvidersRoot)
            {
                Host = ip,
                Port = ports[1],
                InternalRegistersCount = 120,
                FirstWordLowIn32Bit = firstlow
            };


            if (ports[0] == ports[1])
            {
                modbus506 = modbus_provider;
            }
            else
            {
                file.DataProvidersModule.DataProvidersRoot.Items.Add(modbus506);
            }


            var factory = new ModbusDataProviderAddressFactory();

            // nodi dlya otlycheniyta
            HmiUtils.RecursiveNodeAction<TagsHmiNode, string, HmiTag>(file.TagsRoot, n =>
            {
                if (NodeNamesToDisable.Contains(n.ID))
                {
                    HmiUtils.RecursiveNodeAction<TagsHmiNode, string, HmiTag>(n, sn =>
                    {
                        foreach (var stag in sn.Items)
                        {
                            stag.UpdateRate = 0;
                            DisabledTagsCount++;
                        }


                        foreach (var tag in sn.Items.Where(x =>
              x.DataProvider != null &&
              x.DataProvider.GetType() == typeof(OpcDaProvider) &&
              x.DataAddress != null &&
              (x.DataAddress.AsString.IndexOf("mb.plc.") != -1 || x.DataAddress.AsString.IndexOf("mb_fast.plc.") != -1)
              ))
                        {
                            var opc = tag.DataAddress;
                            var _short = opc.AsString.Replace("mb.plc.", "").Replace("mb_fast.plc.", "");

                            if (_short.IndexOf("400") != -1)
                            {

                            }

                            tag.DataProvider = modbus_provider;
                            tag.DataAddress = factory.ThrowOrParseAddress(_short);
                        }
                    });
                }
            });

            HmiUtils.RecursiveNodeAction<TagsHmiNode, string, HmiTag>(file.TagsRoot, n =>
            {

                foreach (var item in n.Items)
                {
                    // Legacy of the void 
                    if (
                    item.UpdateRate > 0 &&
                    (item.DataAddress == null || (item.DataAddress.AsString.IndexOf("mb.plc.") == -1 && item.DataAddress.AsString.IndexOf("mb_fast.plc.") == -1) && (item.DataProvider != null && item.DataProvider.GetType() == typeof(OpcDaProvider))))
                    {
                        item.UpdateRate = 0;
                        StrangeTagsCount++;
                    }
                }

                foreach (var tag in n.Items.Where(x =>
                    x.DataProvider != null &&
                    x.UpdateRate > 0 &&
                    x.DataProvider.GetType() == typeof(OpcDaProvider) &&
                    x.DataAddress != null &&
                    (x.DataAddress.AsString.IndexOf("mb.plc.") != -1 || x.DataAddress.AsString.IndexOf("mb_fast.plc.") != -1)
                    ))
                {
                    var opc = tag.DataAddress;
                    var _short = opc.AsString.Replace("mb.plc.", "").Replace("mb_fast.plc.", "");

                    tag.DataAddress = factory.ThrowOrParseAddress(_short);

                    if (tag.UpdateRate <= 200)
                    {
                        tag.DataProvider = modbus506;
                        tag.UpdateRate = 200;
                        FastTagsCount++;
                        FastTags.Add(tag);
                    }

                    else
                    {
                        tag.DataProvider = modbus_provider;
                        NormalTagsCount++;
                    }
                }
            });

            var zakaz = loadZakazConfig();

            if (zakaz == ZakazType.zakaz54038)
            {
                // 159

                DisableDiscreteAlarms(file, new[] { "emergency160", "readiness160", "emergency_arctic_light", "readiness_arctic_light" });
                DisableAnanlogAlarms(file, new[] { "vfds_abb" });
                ChangeReadiesFiles(zakaz, workingDirectory + @"\data\readies_store\159\", workingDirectory + @"\data\readies\");
                LoadTagsForIO(file, @"\data\io\159\");

            }

            if (zakaz == ZakazType.zakaz54042)
            {
                // 160

                // otkluchaem avarii i preduprejdeniya privodov ABB   
                DisableDiscreteAlarms(file, new[] { "emergency", "readiness", "emergency_arctic_light", "readiness_arctic_light" });
                DisableAnanlogAlarms(file, new[] { "vfds_abb" });


                LoadTagsForIO(file, @"\data\io\160\");
                ChangeReadiesFiles(zakaz, workingDirectory + @"\data\readies_store\160\", workingDirectory + @"\data\readies\");

                // zamena statusa
                ChangeMessages(file.MessagesRoot.Nodes["auxcer_160"], file.MessagesRoot.Nodes["auxcer"]);
                ChangeMessages(file.MessagesRoot.Nodes["AuxWinchcer1_160"], file.MessagesRoot.Nodes["AuxWinchcer1"]);
                ChangeMessages(file.MessagesRoot.Nodes["AuxWinchcer2_160"], file.MessagesRoot.Nodes["AuxWinchcer2"]);
                ChangeMessages(file.MessagesRoot.Nodes["drawworkcer_160"], file.MessagesRoot.Nodes["drawworkcer"]);
                ChangeMessages(file.MessagesRoot.Nodes["pump1cer_160"], file.MessagesRoot.Nodes["pump1cer"]);
                ChangeMessages(file.MessagesRoot.Nodes["pump2cer_160"], file.MessagesRoot.Nodes["pump2cer"]);
                ChangeMessages(file.MessagesRoot.Nodes["rotorcer_160"], file.MessagesRoot.Nodes["rotorcer"]);
            }

            if (zakaz == ZakazType.zakaz54044)
            {
                // Arctic

                // otkluchaem avarii i preduprejdeniya privodov ABB   
                DisableDiscreteAlarms(file, new[] { "emergency", "readiness", "emergency160", "readiness160" });
                DisableAnanlogAlarms(file, new[] { "vfds" });


                LoadTagsForIO(file, @"\data\io\54044\");
                ChangeReadiesFiles(zakaz, workingDirectory + @"\data\readies_store\54044\", workingDirectory + @"\data\readies\");

                // zamena statusa
                ChangeMessages(file.MessagesRoot.Nodes["auxcer_arctic_light"], file.MessagesRoot.Nodes["auxcer"]);
                ChangeMessages(file.MessagesRoot.Nodes["AuxWinchcer1_arctic_light"], file.MessagesRoot.Nodes["AuxWinchcer1"]);
                ChangeMessages(file.MessagesRoot.Nodes["AuxWinchcer2_arctic_light"], file.MessagesRoot.Nodes["AuxWinchcer2"]);
                ChangeMessages(file.MessagesRoot.Nodes["drawworkcer_arctic_light"], file.MessagesRoot.Nodes["drawworkcer"]);
                ChangeMessages(file.MessagesRoot.Nodes["pump1cer_arctic_light"], file.MessagesRoot.Nodes["pump1cer"]);
                ChangeMessages(file.MessagesRoot.Nodes["pump2cer_arctic_light"], file.MessagesRoot.Nodes["pump2cer"]);
                ChangeMessages(file.MessagesRoot.Nodes["pump3cer_arctic_light"], file.MessagesRoot.Nodes["pump3cer"]);
                ChangeMessages(file.MessagesRoot.Nodes["rotorcer_arctic_light"], file.MessagesRoot.Nodes["rotorcer"]);
            }


            if (zakaz == ZakazType.zakaz54056)
            {
                // БНГРЭ

                // то же самое что арктика лайт


                // otkluchaem avarii i preduprejdeniya privodov ABB   
                DisableDiscreteAlarms(file, new[] { "emergency", "readiness", "emergency160", "readiness160" });
                DisableAnanlogAlarms(file, new[] { "vfds" });


                LoadTagsForIO(file, @"\data\io\54056\");
                ChangeReadiesFiles(zakaz, workingDirectory + @"\data\readies_store\54056\", workingDirectory + @"\data\readies\");

                // zamena statusa
                ChangeMessages(file.MessagesRoot.Nodes["auxcer_arctic_light"], file.MessagesRoot.Nodes["auxcer"]);
                ChangeMessages(file.MessagesRoot.Nodes["AuxWinchcer1_arctic_light"], file.MessagesRoot.Nodes["AuxWinchcer1"]);
                ChangeMessages(file.MessagesRoot.Nodes["AuxWinchcer2_arctic_light"], file.MessagesRoot.Nodes["AuxWinchcer2"]);
                ChangeMessages(file.MessagesRoot.Nodes["drawworkcer_arctic_light"], file.MessagesRoot.Nodes["drawworkcer"]);
                ChangeMessages(file.MessagesRoot.Nodes["pump1cer_arctic_light"], file.MessagesRoot.Nodes["pump1cer"]);
                ChangeMessages(file.MessagesRoot.Nodes["pump2cer_arctic_light"], file.MessagesRoot.Nodes["pump2cer"]);
                ChangeMessages(file.MessagesRoot.Nodes["pump3cer_arctic_light"], file.MessagesRoot.Nodes["pump3cer"]);
                ChangeMessages(file.MessagesRoot.Nodes["rotorcer_arctic_light"], file.MessagesRoot.Nodes["rotorcer"]);
            }

            LoadTagsForReadies(file, @"\data\readies\", zakaz);

            var t = DisabledTagsCount;

            InitializeMultilanguageArchive(file);


            var plc = Utils.loadPlcConfig(PlcFilePath);

            // Razbienie tegov na raznie kontrolleri (ktu1, ktu2)
            if (zakaz == ZakazType.zakaz54042 && plc != PlcType.Fastwel)
            {
                var uri = new Uri(Assembly.GetExecutingAssembly().GetName().CodeBase);
                var fileName = Path.GetDirectoryName(uri.LocalPath.ToLower()) + VFDApp.MainWindow.RELATIVE_COMPLECTATION_FILE_PATH;
                var buConfig = BUHelper.LoadConfig(fileName);

                BUHelper.ModifyTagsTo160(file, buConfig, ports, firstlow);
            }

            if (zakaz == ZakazType.zakaz54044)
            {
                var uri = new Uri(Assembly.GetExecutingAssembly().GetName().CodeBase);
                var fileName = Path.GetDirectoryName(uri.LocalPath.ToLower()) + VFDApp.MainWindow.RELATIVE_COMPLECTATION_FILE_PATH;
                var buConfig = BUHelper.LoadConfig(fileName);

                BUHelper.ModifyTagsTo160(file, buConfig, ports, firstlow);
            }



            if (plc == PlcType.Fastwel)
            {
                Utils.SetFastwelTags(file);
                // Motochasiki
                SubscribeToExpression(file);
            }

            //var location2 = Path.GetDirectoryName(Assembly.GetEntryAssembly().Location) + "\\data\\"+ zakaz + "\\complete_" + zakaz + "_" + plc + ".txt";

            //file.SaveFile(location2);

            //HmiUtils.RecursiveNodeAction<TagsHmiNode, string, HmiTag>(file.TagsRoot, n =>
            //{
            //    foreach (var tag in n.Items)
            //    {
            //        // tegi svazannie s otobrazeniem daty
            //        if (!(tag.DataProvider is ModbusDataProvider))
            //            continue;

            //        var address = tag.DataAddress as ModbusDataAddress;

            //        var s7Address = new S7DataAddress();

            //        s7Address.Area = Monokot.Hmi.StdProviders.S7.Common.S7MemoryType.DB;

            //        if (address != null)
            //        {
            //            if (address.AddressTableInfo.AddressTable == Monokot.Hmi.StdProviders.Modbus.Utils.ModbusAddressTableInfo.ModbusAddressTable.HoldingRegisters)
            //            {
            //                //DB 351
            //                s7Address = GetConvertedAddress(351, address);
            //            }

            //            if (address.AddressTableInfo.AddressTable == Monokot.Hmi.StdProviders.Modbus.Utils.ModbusAddressTableInfo.ModbusAddressTable.InternalRegisters)
            //            {
            //                //DB 350
            //                s7Address = GetConvertedAddress(350, address);
            //            }

            //            //file.DataProvidersModule.DataProvidersRoot.Items.Add(new S7DataProvider("s7", file.DataProvidersModule.DataProvidersRoot));
            //            tag.DataProvider = file.DataProvidersModule.DataProvidersRoot.Items["s7"] as S7DataProvider;
            //            tag.DataAddress = s7Address;
            //        }
            //    }
            //});

            //location2 = Path.GetDirectoryName(Assembly.GetEntryAssembly().Location) + @"\data\complete_" + zakaz + "_siemens.txt";

            //file.SaveFile(location2);
        }

        public S7DataAddress GetConvertedAddress(ushort db_number, ModbusDataAddress address)
        {
            var s7Address = new S7DataAddress();

            s7Address.AreaNumber = db_number;
            s7Address.StartAddress = (ushort)((address.StartAddress - 1) * 2);

            switch (address.DataTypeInfo.DataType)
            {
                case Monokot.Hmi.StdProviders.Modbus.Utils.ModbusAddressDataTypeInfo.ModbusAddressDataType.Word:
                    s7Address.DataType = Monokot.Hmi.StdProviders.S7.Common.S7DataAddressTable.S7WordDataType;
                    break;

                case Monokot.Hmi.StdProviders.Modbus.Utils.ModbusAddressDataTypeInfo.ModbusAddressDataType.DWord:
                    s7Address.DataType = Monokot.Hmi.StdProviders.S7.Common.S7DataAddressTable.S7DWordDataType;
                    break;

                case Monokot.Hmi.StdProviders.Modbus.Utils.ModbusAddressDataTypeInfo.ModbusAddressDataType.Short:
                    s7Address.DataType = Monokot.Hmi.StdProviders.S7.Common.S7DataAddressTable.S7IntDataType;
                    break;

                case Monokot.Hmi.StdProviders.Modbus.Utils.ModbusAddressDataTypeInfo.ModbusAddressDataType.Float:
                    s7Address.DataType = Monokot.Hmi.StdProviders.S7.Common.S7DataAddressTable.S7RealDataType;
                    break;

                case Monokot.Hmi.StdProviders.Modbus.Utils.ModbusAddressDataTypeInfo.ModbusAddressDataType.Boolean:
                    s7Address.DataType = Monokot.Hmi.StdProviders.S7.Common.S7DataAddressTable.S7BitType;
                    if (address.BitIndex > 7)
                    {
                        s7Address.StartAddress = (ushort)(s7Address.StartAddress + 1);
                        s7Address.BitIndex = (ushort)(address.BitIndex - 8);
                    }
                    else
                    {
                        s7Address.BitIndex = address.BitIndex;
                    }


                    break;

                default:
                    throw new ArgumentOutOfRangeException("Не указан тип для корректной конверитации");
            }


            return s7Address;
        }

        private void DisableDiscreteAlarms(HmiDataFile file, string[] nodes_for_disable)
        {
            var fake_class = file.AlarmClassList.First(x => x.ID == "fake");

            // nodi diskretnih alarmov dlya otlycheniyta
            HmiUtils.RecursiveNodeAction<DiscreteAlarmsHmiNode, int, IHmiDiscreteAlarm>(file.DiscreteAlarmsRoot, n =>
            {
                if (nodes_for_disable.Contains(n.ID))
                {
                    HmiUtils.RecursiveNodeAction<DiscreteAlarmsHmiNode, int, IHmiDiscreteAlarm>(n, sn =>
                    {
                        foreach (var item in sn.Items)
                        {
                            item.Tag = null;
                            item.AlarmClass = fake_class;
                        }
                    });
                }
            });

        }


        private void DisableAnanlogAlarms(HmiDataFile file, string[] nodes_for_disable)
        {
            var fake_class = file.AlarmClassList.First(x => x.ID == "fake");
            // nodi analnogovih alarmov dlya otlycheniyta
            HmiUtils.RecursiveNodeAction<AnalogAlarmsHmiNode, int, IHmiAnalogAlarm>(file.AnalogAlarmsRoot, n =>
            {
                if (nodes_for_disable.Contains(n.ID))
                {
                    HmiUtils.RecursiveNodeAction<AnalogAlarmsHmiNode, int, IHmiAnalogAlarm>(n, sn =>
                    {
                        foreach (var item in sn.Items)
                        {
                            item.Tag = null;
                            item.AlarmClass = fake_class;
                        }
                    });
                }
            });

        }

        private void ChangeReadiesFiles(ZakazType zakaz, string location, string target)
        {
            foreach (var file in Directory.GetFiles(location))
                File.Copy(file, Path.Combine(target, Path.GetFileName(file)), true);
        }

        private void ChangeMessages(MessagesHmiNode source, MessagesHmiNode target)
        {
            if (source.Items.Count != target.Items.Count)
            {
                throw new Exception(string.Format("{0} {1}", source.ID, target.ID));
            }

            foreach (var item in source.Items)
            {
                ((MultiMessage)target.Items[item.ID])[0, 0] = item.Text;
            }
        }

        private void LoadTagsForIO(HmiDataFile file, string relativePath)
        {
            var uri = new Uri(Assembly.GetExecutingAssembly().GetName().CodeBase);
            var location = Path.GetDirectoryName(Path.GetDirectoryName(uri.LocalPath.ToLower()) + relativePath);

            try
            {
                var dir = Directory.GetDirectories(location);
                foreach (var d in dir)
                {
                    var dinf = new DirectoryInfo(d);
                    createIO(dinf.FullName, file);
                }

            }
            catch (Exception x)
            {
                //
            }

        }

        private const string provider_name = "ModbusDriver";
        private const string io_node_name = "io";
        private const string address_preffix = "";
        private const string float_postffix = "@Float";
        private const string read_node_id = "view";
        private readonly Dictionary<string, HmiTag> _tagsForAdd = new Dictionary<string, HmiTag>();

        public void createIO(string io_files_path, HmiDataFile hmiDataFile)
        {
            var files = Directory.GetFiles(io_files_path);
            var plcModuleType = PLCModuleType.Input;
            var isDiscreteIO = true;
            var descript = "Модуль дискретных входов";

            foreach (var f in files.Select(file => new FileInfo(file)))
            {

                if ((f.Name.IndexOf("AI", StringComparison.Ordinal) > 0) ||
                    (f.Name.IndexOf("FM", StringComparison.Ordinal) > 0) ||
                    (f.Name.IndexOf("IE", StringComparison.Ordinal) > 0) ||
                    (f.Name.IndexOf("VHSC", StringComparison.Ordinal) > 0) ||
                    (f.Name.IndexOf("IF", StringComparison.Ordinal) > 0))
                {
                    isDiscreteIO = false;
                    plcModuleType = PLCModuleType.Input;
                    descript = "Модуль аналоговых входов";
                }
                if ((f.Name.IndexOf("DI", StringComparison.Ordinal) > 0) ||
                    (f.Name.IndexOf("IB", StringComparison.Ordinal) > 0) ||
                    (f.Name.IndexOf("IW", StringComparison.Ordinal) > 0) ||
                    (f.Name.IndexOf("IM", StringComparison.Ordinal) > 0))
                {
                    isDiscreteIO = true;
                    plcModuleType = PLCModuleType.Input;
                    descript = "Модуль дискретных входов";
                }
                if ((f.Name.IndexOf("AO", StringComparison.Ordinal) > 0) ||
                    (f.Name.IndexOf("EO", StringComparison.Ordinal) > 0) ||
                    (f.Name.IndexOf("OE", StringComparison.Ordinal) > 0) ||
                    (f.Name.IndexOf("OF", StringComparison.Ordinal) > 0))
                {
                    isDiscreteIO = false;
                    plcModuleType = PLCModuleType.Output;
                    descript = "Модуль аналоговых выходов";
                }
                if ((f.Name.IndexOf("DO", StringComparison.Ordinal) > 0) ||
                    (f.Name.IndexOf("OB", StringComparison.Ordinal) > 0) ||
                    (f.Name.IndexOf("OW", StringComparison.Ordinal) > 0))
                {
                    isDiscreteIO = true;
                    plcModuleType = PLCModuleType.Output;
                    descript = "Модуль дискретных выходов";
                }

                if (f.Extension != ".txt") continue;

                initialize(f.FullName, isDiscreteIO, hmiDataFile);
            }
        }

        private void InitializeMultilanguageArchive(HmiDataFile file)
        {
            var nodes = file.MessagesModule.MessagesRoot.Nodes;

            //var pump1cer = "pump1cer";
            //var pump1node = nodes[pump1cer];

            foreach (var dictionary in nodes)
            {
                foreach (var multimessage in dictionary.Items.Cast<MultiMessage>())
                {
                    var cells = multimessage.Cells;
                    var translate = Transliteration.Front(cells[0].Text);

                    multimessage.AddColumns(1, new Dictionary<MultiMessageCellPosition, string> { { new MultiMessageCellPosition(0, 1), translate } }, new Dictionary<int, string> { { 0, "Col#1" } });

                    var rp = new Uint32AndTagProperty();

                    rp.Kind = RefPrimitivePropertyKind.Ref;
                    rp.Reference = file.TagsModule.TagsRoot.Items["LanguageCol"];

                    multimessage.SelectedColumn = rp;
                }
            }


        }

        public void initialize(string file, bool IsDiscreteIO, HmiDataFile hmiDataFile)
        {
            //if (_tagsModule == null)
            //{
            //    LogUitls.Report(this, "ВХОДЫ/ВЫХОДЫ", "Ошибка при создании топологии! Указанный TagsModule == null ");
            //    return;
            //}

            var ioProviderName = provider_name;
            var subIoProviderName = provider_name;

            var provider = hmiDataFile.DataProvidersModule.DataProvidersRoot.Items[ioProviderName];

            //var node = new TagsHmiNode(io_node_name, new TagsHmiNode(read_node_id, new TagsHmiNode("root", null)));
            var node = hmiDataFile.TagsModule.TagsRoot.Nodes[read_node_id].Nodes[io_node_name];
            var nodePath = HmiUtils.GetNodePath<TagsHmiNode, string, HmiTag>(node);

            switch (IsDiscreteIO)
            {
                case true:
                    createDiscreteModule(file, node, provider, hmiDataFile.TagsModule);
                    break;
                case false:
                    createAnalogModule(file, node, provider, hmiDataFile.TagsModule);
                    break;
                default:
                    createCounterModule(file, node, provider, hmiDataFile.TagsModule);
                    break;
            }
        }

        private void createAnalogModule(string file, TagsHmiNode node, IHmiDataProvider provider, StorageTagsModule TagsModule)
        {
            var lines = File.ReadAllLines(file, Encoding.Default);

            // 311187
            // 311537
            // lang|ru|(:3, 4 ) Давление масла в рабочих тормозах (4...20 мA = 0...250 атм)
            // {0:0.###} мА = {1:0.#} атм.

            for (var i = 0; i < lines.Count<string>(); i += 6)
            {
                try
                {
                    var mainTagWord = Convert.ToInt32(lines[i]);
                    //var mainTagDataAddress = new OpcDaDataAddress { ItemID = address_preffix + mainTagWord + float_postffix };
                    var id = "modbus_" + mainTagWord;

                    var factory = new ModbusDataProviderAddressFactory();

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

                    if (!TagsModule.TagsRoot.Nodes[read_node_id].Nodes[node.ID].Items.Contains(id))
                    {
                        TagsModule.TagsRoot.Nodes[read_node_id].Nodes[node.ID].Items.Add(tag);
                        TagsModule.TagsRoot.Nodes[read_node_id].Nodes[node.ID].ReadDisablers.Add(new Monokot.Hmi.Core.Common.ActionDisablerContainer(tag));
                        TagsModule.TagsRoot.Nodes[read_node_id].Nodes[node.ID].WriteDisablers.Add(new Monokot.Hmi.Core.Common.ActionDisablerContainer(tag));
                    }

                    if (!TagsModule.TagsRoot.Nodes[read_node_id].Nodes[node.ID].Items.Contains(id + "_subtag_" + subTagWord))
                    {
                        TagsModule.TagsRoot.Nodes[read_node_id].Nodes[node.ID].Items.Add(subtag);
                        TagsModule.TagsRoot.Nodes[read_node_id].Nodes[node.ID].ReadDisablers.Add(new Monokot.Hmi.Core.Common.ActionDisablerContainer(subtag));
                        TagsModule.TagsRoot.Nodes[read_node_id].Nodes[node.ID].WriteDisablers.Add(new Monokot.Hmi.Core.Common.ActionDisablerContainer(subtag));
                    }

                }
                catch (Exception x)
                {
                    LogUitls.Report(this, "ВХОДЫ/ВЫХОДЫ :" + file, x.Message);
                }
            }
        }

        private void createDiscreteModule(string file, TagsHmiNode node, IHmiDataProvider provider, StorageTagsModule TagsModule)
        {

            var lines = File.ReadAllLines(file, Encoding.Default);

            // парсим файл
            // для дискретных выходов/выходов

            //311002
            //8
            //lang|ru|0 (:2 ) Отжата кнопка "Стоп насоса 1" на пульте бурильщика
            //lang|en|0 (:2 ) Not push button "Stop mud pump 1" on drillers console

            // setup.LangCol

            for (var i = 0; i < lines.Count<string>(); i += 4)
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

                    if (!TagsModule.TagsRoot.Nodes[read_node_id].Nodes[node.ID].Items.Contains(id))
                    {
                        TagsModule.TagsRoot.Nodes[read_node_id].Nodes[node.ID].Items.Add(tag);
                        TagsModule.TagsRoot.Nodes[read_node_id].Nodes[node.ID].ReadDisablers.Add(new Monokot.Hmi.Core.Common.ActionDisablerContainer(tag));
                        TagsModule.TagsRoot.Nodes[read_node_id].Nodes[node.ID].WriteDisablers.Add(new Monokot.Hmi.Core.Common.ActionDisablerContainer(tag));
                    }
                }
                catch (Exception x)
                {
                    LogUitls.Report(this, "ВХОДЫ/ВЫХОДЫ :" + file, x.Message);
                }
            }
        }

        private void createCounterModule(string file, TagsHmiNode node, IHmiDataProvider provider, StorageTagsModule TagsModule)
        {
            // 311187
            // 311537
            // lang|ru|(:3, 4 ) Давление масла в рабочих тормозах (4...20 мA = 0...250 атм)
            // {0:0.###} мА = {1:0.#} атм.

            var lines = File.ReadAllLines(file, Encoding.Default);


            for (var i = 0; i < lines.Count<string>(); i += 6)
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

                    if (!TagsModule.TagsRoot.Nodes[read_node_id].Nodes[node.ID].Items.Contains(id))
                        TagsModule.TagsRoot.Nodes[read_node_id].Nodes[node.ID].Items.Add(tag);

                    if (!TagsModule.TagsRoot.Nodes[read_node_id].Nodes[node.ID].Items.Contains(id + "_subtag_" + subTagWord))
                        _tagsForAdd.Add(id + "_subtag_" + subTagWord, subtag);
                }
                catch (Exception x)
                {
                    // LogUitls.Report(this, "ВХОДЫ/ВЫХОДЫ :" + file + name, x.Message);
                }
            }
        }

        private void LoadTagsForReadies(HmiDataFile hmiDataFile, string relativePath, ZakazType zakaz)
        {
            var uri = new Uri(Assembly.GetExecutingAssembly().GetName().CodeBase);

            createReadiesItems(Path.GetDirectoryName(uri.LocalPath.ToLower()) + relativePath + @"AD_Ready.csv", hmiDataFile.DataProvidersModule.DataProvidersRoot.Items[provider_name], hmiDataFile.TagsModule);
            createReadiesItems(Path.GetDirectoryName(uri.LocalPath.ToLower()) + relativePath + @"AD_Emer.csv", hmiDataFile.DataProvidersModule.DataProvidersRoot.Items[provider_name], hmiDataFile.TagsModule);
            createReadiesItems(Path.GetDirectoryName(uri.LocalPath.ToLower()) + relativePath + @"AD_Stop.csv", hmiDataFile.DataProvidersModule.DataProvidersRoot.Items[provider_name], hmiDataFile.TagsModule);

            createReadiesItems(Path.GetDirectoryName(uri.LocalPath.ToLower()) + relativePath + @"AW1_Ready.csv", hmiDataFile.DataProvidersModule.DataProvidersRoot.Items[provider_name], hmiDataFile.TagsModule);
            createReadiesItems(Path.GetDirectoryName(uri.LocalPath.ToLower()) + relativePath + @"AW1_Emer.csv", hmiDataFile.DataProvidersModule.DataProvidersRoot.Items[provider_name], hmiDataFile.TagsModule);
            createReadiesItems(Path.GetDirectoryName(uri.LocalPath.ToLower()) + relativePath + @"AW1_Stop.csv", hmiDataFile.DataProvidersModule.DataProvidersRoot.Items[provider_name], hmiDataFile.TagsModule);

            createReadiesItems(Path.GetDirectoryName(uri.LocalPath.ToLower()) + relativePath + @"AW2_Ready.csv", hmiDataFile.DataProvidersModule.DataProvidersRoot.Items[provider_name], hmiDataFile.TagsModule);
            createReadiesItems(Path.GetDirectoryName(uri.LocalPath.ToLower()) + relativePath + @"AW2_Emer.csv", hmiDataFile.DataProvidersModule.DataProvidersRoot.Items[provider_name], hmiDataFile.TagsModule);
            createReadiesItems(Path.GetDirectoryName(uri.LocalPath.ToLower()) + relativePath + @"AW2_Stop.csv", hmiDataFile.DataProvidersModule.DataProvidersRoot.Items[provider_name], hmiDataFile.TagsModule);

            createReadiesItems(Path.GetDirectoryName(uri.LocalPath.ToLower()) + relativePath + @"AW2_Ready.csv", hmiDataFile.DataProvidersModule.DataProvidersRoot.Items[provider_name], hmiDataFile.TagsModule);
            createReadiesItems(Path.GetDirectoryName(uri.LocalPath.ToLower()) + relativePath + @"AW2_Emer.csv", hmiDataFile.DataProvidersModule.DataProvidersRoot.Items[provider_name], hmiDataFile.TagsModule);

            createReadiesItems(Path.GetDirectoryName(uri.LocalPath.ToLower()) + relativePath + @"Dr_Ready.csv", hmiDataFile.DataProvidersModule.DataProvidersRoot.Items[provider_name], hmiDataFile.TagsModule);
            createReadiesItems(Path.GetDirectoryName(uri.LocalPath.ToLower()) + relativePath + @"Dr_Emer.csv", hmiDataFile.DataProvidersModule.DataProvidersRoot.Items[provider_name], hmiDataFile.TagsModule);
            createReadiesItems(Path.GetDirectoryName(uri.LocalPath.ToLower()) + relativePath + @"Dr_Stop.csv", hmiDataFile.DataProvidersModule.DataProvidersRoot.Items[provider_name], hmiDataFile.TagsModule);

            createReadiesItems(Path.GetDirectoryName(uri.LocalPath.ToLower()) + relativePath + @"P1_Ready.csv", hmiDataFile.DataProvidersModule.DataProvidersRoot.Items[provider_name], hmiDataFile.TagsModule);
            createReadiesItems(Path.GetDirectoryName(uri.LocalPath.ToLower()) + relativePath + @"P1_Emer.csv", hmiDataFile.DataProvidersModule.DataProvidersRoot.Items[provider_name], hmiDataFile.TagsModule);
            createReadiesItems(Path.GetDirectoryName(uri.LocalPath.ToLower()) + relativePath + @"P1_Stop.csv", hmiDataFile.DataProvidersModule.DataProvidersRoot.Items[provider_name], hmiDataFile.TagsModule);

            createReadiesItems(Path.GetDirectoryName(uri.LocalPath.ToLower()) + relativePath + @"P2_Ready.csv", hmiDataFile.DataProvidersModule.DataProvidersRoot.Items[provider_name], hmiDataFile.TagsModule);
            createReadiesItems(Path.GetDirectoryName(uri.LocalPath.ToLower()) + relativePath + @"P2_Emer.csv", hmiDataFile.DataProvidersModule.DataProvidersRoot.Items[provider_name], hmiDataFile.TagsModule);
            createReadiesItems(Path.GetDirectoryName(uri.LocalPath.ToLower()) + relativePath + @"P2_Stop.csv", hmiDataFile.DataProvidersModule.DataProvidersRoot.Items[provider_name], hmiDataFile.TagsModule);

            if (zakaz == ZakazType.zakaz54044 || zakaz == ZakazType.zakaz54056)
            {
                createReadiesItems(Path.GetDirectoryName(uri.LocalPath.ToLower()) + relativePath + @"P3_Ready.csv", hmiDataFile.DataProvidersModule.DataProvidersRoot.Items[provider_name], hmiDataFile.TagsModule);
                createReadiesItems(Path.GetDirectoryName(uri.LocalPath.ToLower()) + relativePath + @"P3_Emer.csv", hmiDataFile.DataProvidersModule.DataProvidersRoot.Items[provider_name], hmiDataFile.TagsModule);
                createReadiesItems(Path.GetDirectoryName(uri.LocalPath.ToLower()) + relativePath + @"P3_Stop.csv", hmiDataFile.DataProvidersModule.DataProvidersRoot.Items[provider_name], hmiDataFile.TagsModule);
            }

            createReadiesItems(Path.GetDirectoryName(uri.LocalPath.ToLower()) + relativePath + @"RT_Ready.csv", hmiDataFile.DataProvidersModule.DataProvidersRoot.Items[provider_name], hmiDataFile.TagsModule);
            createReadiesItems(Path.GetDirectoryName(uri.LocalPath.ToLower()) + relativePath + @"RT_Emer.csv", hmiDataFile.DataProvidersModule.DataProvidersRoot.Items[provider_name], hmiDataFile.TagsModule);
            createReadiesItems(Path.GetDirectoryName(uri.LocalPath.ToLower()) + relativePath + @"RT_Stop.csv", hmiDataFile.DataProvidersModule.DataProvidersRoot.Items[provider_name], hmiDataFile.TagsModule);


        }

        private void createReadiesItems(string file, IHmiDataProvider provider, StorageTagsModule TagsModule)
        {
            // парсим файл
            // для дискретных выходов/выходов

            //311002
            //8
            //lang|ru|0 (:2 ) Отжата кнопка "Стоп насоса 1" на пульте бурильщика
            //lang|en|0 (:2 ) Not push button "Stop mud pump 1" on drillers console

            // setup.LangCol

            var lines = File.ReadAllLines(file, Encoding.Default);
            var factory = new ModbusDataProviderAddressFactory();

            for (var i = 0; i < lines.Length; i += 5)
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

                    //var opcDaDataAddress = new OpcDaDataAddress { ItemID = "mb.plc." + word + "." + bit };
                    var opcDaDataAddress = factory.ThrowOrParseAddress(word + "." + bit);

                    var id = "modbus_" + word + "_" + bit;

                    var tag = new HmiTag(id, TagsModule.TagsRoot.Nodes[read_node_id].Nodes["io"])
                    {
                        DataProvider = provider,
                        UpdateRate = 500,
                        DataAddress = opcDaDataAddress
                    };

                    if (!TagsModule.TagsRoot.Nodes[read_node_id].Nodes["io"].Items.Contains(id))
                    {
                        TagsModule.TagsRoot.Nodes[read_node_id].Nodes["io"].Items.Add(tag);
                        TagsModule.TagsRoot.Nodes[read_node_id].Nodes["io"].ReadDisablers.Add(new Monokot.Hmi.Core.Common.ActionDisablerContainer(tag));
                        TagsModule.TagsRoot.Nodes[read_node_id].Nodes["io"].WriteDisablers.Add(new Monokot.Hmi.Core.Common.ActionDisablerContainer(tag));
                    }
                }
                catch (Exception x)
                {
                    LogUitls.Report(this, "АВАРИИ/ГОТОВНОСТИ СПИСКОМ :", x.Message);
                }
            }
        }


        private ushort[] GetPorts(string filepath)
        {
            ObservableCollection<ushort> ports = new ObservableCollection<ushort>();

            try
            {
                ports = JsonConvert.DeserializeObject<ObservableCollection<ushort>>(File.ReadAllText(filepath));
            }
            catch (Exception x)
            {
                LogUitls.Report(this, "Ports", x.Message);
                ports = new ObservableCollection<ushort>() { 502, 502 };
            }

            if (ports.Count < 2)
            {
                LogUitls.Report(this, "Ports", "Incorrect modbus ports number");
                ports = new ObservableCollection<ushort>() { 502, 502 };
            }

            return ports.ToArray();
        }

        static Dictionary<string, Dictionary<string, string>> translate = new Dictionary<string, Dictionary<string, string>>();

        protected override void OnStartup(StartupEventArgs e)
        {
            base.OnStartup(e);


            if (!CREATE_TRANSLAYE_DICTIONARY)
            {
                var location = Path.GetDirectoryName(Assembly.GetEntryAssembly().Location) + @"\data\translate.txt";

                try
                {
                    translate = JsonConvert.DeserializeObject<Dictionary<string, Dictionary<string, string>>>(File.ReadAllText(location));
                }
                catch (Exception x)
                {
                    return;
                }
            }
        }

        internal Binding GetTranslationBinding()
        {
            var _binding = new Binding("Tags.Node[].Items[LanguageCol].Value");

            // Заполняем декарируемый биндинг свойствами
            _binding.NotifyOnValidationError = true;
            _binding.ValidatesOnDataErrors = true;
            _binding.ValidatesOnExceptions = true;
            _binding.Source = RuntimeManager.Manager;
            _binding.Mode = BindingMode.OneWay;

            return _binding;
        }

        internal string GetLanguage(int trans)
        {
            if (trans == 0)
                return "ru-RU";

            if (trans == 1)
                return "en-US";

            throw new ArgumentException();
        }

        private void TextBlock_OnHandler(object sender, RoutedEventArgs e)
        {
            var txt = (sender as TextBlock);
            if (txt != null)
            {
                var exp = txt.GetBindingExpression(TextBlock.TextProperty);
                if (exp == null)
                {
                    //var binding = new Bind("Tags.Node[].Items[LanguageCol].Value");
                    var _binding = GetTranslationBinding();

                    var bindingExpression = BindingOperations.SetBinding(txt, App.TranslationLanguageProperty, _binding);

                    if (CREATE_TRANSLAYE_DICTIONARY)
                    {
                        var translit = Transliteration.Front(txt.Text);

                        if (!translate.ContainsKey(translit))
                        {
                            var ru = new Dictionary<string, string>();
                            ru.Add("ru-RU", txt.Text);
                            ru.Add("en-US", string.Empty);
                            translate.Add(translit, ru);
                        }

                        App.SetTranslationDictionaryMainID(txt, translate.Keys.ToList().IndexOf(translit));
                    }

                    else
                    {
                        var trans = App.GetTranslationDictionaryMainID(txt);
                        if (trans != null)
                        {
                            var lang = GetLanguage(App.GetTranslationLanguage(txt));

                            var dict = translate.ElementAtOrDefault(trans.Value);

                            if (dict.Value.ContainsKey(lang))
                                txt.Text = dict.Value[lang];

                            return;
                        }

                        var translit = Transliteration.Front(txt.Text);

                        if (translate.ContainsKey(translit))
                        {
                            App.SetTranslationDictionaryMainID(txt, translate.Keys.ToList().IndexOf(translit));
                        }
                    }
                }
            }
        }

        private void ValueDisplay_OnLoaded(object sender, RoutedEventArgs e)
        {
            var vd = (sender as ValueDisplay);
            if (vd != null)
            {
                var exp = vd.GetBindingExpression(ValueDisplay.HeaderProperty);
                if (exp == null)
                {
                    //var binding = new Bind("Tags.Node[].Items[LanguageCol].Value");
                    Binding _binding = GetTranslationBinding();

                    var bindingExpression = BindingOperations.SetBinding(vd, App.TranslationLanguageProperty, _binding);

                    if (CREATE_TRANSLAYE_DICTIONARY)
                    {
                        var translit = Transliteration.Front(vd.Header);

                        if (!translate.ContainsKey(translit))
                        {
                            var ru = new Dictionary<string, string>();
                            ru.Add("ru-RU", vd.Header);
                            ru.Add("en-US", string.Empty);
                            translate.Add(translit, ru);
                        }

                        App.SetTranslationDictionaryMainID(vd, translate.Keys.ToList().IndexOf(translit));
                    }

                    else
                    {
                        var trans = App.GetTranslationDictionaryMainID(vd);
                        if (trans != null)
                        {
                            var lang = GetLanguage(App.GetTranslationLanguage(vd));

                            var dict = translate.ElementAtOrDefault(trans.Value);

                            if (dict.Value.ContainsKey(lang))
                                vd.Header = dict.Value[lang];

                            return;
                        }

                        var translit = Transliteration.Front(vd.Header);

                        if (translate.ContainsKey(translit))
                            App.SetTranslationDictionaryMainID(vd, translate.Keys.ToList().IndexOf(translit));
                    }
                }

                exp = vd.GetBindingExpression(ValueDisplay.LabelProperty);
                if (exp == null)
                {

                    if (CREATE_TRANSLAYE_DICTIONARY)
                    {
                        var translit = Transliteration.Front(vd.Label);

                        if (!translate.ContainsKey(translit))
                        {
                            var ru = new Dictionary<string, string>();
                            ru.Add("ru-RU", vd.Label);
                            ru.Add("en-US", string.Empty);
                            translate.Add(translit, ru);
                        }

                        App.SetTranslationDictionarySubID(vd, translate.Keys.ToList().IndexOf(translit));
                    }

                    else
                    {
                        var trans = App.GetTranslationDictionarySubID(vd);
                        if (trans != null)
                        {
                            var lang = GetLanguage(App.GetTranslationLanguage(vd));

                            var dict = translate.ElementAtOrDefault(trans.Value);

                            if (dict.Value.ContainsKey(lang))
                                vd.Label = dict.Value[lang];

                            return;
                        }

                        var translit = Transliteration.Front(vd.Label);

                        if (translate.ContainsKey(translit))
                        {
                            App.SetTranslationDictionarySubID(vd, translate.Keys.ToList().IndexOf(translit));
                        }
                    }
                }
            }
        }

        private void Button_OnLoaded(object sender, RoutedEventArgs e)
        {
            var btn = (sender as Button);
            if (btn != null)
            {
                var exp = btn.GetBindingExpression(Button.ContentProperty);
                if (exp == null)
                {

                    //var binding = new Bind("Tags.Node[].Items[LanguageCol].Value");
                    var _binding = GetTranslationBinding();

                    var bindingExpression = BindingOperations.SetBinding(btn, App.TranslationLanguageProperty, _binding);

                    if (CREATE_TRANSLAYE_DICTIONARY)
                    {
                        var text = btn.Content as string;

                        if (text != null)
                        {
                            var translit = Transliteration.Front(text);

                            if (!translate.ContainsKey(translit))
                            {
                                var ru = new Dictionary<string, string>();
                                ru.Add("ru-RU", text);
                                ru.Add("en-US", string.Empty);
                                translate.Add(translit, ru);
                            }

                            App.SetTranslationDictionaryMainID(btn, translate.Keys.ToList().IndexOf(translit));

                        }
                    }

                    else
                    {
                        var trans = App.GetTranslationDictionaryMainID(btn);
                        if (trans != null)
                        {
                            var lang = GetLanguage(App.GetTranslationLanguage(btn));

                            var dict = translate.ElementAtOrDefault(trans.Value);

                            if (dict.Value.ContainsKey(lang))
                                btn.Content = dict.Value[lang];

                            return;
                        }

                        var text = btn.Content as string;

                        if (text != null && text != string.Empty)
                        {

                            var str = btn.Content as string;
                            if (str == "НАСТРОЙКА&#xD;&#xA;ДЖОЙСТИКА")
                            {
                                var t = 3;
                            }

                            var translit = Transliteration.Front(text);

                            if (translate.ContainsKey(translit))
                            {
                                App.SetTranslationDictionaryMainID(btn, translate.Keys.ToList().IndexOf(translit));
                            }
                        }
                    }
                }
            }
        }

        private void TabItem_OnLoaded(object sender, RoutedEventArgs e)
        {
            var tb = (sender as HmiTabItem);
            if (tb != null)
            {


                var exp = tb.GetBindingExpression(HmiTabItem.HeaderProperty);
                if (exp == null)
                {

                    //var binding = new Bind("Tags.Node[].Items[LanguageCol].Value");
                    var _binding = GetTranslationBinding();

                    var bindingExpression = BindingOperations.SetBinding(tb, App.TranslationLanguageProperty, _binding);

                    if (CREATE_TRANSLAYE_DICTIONARY)
                    {
                        var text = tb.Header as string;

                        if (text != null)
                        {
                            var translit = Transliteration.Front(text);

                            if (!translate.ContainsKey(translit))
                            {
                                var ru = new Dictionary<string, string>();
                                ru.Add("ru-RU", text);
                                ru.Add("en-US", string.Empty);
                                translate.Add(translit, ru);
                            }

                            App.SetTranslationDictionaryMainID(tb, translate.Keys.ToList().IndexOf(translit));
                        }
                    }

                    else
                    {
                        var text = tb.Header as string;

                        var trans = App.GetTranslationDictionaryMainID(tb);
                        if (trans != null)
                        {
                            var lang = GetLanguage(App.GetTranslationLanguage(tb));

                            var dict = translate.ElementAtOrDefault(trans.Value);

                            if (dict.Value.ContainsKey(lang))
                                tb.Header = dict.Value[lang];

                            return;
                        }

                        if (text != null)
                        {
                            var translit = Transliteration.Front(text);

                            if (translate.ContainsKey(translit))
                            {
                                App.SetTranslationDictionaryMainID(tb, translate.Keys.ToList().IndexOf(translit));
                            }
                        }
                    }
                }
            }
        }

        private void VisualBox_OnLoaded(object sender, RoutedEventArgs e)
        {
            var vb = (sender as VisualBox);
            if (vb != null)
            {
                var exp = vb.GetBindingExpression(VisualBox.TextProperty);
                if (exp == null)
                {
                    var _binding = GetTranslationBinding();

                    var bindingExpression = BindingOperations.SetBinding(vb, App.TranslationLanguageProperty, _binding);

                    if (CREATE_TRANSLAYE_DICTIONARY)
                    {
                        var text = vb.Text as string;

                        if (text != null)
                        {
                            var translit = Transliteration.Front(text);

                            if (!translate.ContainsKey(translit))
                            {
                                var ru = new Dictionary<string, string>();
                                ru.Add("ru-RU", text);
                                ru.Add("en-US", string.Empty);
                                translate.Add(translit, ru);
                            }

                            App.SetTranslationDictionaryMainID(vb, translate.Keys.ToList().IndexOf(translit));
                        }
                    }

                    else
                    {
                        var text = vb.Text as string;

                        var trans = App.GetTranslationDictionaryMainID(vb);
                        if (trans != null)
                        {
                            var lang = GetLanguage(App.GetTranslationLanguage(vb));

                            var dict = translate.ElementAtOrDefault(trans.Value);

                            if (dict.Value.ContainsKey(lang))
                                vb.Text = dict.Value[lang];

                            return;
                        }

                        if (text != null)
                        {
                            var translit = Transliteration.Front(text);

                            if (translate.ContainsKey(translit))
                            {
                                App.SetTranslationDictionaryMainID(vb, translate.Keys.ToList().IndexOf(translit));
                            }
                        }
                    }
                }

                foreach (var item in vb.VisualStateCollection)
                {
                    var exp1 = item.GetBindingExpression(VisualBoxStateItem.TextProperty);
                    if (exp1 == null)
                    {
                        var _binding = GetTranslationBinding();

                        var bindingExpression = BindingOperations.SetBinding(item, App.TranslationLanguageProperty, _binding);

                        if (CREATE_TRANSLAYE_DICTIONARY)
                        {
                            var text = item.Text as string;

                            if (text != null)
                            {
                                var translit = Transliteration.Front(text);

                                if (!translate.ContainsKey(translit))
                                {
                                    var ru = new Dictionary<string, string>();
                                    ru.Add("ru-RU", text);
                                    ru.Add("en-US", string.Empty);
                                    translate.Add(translit, ru);
                                }

                                App.SetTranslationDictionaryMainID(item, translate.Keys.ToList().IndexOf(translit));
                            }
                        }

                        else
                        {
                            var trans = App.GetTranslationDictionaryMainID(item);
                            if (trans != null)
                            {
                                var lang = GetLanguage(App.GetTranslationLanguage(item));

                                var dict = translate.ElementAtOrDefault(trans.Value);

                                if (dict.Value.ContainsKey(lang))
                                    item.Text = dict.Value[lang];

                                return;
                            }

                            var text = item.Text as string;

                            if (text != null)
                            {
                                var translit = Transliteration.Front(text);

                                if (translate.ContainsKey(translit))
                                {
                                    App.SetTranslationDictionaryMainID(item, translate.Keys.ToList().IndexOf(translit));
                                }
                            }
                        }
                    }
                }
            }
        }

        protected override void OnExit(ExitEventArgs e)
        {
            base.OnExit(e);

            if (CREATE_TRANSLAYE_DICTIONARY)
            {
                var output = JsonConvert.SerializeObject(translate, Formatting.Indented);
                var location = Path.GetDirectoryName(Assembly.GetEntryAssembly().Location) + @"\data\translate.txt";

                try
                {
                    File.WriteAllText(location, output);
                }
                catch (Exception)
                {
                    //Helper.exceptionReport(x);
                }
            }
        }

        public static readonly DependencyProperty TranslationLanguageProperty = DependencyProperty.RegisterAttached(
            "TranslationLanguage", typeof(int), typeof(App), new FrameworkPropertyMetadata(0, callback));

        private static void callback(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            int langIndex = (int)e.NewValue;

            string lang = "ru-RU";

            if (langIndex == 0)
                lang = "ru-RU";

            if (langIndex == 1)
                lang = "en-US";


            var txt = d as TextBlock;
            if (txt != null)
            {
                var exp = txt.GetBindingExpression(TextBlock.TextProperty);
                if (exp == null)
                {
                    var el = App.GetTranslationDictionaryMainID(txt);
                    if (el != null)
                    {
                        var dict = translate.ElementAtOrDefault(el.Value);
                        if (dict.Value.ContainsKey(lang))
                            txt.Text = dict.Value[lang];
                    }

                }

                return;
            }

            var btn = d as Button;
            if (btn != null)
            {
                var str = btn.Content as string;
                if (str != null)
                {
                    var exp = btn.GetBindingExpression(Button.ContentProperty);
                    if (exp == null)
                    {
                        var el = App.GetTranslationDictionaryMainID(btn);
                        if (el != null)
                        {
                            var dict = translate.ElementAtOrDefault(el.Value);
                            if (dict.Value.ContainsKey(lang))
                                btn.Content = dict.Value[lang];
                        }
                    }
                }

                return;
            }

            var vd = d as ValueDisplay;
            if (vd != null)
            {
                var exp = vd.GetBindingExpression(ValueDisplay.HeaderProperty);
                if (exp == null)
                {
                    var el = App.GetTranslationDictionaryMainID(vd);
                    if (el != null)
                    {
                        var dict = translate.ElementAtOrDefault(el.Value);
                        if (dict.Value.ContainsKey(lang))
                            vd.Header = dict.Value[lang];
                    }
                }

                exp = vd.GetBindingExpression(ValueDisplay.LabelProperty);
                if (exp == null)
                {
                    var el = App.GetTranslationDictionarySubID(vd);
                    if (el != null)
                    {
                        var dict = translate.ElementAtOrDefault(el.Value);
                        if (dict.Value.ContainsKey(lang))
                        {
                            vd.Label = dict.Value[lang];
                        }
                    }
                }

                return;
            }

            var tb = d as HmiTabItem;
            if (tb != null)
            {
                var exp = tb.GetBindingExpression(HmiTabItem.HeaderProperty);
                if (exp == null)
                {
                    var el = App.GetTranslationDictionaryMainID(tb);
                    if (el != null)
                    {
                        var dict = translate.ElementAtOrDefault(el.Value);
                        if (dict.Value.ContainsKey(lang))
                            tb.Header = dict.Value[lang];
                    }
                }
            }

            var vb = d as VisualBox;
            if (vb != null)
            {
                var exp = vb.GetBindingExpression(VisualBox.TextProperty);
                if (exp == null)
                {
                    var el = App.GetTranslationDictionaryMainID(vb);
                    if (el != null)
                    {
                        var dict = translate.ElementAtOrDefault(el.Value);
                        if (dict.Value.ContainsKey(lang))
                        {
                            if (vb.Text != null)
                                vb.Text = dict.Value[lang];
                        }
                    }
                }
            }

            var vsi = d as VisualBoxStateItem;
            if (vsi != null)
            {
                var exp = vsi.GetBindingExpression(VisualBoxStateItem.TextProperty);
                if (exp == null)
                {
                    var el = App.GetTranslationDictionaryMainID(vsi);
                    if (el != null)
                    {
                        var dict = translate.ElementAtOrDefault(el.Value);
                        if (dict.Value.ContainsKey(lang))
                        {
                            if (vsi.Text != null)
                                vsi.Text = dict.Value[lang];
                        }
                    }
                }
            }
        }

        public static void SetTranslationLanguage(DependencyObject element, int value)
        {
            element.SetValue(TranslationLanguageProperty, value);
        }

        public static int GetTranslationLanguage(DependencyObject element)
        {
            return (int)element.GetValue(TranslationLanguageProperty);
        }

        public static readonly DependencyProperty TranslationDictionaryMainIDProperty = DependencyProperty.RegisterAttached(
            "TranslationDictionaryMainID", typeof(int?), typeof(App), new FrameworkPropertyMetadata(default(int?)));

        public static void SetTranslationDictionaryMainID(DependencyObject element, int? value)
        {
            element.SetValue(TranslationDictionaryMainIDProperty, value);
        }

        public static int? GetTranslationDictionaryMainID(DependencyObject element)
        {
            return (int?)element.GetValue(TranslationDictionaryMainIDProperty);
        }

        public static readonly DependencyProperty TranslationDictionarySubIDProperty = DependencyProperty.RegisterAttached(
            "TranslationDictionarySubID", typeof(int?), typeof(App), new FrameworkPropertyMetadata(default(int?)));

        public static void SetTranslationDictionarySubID(DependencyObject element, int? value)
        {
            element.SetValue(TranslationDictionarySubIDProperty, value);
        }

        public static int? GetTranslationDictionarySubID(DependencyObject element)
        {
            return (int?)element.GetValue(TranslationDictionarySubIDProperty);
        }

        private ZakazType loadZakazConfig()
        {
            var uri = new Uri(Assembly.GetExecutingAssembly().GetName().CodeBase);
            var file = Path.GetDirectoryName(uri.LocalPath.ToLower()) + @"\data\zakaz.txt";

            try
            {
                return JsonConvert.DeserializeObject<ZakazType>(File.ReadAllText(file));
            }
            catch (Exception x)
            {
                return ZakazType.zakaz54038;
            }
        }


        HmiTag _tagZapisiObnovleniaBegunka;
        HmiTag _tagCteniaObnovleniaBegunka;
        short _predidushiyKodUstroistva;
        bool flagZapisPosleObnovlenia;
        bool flag_obnovili_posle_zapisi;

        public static bool _Ostanovit_obnovlenie_begunka = false;

        private void SubscribeToExpression(HmiDataFile file)
        {
            // put do tega zapisi koda ustroistva
            _tagZapisiObnovleniaBegunka = file.TagsModule.TagsRoot.Nodes["write"].Nodes["motochasy"].Items["kod_ustrojstva"];
            _tagCteniaObnovleniaBegunka = file.TagsModule.TagsRoot.Nodes["view"].Nodes["motochasy"].Items["begunok"];

            _tagCteniaObnovleniaBegunka.PropertyChanged += _tagCteniaObnovleniaBegunka_PropertyChanged;
        }

        private void _tagCteniaObnovleniaBegunka_PropertyChanged(object sender, PropertyChangedEventArgs e)
        {
            var tag = sender as HmiTag;

            if (e.PropertyName == "Value" && tag.Value != null)
            {
                if (tag.Value is short)
                    if ((short)tag.Value != -1)
                    {
                        if (!_Ostanovit_obnovlenie_begunka)
                            ((RelayCommand)_tagZapisiObnovleniaBegunka.WriteCommand).Execute(new WriteArgs(tag.Value, null, null) { DisplayFailureMessage = false });

                        flagZapisPosleObnovlenia = true;
                    }
                    else
                    {

                    }
            }
        }


        public static void zapisat_kodUstroistva_i_ostanovit_obnovlenie_pri_schitivanii(Button btn)
        {
            var mode = Monokot.Hmi.Wpf.AttachedBehaviors.Click.GetMode(btn);
            if (mode is NGO.Elements.Special.ClickCommandMode.MomentaryConfirmMode)
            {
                var mm = (NGO.Elements.Special.ClickCommandMode.MomentaryConfirmMode)mode;
                var parameter = mm.PressValue.GetValue();

                if (parameter is Array)
                {
                    var array = (Array)parameter;
                    var id = array.GetValue(0);

                    RuntimeManager.Manager.Tags.Node["write.motochasy"].Items["kod_ustrojstva"].Value = id;

                    _Ostanovit_obnovlenie_begunka = true;

                }
            }
        }

        public static void vozobnovit_schitivanie()
        {
            _Ostanovit_obnovlenie_begunka = false;
        }
    }
}
