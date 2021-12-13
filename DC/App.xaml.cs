using System;
using System.IO;
using System.Reflection;
using System.Windows;
using Monokot.Hmi.Core.Framework.Storage;
using Monokot.ScadaExtension.Wpf.Controls;
using Newtonsoft.Json;
using System.Collections.Generic;
using Monokot.Hmi.Core.Tags;
using System.Linq;
using Monokot.Hmi.StdProviders.Modbus;
using Monokot.Hmi.Core.Utils;
using Monokot.Hmi.Core.Framework.Trees;
using Monokot.ScadaExtension.Opc.Da;
using NGO.Elements.Special.IO;
using Monokot.Hmi.Core.Fundamental;
using System.Text;
using Monokot.Hmi.Core.Utils.Log;
using System.Collections.ObjectModel;
using Newtonsoft.Json.Converters;
using System.ComponentModel;
using NGO.Elements;
using NGO.Elements.Special.Complectation;
using Monokot.Hmi.StdProviders.Ping;

namespace DC
{
    public partial class App : SEApplication
    {
        //public static TrainingViewModel trainingContext;

        private int DisabledTagsCount = 0;
        private int FastTagsCount = 0;
        private int StrangeTagsCount = 0;
        private int NormalTagsCount = 0;
        private List<HmiTag> FastTags = new List<HmiTag>();

        private bool S7_PROVIDER_TEST = false;

        //private Monokot.Hmi.StdProviders.S7.S7DataProvider s7_provider;

        protected override void OnHmiDataFileLoaded(HmiDataFile file)
        {
            base.OnHmiDataFileLoaded(file);

            //s7_provider = file.DataProvidersModule.DataProvidersRoot.Items["s7_tcp"] as Monokot.Hmi.StdProviders.S7.S7DataProvider;


            var writeNode = file.TagsRoot.Nodes.FirstOrDefault(x => string.Equals(x.ID, "write",
                StringComparison.CurrentCultureIgnoreCase));

            var NodeNamesToDisable = new string[] { "multimetr1", "multimetr2", "bkt1", "kru", "write" };

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
                //provider.Host = "127.0.0.1";

                var p = provider.GetType().GetProperty("IsLicensed", BindingFlags.NonPublic | BindingFlags.Instance | BindingFlags.SetField);
                p.SetValue(provider, true, null);
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }

            if (!S7_PROVIDER_TEST)
            {

                var modbus506 = new ModbusDataProvider("modbus_506", file.DataProvidersModule.DataProvidersRoot)
                {
                    Host = ip,
                    Port = ports[1],
                    //Port = 506,
                    //InternalRegistersCount = 64,
                    InternalRegistersCount = 120,
                    FirstWordLowIn32Bit = firstlow
                };

                var p = modbus506.GetType().GetProperty("IsLicensed", BindingFlags.NonPublic | BindingFlags.Instance | BindingFlags.SetField);
                p.SetValue(modbus506, true, null);

                if (ports[0] == ports[1])
                {
                    modbus506 = modbus_provider;
                }
                else
                {
                    file.DataProvidersModule.DataProvidersRoot.Items.Add(modbus506);
                }

                var factory = new ModbusDataProviderAddressFactory();

                // otlycheniye ping providera na pulte burilshika
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


                            foreach (var tag in sn.Items.Where(x => x.DataProvider != null && x.DataProvider.GetType() == typeof(PingDataProvider)))
                            {
                                tag.DataProvider = null;
                            }
                        });
                    }
                });

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
                            // esli port odinakovii to rabotaem s odnim soketom
                            if (ports[0] != ports[1])
                                tag.DataProvider = modbus506;
                            else
                            {
                                tag.DataProvider = modbus_provider;
                            }

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
            }

            var plc = loadPlcConfig();

            if (plc == ZakazType.zakaz54038)
            {
                // 159

                DisableDiscreteAlarms(file, new[] { "emergency160", "readiness160", "emergency_arctic_light", "readiness_arctic_light" });
                DisableAnanlogAlarms(file, new[] { "vfds160", "vfds_abb" });
                ChangeReadiesFiles(plc, workingDirectory + @"\data\readies_store\159\", workingDirectory + @"\data\readies\");
                LoadTagsForReadies(file, @"\data\readies\", plc);
            }

            if (plc == ZakazType.zakaz54042)
            {
                // 160

                DisableDiscreteAlarms(file, new[] { "emergency", "readiness", "emergency_arctic_light", "readiness_arctic_light" });
                DisableAnanlogAlarms(file, new[] { "vfds", "vfds_abb" });

                ChangeReadiesFiles(plc, workingDirectory + @"\data\readies_store\160\", workingDirectory + @"\data\readies\");
                // zamena statusa
                ChangeMessages(file.MessagesRoot.Nodes["auxcer_160"], file.MessagesRoot.Nodes["auxcer"]);
                ChangeMessages(file.MessagesRoot.Nodes["AuxWinchcer1_160"], file.MessagesRoot.Nodes["AuxWinchcer1"]);
                ChangeMessages(file.MessagesRoot.Nodes["AuxWinchcer2_160"], file.MessagesRoot.Nodes["AuxWinchcer2"]);
                ChangeMessages(file.MessagesRoot.Nodes["drawworkcer_160"], file.MessagesRoot.Nodes["drawworkcer"]);
                ChangeMessages(file.MessagesRoot.Nodes["pump1cer_160"], file.MessagesRoot.Nodes["pump1cer"]);
                ChangeMessages(file.MessagesRoot.Nodes["pump2cer_160"], file.MessagesRoot.Nodes["pump2cer"]);
                ChangeMessages(file.MessagesRoot.Nodes["rotorcer_160"], file.MessagesRoot.Nodes["rotorcer"]);
                ChangeMessages(file.MessagesRoot.Nodes["pump2cer_160"], file.MessagesRoot.Nodes["pump2cer"]);
                ChangeMessages(file.MessagesRoot.Nodes["pump2cer_160"], file.MessagesRoot.Nodes["pump2cer"]);
                ChangeMessages(file.MessagesRoot.Nodes["pump2cer_160"], file.MessagesRoot.Nodes["pump2cer"]);

                var uri = new Uri(Assembly.GetExecutingAssembly().GetName().CodeBase);
                var fileName = Path.GetDirectoryName(uri.LocalPath.ToLower()) + @"\data\complect.txt";
                var buConfig = BUHelper.LoadConfig(fileName);

                LoadTagsForReadies(file, @"\data\readies\", plc);
                BUHelper.ModifyTagsTo160(file, buConfig, ports, firstlow);
            }

            // Razbienie tegov na raznie kontrolleri (ktu1, ktu2)
            if (plc == ZakazType.zakaz54044)
            {
                // Arktika light

                DisableDiscreteAlarms(file, new[] { "emergency", "readiness", "emergency160", "readiness160" });
                // otkluchaem simensovskie avarii i preduprejdenia privodov
                DisableAnanlogAlarms(file, new[] { "vfds" });

                ChangeReadiesFiles(plc, workingDirectory + @"\data\readies_store\54044\", workingDirectory + @"\data\readies\");

                // zamena statusa
                ChangeMessages(file.MessagesRoot.Nodes["auxcer_arctic_light"], file.MessagesRoot.Nodes["auxcer"]);
                ChangeMessages(file.MessagesRoot.Nodes["AuxWinchcer1_arctic_light"], file.MessagesRoot.Nodes["AuxWinchcer1"]);
                ChangeMessages(file.MessagesRoot.Nodes["AuxWinchcer2_arctic_light"], file.MessagesRoot.Nodes["AuxWinchcer2"]);
                ChangeMessages(file.MessagesRoot.Nodes["drawworkcer_arctic_light"], file.MessagesRoot.Nodes["drawworkcer"]);
                ChangeMessages(file.MessagesRoot.Nodes["pump1cer_arctic_light"], file.MessagesRoot.Nodes["pump1cer"]);
                ChangeMessages(file.MessagesRoot.Nodes["pump2cer_arctic_light"], file.MessagesRoot.Nodes["pump2cer"]);
                ChangeMessages(file.MessagesRoot.Nodes["pump3cer_arctic_light"], file.MessagesRoot.Nodes["pump3cer"]);
                ChangeMessages(file.MessagesRoot.Nodes["rotorcer_arctic_light"], file.MessagesRoot.Nodes["rotorcer"]);

                var uri = new Uri(Assembly.GetExecutingAssembly().GetName().CodeBase);
                var fileName = Path.GetDirectoryName(uri.LocalPath.ToLower()) + @"\data\complect.txt";
                var buConfig = BUHelper.LoadConfig(fileName);

                LoadTagsForReadies(file, @"\data\readies\", plc);
                BUHelper.ModifyTagsTo160(file, buConfig, ports, firstlow);
            }

            if (plc == ZakazType.zakaz54056)
            {
                // БНГРЭ

                // то же самое что арктика лайт

                DisableDiscreteAlarms(file, new[] { "emergency", "readiness", "emergency160", "readiness160" });
                // otkluchaem simensovskie avarii i preduprejdenia privodov
                DisableAnanlogAlarms(file, new[] { "vfds" });

                ChangeReadiesFiles(plc, workingDirectory + @"\data\readies_store\54056\", workingDirectory + @"\data\readies\");

                // zamena statusa
                ChangeMessages(file.MessagesRoot.Nodes["auxcer_arctic_light"], file.MessagesRoot.Nodes["auxcer"]);
                ChangeMessages(file.MessagesRoot.Nodes["AuxWinchcer1_arctic_light"], file.MessagesRoot.Nodes["AuxWinchcer1"]);
                ChangeMessages(file.MessagesRoot.Nodes["AuxWinchcer2_arctic_light"], file.MessagesRoot.Nodes["AuxWinchcer2"]);
                ChangeMessages(file.MessagesRoot.Nodes["drawworkcer_arctic_light"], file.MessagesRoot.Nodes["drawworkcer"]);
                ChangeMessages(file.MessagesRoot.Nodes["pump1cer_arctic_light"], file.MessagesRoot.Nodes["pump1cer"]);
                ChangeMessages(file.MessagesRoot.Nodes["pump2cer_arctic_light"], file.MessagesRoot.Nodes["pump2cer"]);
                ChangeMessages(file.MessagesRoot.Nodes["pump3cer_arctic_light"], file.MessagesRoot.Nodes["pump3cer"]);
                ChangeMessages(file.MessagesRoot.Nodes["rotorcer_arctic_light"], file.MessagesRoot.Nodes["rotorcer"]);

                var uri = new Uri(Assembly.GetExecutingAssembly().GetName().CodeBase);
                var fileName = Path.GetDirectoryName(uri.LocalPath.ToLower()) + @"\data\complect.txt";
                var buConfig = BUHelper.LoadConfig(fileName);

                LoadTagsForReadies(file, @"\data\readies\", plc);
                BUHelper.ModifyTagsTo160(file, buConfig, ports, firstlow);
            }
        }

        private void LoadTagsForIO(HmiDataFile file)
        {
            var uri = new Uri(System.Reflection.Assembly.GetExecutingAssembly().GetName().CodeBase);
            var location = Path.GetDirectoryName(Path.GetDirectoryName(uri.LocalPath.ToLower()) + @"\data\io\");

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
            //createReadiesItems(Path.GetDirectoryName(uri.LocalPath.ToLower()) + @"\data\readies\AD_Stop.csv", hmiDataFile.DataProvidersModule.DataProvidersRoot.Items[provider_name], hmiDataFile.TagsModule);

            createReadiesItems(Path.GetDirectoryName(uri.LocalPath.ToLower()) + relativePath + @"AW1_Ready.csv", hmiDataFile.DataProvidersModule.DataProvidersRoot.Items[provider_name], hmiDataFile.TagsModule);
            createReadiesItems(Path.GetDirectoryName(uri.LocalPath.ToLower()) + relativePath + @"AW1_Emer.csv", hmiDataFile.DataProvidersModule.DataProvidersRoot.Items[provider_name], hmiDataFile.TagsModule);
            //createReadiesItems(Path.GetDirectoryName(uri.LocalPath.ToLower()) + @"\data\readies\AW_Stop.csv", hmiDataFile.DataProvidersModule.DataProvidersRoot.Items[provider_name], hmiDataFile.TagsModule);

            createReadiesItems(Path.GetDirectoryName(uri.LocalPath.ToLower()) + relativePath + @"AW2_Ready.csv", hmiDataFile.DataProvidersModule.DataProvidersRoot.Items[provider_name], hmiDataFile.TagsModule);
            createReadiesItems(Path.GetDirectoryName(uri.LocalPath.ToLower()) + relativePath + @"AW2_Emer.csv", hmiDataFile.DataProvidersModule.DataProvidersRoot.Items[provider_name], hmiDataFile.TagsModule);

            createReadiesItems(Path.GetDirectoryName(uri.LocalPath.ToLower()) + relativePath + @"Dr_Ready.csv", hmiDataFile.DataProvidersModule.DataProvidersRoot.Items[provider_name], hmiDataFile.TagsModule);
            createReadiesItems(Path.GetDirectoryName(uri.LocalPath.ToLower()) + relativePath + @"Dr_Emer.csv", hmiDataFile.DataProvidersModule.DataProvidersRoot.Items[provider_name], hmiDataFile.TagsModule);
            //createReadiesItems(Path.GetDirectoryName(uri.LocalPath.ToLower()) + @"\data\readies\Dr_Stop.csv", hmiDataFile.DataProvidersModule.DataProvidersRoot.Items[provider_name], hmiDataFile.TagsModule);

            createReadiesItems(Path.GetDirectoryName(uri.LocalPath.ToLower()) + relativePath + @"P1_Ready.csv", hmiDataFile.DataProvidersModule.DataProvidersRoot.Items[provider_name], hmiDataFile.TagsModule);
            createReadiesItems(Path.GetDirectoryName(uri.LocalPath.ToLower()) + relativePath + @"P1_Emer.csv", hmiDataFile.DataProvidersModule.DataProvidersRoot.Items[provider_name], hmiDataFile.TagsModule);
            //createReadiesItems(Path.GetDirectoryName(uri.LocalPath.ToLower()) + @"\data\readies\P1_Stop.csv", hmiDataFile.DataProvidersModule.DataProvidersRoot.Items[provider_name], hmiDataFile.TagsModule);

            createReadiesItems(Path.GetDirectoryName(uri.LocalPath.ToLower()) + relativePath + @"P2_Ready.csv", hmiDataFile.DataProvidersModule.DataProvidersRoot.Items[provider_name], hmiDataFile.TagsModule);
            createReadiesItems(Path.GetDirectoryName(uri.LocalPath.ToLower()) + relativePath + @"P2_Emer.csv", hmiDataFile.DataProvidersModule.DataProvidersRoot.Items[provider_name], hmiDataFile.TagsModule);
            //createReadiesItems(Path.GetDirectoryName(uri.LocalPath.ToLower()) + @"\data\readies\P2_Stop.csv", hmiDataFile.DataProvidersModule.DataProvidersRoot.Items[provider_name], hmiDataFile.TagsModule);

            createReadiesItems(Path.GetDirectoryName(uri.LocalPath.ToLower()) + relativePath + @"RT_Ready.csv", hmiDataFile.DataProvidersModule.DataProvidersRoot.Items[provider_name], hmiDataFile.TagsModule);
            createReadiesItems(Path.GetDirectoryName(uri.LocalPath.ToLower()) + relativePath + @"RT_Emer.csv", hmiDataFile.DataProvidersModule.DataProvidersRoot.Items[provider_name], hmiDataFile.TagsModule);
            //createReadiesItems(Path.GetDirectoryName(uri.LocalPath.ToLower()) + @"\data\readies\RT_Stop.csv", hmiDataFile.DataProvidersModule.DataProvidersRoot.Items[provider_name], hmiDataFile.TagsModule);

            if (zakaz == ZakazType.zakaz54044 || zakaz == ZakazType.zakaz54056)
            {
                createReadiesItems(Path.GetDirectoryName(uri.LocalPath.ToLower()) + relativePath + @"P3_Ready.csv", hmiDataFile.DataProvidersModule.DataProvidersRoot.Items[provider_name], hmiDataFile.TagsModule);
                createReadiesItems(Path.GetDirectoryName(uri.LocalPath.ToLower()) + relativePath + @"P3_Emer.csv", hmiDataFile.DataProvidersModule.DataProvidersRoot.Items[provider_name], hmiDataFile.TagsModule);
                //createReadiesItems(Path.GetDirectoryName(uri.LocalPath.ToLower()) + relativePath + @"P3_Stop.csv", hmiDataFile.DataProvidersModule.DataProvidersRoot.Items[provider_name], hmiDataFile.TagsModule);
            }
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

        private ZakazType loadPlcConfig()
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
                ((Monokot.Hmi.Core.Messages.MultiMessage)target.Items[item.ID])[0, 0] = item.Text;
            }
        }

        private void DisableDiscreteAlarms(HmiDataFile file, string[] nodes_for_disable)
        {
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
                        }
                    });
                }
            });

        }


        private void DisableAnanlogAlarms(HmiDataFile file, string[] nodes_for_disable)
        {
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
                        }
                    });
                }
            });

        }
    }

    //[JsonConverter(typeof(StringEnumConverter))]
    //public enum ZakazType
    //{
    //    [Description("159")]
    //    zakaz54038,
    //    [Description("160")]
    //    zakaz54042
    //}

    //[JsonConverter(typeof(StringEnumConverter))]
    //public enum Complectations
    //{
    //    [Description("КТУ1 + КТУ2")]
    //    KTU1_KTU2,
    //    [Description("КТУ1")]
    //    KTU1,
    //    [Description("КТУ2")]
    //    KTU2,
    //}
}
