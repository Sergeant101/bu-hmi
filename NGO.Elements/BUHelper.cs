using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using Monokot.Hmi.Core.Framework.Storage;
using Monokot.Hmi.Core.Framework.Trees;
using Monokot.Hmi.Core.Tags;
using Monokot.Hmi.Core.Utils;
using Monokot.Hmi.StdProviders.Modbus;
using Monokot.Hmi.StdProviders.Modbus.Utils;
using Newtonsoft.Json;
using System.IO;
using System.Reflection;

namespace NGO.Elements
{
    public enum BU160_Config
    {
        KTU1_KTU2,
        KTU1,
        KTU2
    }

    public enum SiemensDbName
    {
        DB350,
        DB351
    }

    public class SiemensAddressDescr
    {
        public SiemensDbName DB;
        public int StartByte;
        public int Bit = -1;
    }

    public class BUHelper
    {

        public static SiemensAddressDescr ModbusAddressToSiemens(ModbusDataAddress address)
        {
            if (address.AddressTableInfo == ModbusAddressTableInfo.InputCoils || address.AddressTableInfo == ModbusAddressTableInfo.OutputCoils)
                throw new ArgumentException("AddressTableInfo");

            var result = new SiemensAddressDescr();

            if (address.AddressTableInfo == ModbusAddressTableInfo.InternalRegisters)
                result.DB = SiemensDbName.DB350;
            else if (address.AddressTableInfo == ModbusAddressTableInfo.HoldingRegisters)
                result.DB = SiemensDbName.DB351;

            result.StartByte = (address.StartAddress - 1) * 2;

            // Для булевых тэгов
            if (address.DataTypeInfo == ModbusAddressDataTypeInfo.Boolean)
            {
                if (address.BitIndex > 7)
                    result.Bit = address.BitIndex - 8;
                else
                {
                    result.Bit = address.BitIndex;
                    result.StartByte += 1;
                }
            }

            return result;
        }

        public const string SECOND_PLC_IP = "192.168.1.3";

        #region DB350

        public const int PUMP_MIN_RANGE = 0;
        public const int PUMP_MAX_RANGE = 5999;

        public const int DR_MIN_RANGE = 6000;
        public const int DR_MAX_RANGE = 7999;

        public const int RT_MIN_RANGE = 8000;
        public const int RT_MAX_RANGE = 9999;

        public const int AUX_MIN_RANGE = 10000;
        public const int AUX_MAX_RANGE = 11999;

        public const int DSU_KTU2_MIN_RANGE = 12000;
        public const int DSU_KTU2_MAX_RANGE = 12999;

        public const int DSU_KTU1_MIN_RANGE = 13000;
        public const int DSU_KTU1_MAX_RANGE = 13999;

        public const int AW1_MIN_RANGE = 14000;
        public const int AW1_MAX_RANGE = 14199;

        public const int BKT_KRU_MIN_RANGE = 14200;
        public const int BKT_KRU_MAX_RANGE = 15999;

        public const int GRANGE_1_MIN_RANGE = 16000;
        public const int GRANGE_1_MAX_RANGE = 16188;

        public const int GRANGE_2_MIN_RANGE = 16192;
        public const int GRANGE_2_MAX_RANGE = 16380;

        public const int GRANGE_3_MIN_RANGE = 16384;
        public const int GRANGE_3_MAX_RANGE = 16404;

        public const int GRANGE_4_MIN_RANGE = 16408;
        public const int GRANGE_4_MAX_RANGE = 16452;

        public const int GRANGE_5_MIN_RANGE = 16456;
        public const int GRANGE_5_MAX_RANGE = 16548;

        public const int GRANGE_6_MIN_RANGE = 16552;
        public const int GRANGE_6_MAX_RANGE = 16620;

        public const int GRANGE_7_MIN_RANGE = 16624;
        public const int GRANGE_7_MAX_RANGE = 16764;

        public const int GRANGE_8_MIN_RANGE = 16768;
        public const int GRANGE_8_MAX_RANGE = 16980;

        public const int SLD_MIN_RANGE = 17000;
        public const int SLD_MAX_RANGE = 17039;

        public const int MODEL_1_MIN_RANGE = 17040;
        public const int MODEL_1_MAX_RANGE = 17048;

        public const int MODEL_2_MIN_RANGE = 17050;
        public const int MODEL_2_MAX_RANGE = 17051;

        public const int BP_18_000_RANGE = 18000;
        public const int BP_18_001_RANGE = 18001;
        public const int BP_18_002_RANGE = 18002;
        public const int BP_18_003_RANGE = 18003;
        public const int BP_18_004_RANGE = 18004;
        public const int BP_18_005_RANGE = 18005;
        public const int BP_18_006_RANGE = 18006;
        public const int BP_18_007_RANGE = 18007;
        public const int BP_18_008_RANGE = 18008;
        public const int BP_18_009_RANGE = 18009;

        public const int XX_18_028_RANGE = 18028;
        public const int XX_18_029_RANGE = 18029;
        public const int XX_18_030_RANGE = 18030;
        public const int XX_18_031_RANGE = 18031;
        public const int XX_18_032_RANGE = 18032;
        public const int XX_18_033_RANGE = 18033;

        public const int CLIMAT_MIN_RANGE = 18046;
        public const int CLIMAT_MAX_RANGE = 18056;

        public const int TAR_MIN_RANGE = 20000;
        public const int TAR_MAX_RANGE = 20150;

        public const int TIMER_1_MIN_RANGE = 21000;
        public const int TIMER_1_MAX_RANGE = 21020;

        public const int TIMER_2_MIN_RANGE = 21024;
        public const int TIMER_2_MAX_RANGE = 21072;

        public const int IO_1_MIN_RANGE = 22000;
        public const int IO_1_MAX_RANGE = 22021;

        public const int IO_2_MIN_RANGE = 22022;
        public const int IO_2_MAX_RANGE = 22027;

        public const int IO_3_MIN_RANGE = 22028;
        public const int IO_3_MAX_RANGE = 22039;

        public const int IO_4_MIN_RANGE = 22040;
        public const int IO_4_MAX_RANGE = 22043;

        public const int IO_5_MIN_RANGE = 22044;
        public const int IO_5_MAX_RANGE = 22065;

        public const int IO_VAL_1_MIN_RANGE = 22300;
        public const int IO_VAL_1_MAX_RANGE = 22376;

        public const int IO_VAL_2_MIN_RANGE = 22380;
        public const int IO_VAL_2_MAX_RANGE = 22384;

        public const int IO_VAL_3_MIN_RANGE = 22388;
        public const int IO_VAL_3_MAX_RANGE = 22999;

        public const int IO_VAL_4_MIN_RANGE = 23000;
        public const int IO_VAL_4_MAX_RANGE = 23076;

        public const int IO_VAL_5_MIN_RANGE = 23080;
        public const int IO_VAL_5_MAX_RANGE = 23084;

        public const int IO_VAL_6_MIN_RANGE = 23088;
        public const int IO_VAL_6_MAX_RANGE = 23899;

        public const int SERVICE_MIN_RANGE = 23900;
        public const int SERVICE_MAX_RANGE = 23999;

        public const int MSAB_MIN_RANGE = 24000;
        public const int MSAB_MAX_RANGE = 25999;

        public const int TDS_MIN_RANGE = 26000;
        public const int TDS_MAX_RANGE = 27999;

        #endregion

        #region DB351

        public const int BTN_DC_0000_RANGE = 0;
        public const int BTN_DC_0001_RANGE = 1;
        public const int BTN_DC_0002_RANGE = 2;
        public const int BTN_DC_0003_RANGE = 3;
        public const int BTN_DC_0004_RANGE = 4;
        public const int BTN_DC_0005_RANGE = 5;
        public const int BTN_DC_0010_RANGE = 10;
        public const int BTN_DC_0011_RANGE = 11;
        public const int BTN_DC_0012_RANGE = 12;
        public const int BTN_DC_0013_RANGE = 13;

        public const int BTN_BP_2000_RANGE = 2000;
        public const int BTN_BP_2001_RANGE = 2001;
        public const int BTN_BP_2002_RANGE = 2002;
        public const int BTN_BP_2003_RANGE = 2003;
        public const int BTN_BP_2004_RANGE = 2004;
        public const int BTN_BP_2005_RANGE = 2005;
        public const int BTN_BP_2006_RANGE = 2006;
        public const int BTN_BP_2007_RANGE = 2007;
        public const int BTN_BP_2008_RANGE = 2008;
        public const int BTN_BP_2009_RANGE = 2009;

        public const int BTN_CLIMAT_MIN_RANGE = 4000;
        public const int BTN_CLIMAT_MAX_RANGE = 4002;

        public const int BTN_SETUP_1_MIN_RANGE = 6000;
        public const int BTN_SETUP_1_MAX_RANGE = 6018;

        public const int BTN_SETUP_2_MIN_RANGE = 6020;
        public const int BTN_SETUP_2_MAX_RANGE = 6024;

        public const int BTN_SETUP_3_MIN_RANGE = 6026;
        public const int BTN_SETUP_3_MAX_RANGE = 6062;

        public const int BTN_TAR_MIN_RANGE = 8000;
        public const int BTN_TAR_MAX_RANGE = 8180;

        public const int BTN_TIME_1_MIN_RANGE = 9000;
        public const int BTN_TIME_1_MAX_RANGE = 9020;

        public const int BTN_TIME_2_MIN_RANGE = 9024;
        public const int BTN_TIME_2_MAX_RANGE = 9076;

        public const int BTN_SENSOR_1_MIN_RANGE = 10000;
        public const int BTN_SENSOR_1_MAX_RANGE = 10188;

        public const int BTN_SENSOR_2_MIN_RANGE = 10192;
        public const int BTN_SENSOR_2_MAX_RANGE = 10380;

        public const int BTN_SENSOR_3_MIN_RANGE = 10384;
        public const int BTN_SENSOR_3_MAX_RANGE = 10404;

        public const int BTN_SENSOR_4_MIN_RANGE = 10408;
        public const int BTN_SENSOR_4_MAX_RANGE = 10452;

        public const int BTN_SENSOR_5_MIN_RANGE = 10456;
        public const int BTN_SENSOR_5_MAX_RANGE = 10548;

        public const int BTN_SENSOR_6_MIN_RANGE = 10552;
        public const int BTN_SENSOR_6_MAX_RANGE = 10620;

        public const int BTN_SENSOR_7_MIN_RANGE = 10624;
        public const int BTN_SENSOR_7_MAX_RANGE = 10764;

        public const int BTN_SENSOR_8_MIN_RANGE = 10768;
        public const int BTN_SENSOR_8_MAX_RANGE = 10980;

        public const int BTN_SERVICE_MIN_RANGE = 12000;
        public const int BTN_SERVICE_MAX_RANGE = 12022;

        public const int BTN_MSAB_MIN_RANGE = 14000;
        public const int BTN_MSAB_MAX_RANGE = 24168;

        #endregion

        public static BU160_Config LoadConfig(string fileName)
        {
            try
            {
                var text = File.ReadAllText(fileName);
                return JsonConvert.DeserializeObject<BU160_Config>(text);
            }
            catch (Exception x)
            {
                return BU160_Config.KTU1_KTU2;
            }
        }

        private static void SetModbusProviderHelper(HmiTag tag, ushort[] ports, ModbusDataProvider first, ModbusDataProvider second)
        {
            var modp = (ModbusDataProvider)tag.DataProvider;
            if (modp.Port == ports[0])
                tag.DataProvider = first;
            else if (modp.Port == ports[1])
                tag.DataProvider = second;
            else throw new InvalidConstraintException("Bad Port");
        }

        public static void ModifyTagsTo160(HmiDataFile file, BU160_Config config, ushort[] ports, bool firstlow)
        {

            var modbusIp3Port0 = new ModbusDataProvider("modbus_ip3_port0", file.DataProvidersModule.DataProvidersRoot)
            {
                Host = SECOND_PLC_IP,
                Port = ports[0],
                InternalRegistersCount = 120,
                FirstWordLowIn32Bit = firstlow
            };

            file.DataProvidersModule.DataProvidersRoot.Items.Add(modbusIp3Port0);

            var p1 = modbusIp3Port0.GetType().GetProperty("IsLicensed", BindingFlags.NonPublic | BindingFlags.Instance | BindingFlags.SetField);
            p1.SetValue(modbusIp3Port0, true, null);

            var modbusIp3Port1 = new ModbusDataProvider("modbus_ip3_port1", file.DataProvidersModule.DataProvidersRoot)
            {
                Host = SECOND_PLC_IP,
                Port = ports[1],
                InternalRegistersCount = 120,
                FirstWordLowIn32Bit = firstlow
            };


            if (ports[0] == ports[1])
            {
                modbusIp3Port1 = modbusIp3Port0;
            }
            else
            {
                file.DataProvidersModule.DataProvidersRoot.Items.Add(modbusIp3Port1);

                var p2 = modbusIp3Port0.GetType().GetProperty("IsLicensed", BindingFlags.NonPublic | BindingFlags.Instance | BindingFlags.SetField);
                p2.SetValue(modbusIp3Port0, true, null);
            }

            HmiUtils.RecursiveNodeAction<TagsHmiNode, string, HmiTag>(file.TagsRoot, n =>
            {
                foreach (var tag in n.Items)
                {
                    var mbAdr = tag.DataAddress as ModbusDataAddress;
                    if (tag.DataProvider is ModbusDataProvider && mbAdr != null)
                    {
                        var currentMbProv = (ModbusDataProvider)tag.DataProvider;

                        switch (config)
                        {
                            case BU160_Config.KTU1_KTU2:
                            case BU160_Config.KTU1:
                                SetModbusProviderHelper(tag, ports, modbusIp3Port0, modbusIp3Port1);
                                break;

                            case BU160_Config.KTU2:
                                break;
                            default:
                                throw new ArgumentOutOfRangeException("config", config, null);
                        }


                        //// Вычисляем эквивалент для сименсовского адреса
                        //var simAdr = ModbusAddressToSiemens(mbAdr);

                        //#region DB350

                        //if (simAdr.DB == SiemensDbName.DB350)
                        //{

                        //    if ((simAdr.StartByte >= PUMP_MIN_RANGE && simAdr.StartByte <= PUMP_MAX_RANGE)
                        //        || (simAdr.StartByte >= DSU_KTU1_MIN_RANGE && simAdr.StartByte <= DSU_KTU1_MAX_RANGE)
                        //        || (simAdr.StartByte >= BKT_KRU_MIN_RANGE && simAdr.StartByte <= BKT_KRU_MAX_RANGE))
                        //    {
                        //        switch (config)
                        //        {
                        //            case BU160_Config.KTU1_KTU2:
                        //            case BU160_Config.KTU1:
                        //                SetModbusProviderHelper(tag, ports, modbusIp3Port0, modbusIp3Port1);
                        //                break;

                        //            case BU160_Config.KTU2:
                        //                break;
                        //            default:
                        //                throw new ArgumentOutOfRangeException("config", config, null);
                        //        }
                        //    }
                        //    else if ((simAdr.StartByte >= DR_MIN_RANGE && simAdr.StartByte <= DR_MAX_RANGE)
                        //             || (simAdr.StartByte >= RT_MIN_RANGE && simAdr.StartByte <= RT_MAX_RANGE)
                        //             || (simAdr.StartByte >= AUX_MIN_RANGE && simAdr.StartByte <= AUX_MAX_RANGE)
                        //             || (simAdr.StartByte >= DSU_KTU2_MIN_RANGE && simAdr.StartByte <= DSU_KTU2_MAX_RANGE)
                        //             || (simAdr.StartByte >= AW1_MIN_RANGE && simAdr.StartByte <= AW1_MAX_RANGE))
                        //    {
                        //        switch (config)
                        //        {
                        //            case BU160_Config.KTU1:
                        //                SetModbusProviderHelper(tag, ports, modbusIp3Port0, modbusIp3Port1);
                        //                break;

                        //            case BU160_Config.KTU1_KTU2:
                        //            case BU160_Config.KTU2:
                        //                break;
                        //            default:
                        //                throw new ArgumentOutOfRangeException("config", config, null);
                        //        }
                        //    }

                        //    #region Диапазоны датчиков

                        //    else if (simAdr.StartByte >= GRANGE_1_MIN_RANGE && simAdr.StartByte <= GRANGE_1_MAX_RANGE)
                        //    {
                        //        switch (config)
                        //        {
                        //            case BU160_Config.KTU1_KTU2:
                        //                SetModbusProviderHelper(tag, ports, modbusIp3Port0, modbusIp3Port1);
                        //                break;

                        //            case BU160_Config.KTU1:
                        //                SetModbusProviderHelper(tag, ports, modbusIp3Port0, modbusIp3Port1);
                        //                break;

                        //            case BU160_Config.KTU2:
                        //                break;

                        //            default:
                        //                throw new ArgumentOutOfRangeException("config", config, null);
                        //        }
                        //    }
                        //    else if (simAdr.StartByte >= GRANGE_2_MIN_RANGE && simAdr.StartByte <= GRANGE_2_MAX_RANGE)
                        //    {
                        //        switch (config)
                        //        {
                        //            case BU160_Config.KTU1_KTU2:
                        //                break;

                        //            case BU160_Config.KTU1:
                        //                SetModbusProviderHelper(tag, ports, modbusIp3Port0, modbusIp3Port1);
                        //                break;

                        //            case BU160_Config.KTU2:
                        //                break;

                        //            default:
                        //                throw new ArgumentOutOfRangeException("config", config, null);
                        //        }
                        //    }
                        //    else if (simAdr.StartByte >= GRANGE_3_MIN_RANGE && simAdr.StartByte <= GRANGE_3_MAX_RANGE)
                        //    {
                        //        switch (config)
                        //        {
                        //            case BU160_Config.KTU1_KTU2:
                        //                SetModbusProviderHelper(tag, ports, modbusIp3Port0, modbusIp3Port1);
                        //                break;

                        //            case BU160_Config.KTU1:
                        //                SetModbusProviderHelper(tag, ports, modbusIp3Port0, modbusIp3Port1);
                        //                break;

                        //            case BU160_Config.KTU2:
                        //                break;

                        //            default:
                        //                throw new ArgumentOutOfRangeException("config", config, null);
                        //        }
                        //    }
                        //    else if (simAdr.StartByte >= GRANGE_4_MIN_RANGE && simAdr.StartByte <= GRANGE_4_MAX_RANGE)
                        //    {
                        //        switch (config)
                        //        {
                        //            case BU160_Config.KTU1_KTU2:
                        //                break;

                        //            case BU160_Config.KTU1:
                        //                SetModbusProviderHelper(tag, ports, modbusIp3Port0, modbusIp3Port1);
                        //                break;

                        //            case BU160_Config.KTU2:
                        //                break;

                        //            default:
                        //                throw new ArgumentOutOfRangeException("config", config, null);
                        //        }
                        //    }
                        //    else if (simAdr.StartByte >= GRANGE_5_MIN_RANGE && simAdr.StartByte <= GRANGE_5_MAX_RANGE)
                        //    {
                        //        switch (config)
                        //        {
                        //            case BU160_Config.KTU1_KTU2:
                        //                SetModbusProviderHelper(tag, ports, modbusIp3Port0, modbusIp3Port1);
                        //                break;

                        //            case BU160_Config.KTU1:
                        //                SetModbusProviderHelper(tag, ports, modbusIp3Port0, modbusIp3Port1);
                        //                break;

                        //            case BU160_Config.KTU2:
                        //                break;

                        //            default:
                        //                throw new ArgumentOutOfRangeException("config", config, null);
                        //        }
                        //    }
                        //    else if (simAdr.StartByte >= GRANGE_6_MIN_RANGE && simAdr.StartByte <= GRANGE_6_MAX_RANGE)
                        //    {
                        //        switch (config)
                        //        {
                        //            case BU160_Config.KTU1_KTU2:
                        //                break;

                        //            case BU160_Config.KTU1:
                        //                SetModbusProviderHelper(tag, ports, modbusIp3Port0, modbusIp3Port1);
                        //                break;

                        //            case BU160_Config.KTU2:
                        //                break;

                        //            default:
                        //                throw new ArgumentOutOfRangeException("config", config, null);
                        //        }
                        //    }
                        //    else if (simAdr.StartByte >= GRANGE_7_MIN_RANGE && simAdr.StartByte <= GRANGE_7_MAX_RANGE)
                        //    {
                        //        switch (config)
                        //        {
                        //            case BU160_Config.KTU1_KTU2:
                        //                SetModbusProviderHelper(tag, ports, modbusIp3Port0, modbusIp3Port1);
                        //                break;

                        //            case BU160_Config.KTU1:
                        //                SetModbusProviderHelper(tag, ports, modbusIp3Port0, modbusIp3Port1);
                        //                break;

                        //            case BU160_Config.KTU2:
                        //                break;

                        //            default:
                        //                throw new ArgumentOutOfRangeException("config", config, null);
                        //        }
                        //    }
                        //    else if (simAdr.StartByte >= GRANGE_8_MIN_RANGE && simAdr.StartByte <= GRANGE_8_MAX_RANGE)
                        //    {
                        //        switch (config)
                        //        {
                        //            case BU160_Config.KTU1_KTU2:
                        //                break;

                        //            case BU160_Config.KTU1:
                        //                SetModbusProviderHelper(tag, ports, modbusIp3Port0, modbusIp3Port1);
                        //                break;

                        //            case BU160_Config.KTU2:
                        //                break;

                        //            default:
                        //                throw new ArgumentOutOfRangeException("config", config, null);
                        //        }

                        //        #endregion

                        //    }
                        //    else if (simAdr.StartByte >= SLD_MIN_RANGE && simAdr.StartByte <= SLD_MAX_RANGE)
                        //    {
                        //        switch (config)
                        //        {
                        //            case BU160_Config.KTU1_KTU2:
                        //                SetModbusProviderHelper(tag, ports, modbusIp3Port0, modbusIp3Port1);
                        //                break;

                        //            case BU160_Config.KTU1:
                        //                SetModbusProviderHelper(tag, ports, modbusIp3Port0, modbusIp3Port1);
                        //                break;

                        //            case BU160_Config.KTU2:
                        //                break;

                        //            default:
                        //                throw new ArgumentOutOfRangeException("config", config, null);
                        //        }
                        //    }
                        //    else if (simAdr.StartByte >= MODEL_1_MIN_RANGE && simAdr.StartByte <= MODEL_1_MAX_RANGE)
                        //    {
                        //        switch (config)
                        //        {
                        //            case BU160_Config.KTU1_KTU2:
                        //                SetModbusProviderHelper(tag, ports, modbusIp3Port0, modbusIp3Port1);
                        //                break;

                        //            case BU160_Config.KTU1:
                        //                SetModbusProviderHelper(tag, ports, modbusIp3Port0, modbusIp3Port1);
                        //                break;

                        //            case BU160_Config.KTU2:
                        //                break;

                        //            default:
                        //                throw new ArgumentOutOfRangeException("config", config, null);
                        //        }
                        //    }
                        //    else if (simAdr.StartByte >= MODEL_2_MIN_RANGE && simAdr.StartByte <= MODEL_2_MAX_RANGE)
                        //    {
                        //        switch (config)
                        //        {
                        //            case BU160_Config.KTU1_KTU2:
                        //                break;

                        //            case BU160_Config.KTU1:
                        //                SetModbusProviderHelper(tag, ports, modbusIp3Port0, modbusIp3Port1);
                        //                break;

                        //            case BU160_Config.KTU2:
                        //                break;

                        //            default:
                        //                throw new ArgumentOutOfRangeException("config", config, null);
                        //        }
                        //    }

                        //    #region Байпасы

                        //    else if (simAdr.StartByte >= BP_18_000_RANGE && simAdr.StartByte <= BP_18_001_RANGE)
                        //    {
                        //        switch (config)
                        //        {
                        //            case BU160_Config.KTU1_KTU2:
                        //                SetModbusProviderHelper(tag, ports, modbusIp3Port0, modbusIp3Port1);
                        //                break;

                        //            case BU160_Config.KTU1:
                        //                SetModbusProviderHelper(tag, ports, modbusIp3Port0, modbusIp3Port1);
                        //                break;

                        //            case BU160_Config.KTU2:
                        //                break;

                        //            default:
                        //                throw new ArgumentOutOfRangeException("config", config, null);
                        //        }
                        //    }
                        //    else if (simAdr.StartByte >= BP_18_002_RANGE && simAdr.StartByte <= BP_18_003_RANGE)
                        //    {
                        //        switch (config)
                        //        {
                        //            case BU160_Config.KTU1_KTU2:
                        //                break;

                        //            case BU160_Config.KTU1:
                        //                SetModbusProviderHelper(tag, ports, modbusIp3Port0, modbusIp3Port1);
                        //                break;

                        //            case BU160_Config.KTU2:
                        //                break;

                        //            default:
                        //                throw new ArgumentOutOfRangeException("config", config, null);
                        //        }
                        //    }
                        //    else if (simAdr.StartByte == BP_18_004_RANGE && simAdr.Bit >= 0 && simAdr.Bit <= 2)
                        //    {
                        //        switch (config)
                        //        {
                        //            case BU160_Config.KTU1_KTU2:
                        //                break;

                        //            case BU160_Config.KTU1:
                        //                SetModbusProviderHelper(tag, ports, modbusIp3Port0, modbusIp3Port1);
                        //                break;

                        //            case BU160_Config.KTU2:
                        //                break;

                        //            default:
                        //                throw new ArgumentOutOfRangeException("config", config, null);
                        //        }
                        //    }
                        //    else if (simAdr.StartByte == BP_18_004_RANGE && simAdr.Bit >= 3 && simAdr.Bit <= 7)
                        //    {
                        //        switch (config)
                        //        {
                        //            case BU160_Config.KTU1_KTU2:
                        //                SetModbusProviderHelper(tag, ports, modbusIp3Port0, modbusIp3Port1);
                        //                break;

                        //            case BU160_Config.KTU1:
                        //                SetModbusProviderHelper(tag, ports, modbusIp3Port0, modbusIp3Port1);
                        //                break;

                        //            case BU160_Config.KTU2:
                        //                break;

                        //            default:
                        //                throw new ArgumentOutOfRangeException("config", config, null);
                        //        }
                        //    }
                        //    else if (simAdr.StartByte == BP_18_005_RANGE)
                        //    {
                        //        switch (config)
                        //        {
                        //            case BU160_Config.KTU1_KTU2:
                        //                break;

                        //            case BU160_Config.KTU1:
                        //                SetModbusProviderHelper(tag, ports, modbusIp3Port0, modbusIp3Port1);
                        //                break;

                        //            case BU160_Config.KTU2:
                        //                break;

                        //            default:
                        //                throw new ArgumentOutOfRangeException("config", config, null);
                        //        }
                        //    }
                        //    else if (simAdr.StartByte == BP_18_006_RANGE && simAdr.Bit >= 0 && simAdr.Bit <= 2)
                        //    {
                        //        switch (config)
                        //        {
                        //            case BU160_Config.KTU1_KTU2:
                        //                SetModbusProviderHelper(tag, ports, modbusIp3Port0, modbusIp3Port1);
                        //                break;

                        //            case BU160_Config.KTU1:
                        //                SetModbusProviderHelper(tag, ports, modbusIp3Port0, modbusIp3Port1);
                        //                break;

                        //            case BU160_Config.KTU2:
                        //                break;

                        //            default:
                        //                throw new ArgumentOutOfRangeException("config", config, null);
                        //        }
                        //    }
                        //    else if (simAdr.StartByte == BP_18_006_RANGE && simAdr.Bit == 3)
                        //    {
                        //        switch (config)
                        //        {
                        //            case BU160_Config.KTU1_KTU2:
                        //                break;

                        //            case BU160_Config.KTU1:
                        //                SetModbusProviderHelper(tag, ports, modbusIp3Port0, modbusIp3Port1);
                        //                break;

                        //            case BU160_Config.KTU2:
                        //                break;

                        //            default:
                        //                throw new ArgumentOutOfRangeException("config", config, null);
                        //        }
                        //    }
                        //    else if (simAdr.StartByte == BP_18_006_RANGE && simAdr.Bit >= 4)
                        //    {
                        //        switch (config)
                        //        {
                        //            case BU160_Config.KTU1_KTU2:
                        //                SetModbusProviderHelper(tag, ports, modbusIp3Port0, modbusIp3Port1);
                        //                break;

                        //            case BU160_Config.KTU1:
                        //                SetModbusProviderHelper(tag, ports, modbusIp3Port0, modbusIp3Port1);
                        //                break;

                        //            case BU160_Config.KTU2:
                        //                break;

                        //            default:
                        //                throw new ArgumentOutOfRangeException("config", config, null);
                        //        }
                        //    }
                        //    else if (simAdr.StartByte == BP_18_007_RANGE && simAdr.Bit == 0)
                        //    {
                        //        switch (config)
                        //        {
                        //            case BU160_Config.KTU1_KTU2:
                        //                SetModbusProviderHelper(tag, ports, modbusIp3Port0, modbusIp3Port1);
                        //                break;

                        //            case BU160_Config.KTU1:
                        //                SetModbusProviderHelper(tag, ports, modbusIp3Port0, modbusIp3Port1);
                        //                break;

                        //            case BU160_Config.KTU2:
                        //                break;

                        //            default:
                        //                throw new ArgumentOutOfRangeException("config", config, null);
                        //        }
                        //    }
                        //    else if (simAdr.StartByte == BP_18_007_RANGE && simAdr.Bit >= 1 && simAdr.Bit <= 2)
                        //    {
                        //        switch (config)
                        //        {
                        //            case BU160_Config.KTU1_KTU2:
                        //                break;

                        //            case BU160_Config.KTU1:
                        //                SetModbusProviderHelper(tag, ports, modbusIp3Port0, modbusIp3Port1);
                        //                break;

                        //            case BU160_Config.KTU2:
                        //                break;

                        //            default:
                        //                throw new ArgumentOutOfRangeException("config", config, null);
                        //        }
                        //    }
                        //    else if (simAdr.StartByte == BP_18_007_RANGE && simAdr.Bit >= 3 && simAdr.Bit <= 6)
                        //    {
                        //        switch (config)
                        //        {
                        //            case BU160_Config.KTU1_KTU2:
                        //                SetModbusProviderHelper(tag, ports, modbusIp3Port0, modbusIp3Port1);
                        //                break;

                        //            case BU160_Config.KTU1:
                        //                SetModbusProviderHelper(tag, ports, modbusIp3Port0, modbusIp3Port1);
                        //                break;

                        //            case BU160_Config.KTU2:
                        //                break;

                        //            default:
                        //                throw new ArgumentOutOfRangeException("config", config, null);
                        //        }
                        //    }
                        //    else if (simAdr.StartByte == BP_18_007_RANGE && simAdr.Bit == 7)
                        //    {
                        //        switch (config)
                        //        {
                        //            case BU160_Config.KTU1_KTU2:
                        //                break;

                        //            case BU160_Config.KTU1:
                        //                SetModbusProviderHelper(tag, ports, modbusIp3Port0, modbusIp3Port1);
                        //                break;

                        //            case BU160_Config.KTU2:
                        //                break;

                        //            default:
                        //                throw new ArgumentOutOfRangeException("config", config, null);
                        //        }
                        //    }
                        //    else if (simAdr.StartByte == BP_18_008_RANGE && simAdr.Bit >= 0 && simAdr.Bit <= 1)
                        //    {
                        //        switch (config)
                        //        {
                        //            case BU160_Config.KTU1_KTU2:
                        //                break;

                        //            case BU160_Config.KTU1:
                        //                SetModbusProviderHelper(tag, ports, modbusIp3Port0, modbusIp3Port1);
                        //                break;

                        //            case BU160_Config.KTU2:
                        //                break;

                        //            default:
                        //                throw new ArgumentOutOfRangeException("config", config, null);
                        //        }
                        //    }
                        //    else if (simAdr.StartByte == BP_18_008_RANGE && simAdr.Bit >= 2 && simAdr.Bit <= 5)
                        //    {
                        //        switch (config)
                        //        {
                        //            case BU160_Config.KTU1_KTU2:
                        //                SetModbusProviderHelper(tag, ports, modbusIp3Port0, modbusIp3Port1);
                        //                break;

                        //            case BU160_Config.KTU1:
                        //                SetModbusProviderHelper(tag, ports, modbusIp3Port0, modbusIp3Port1);
                        //                break;

                        //            case BU160_Config.KTU2:
                        //                break;

                        //            default:
                        //                throw new ArgumentOutOfRangeException("config", config, null);
                        //        }
                        //    }
                        //    else if (simAdr.StartByte == BP_18_008_RANGE && simAdr.Bit >= 6 && simAdr.Bit <= 7)
                        //    {
                        //        switch (config)
                        //        {
                        //            case BU160_Config.KTU1_KTU2:
                        //                break;

                        //            case BU160_Config.KTU1:
                        //                SetModbusProviderHelper(tag, ports, modbusIp3Port0, modbusIp3Port1);
                        //                break;

                        //            case BU160_Config.KTU2:
                        //                break;

                        //            default:
                        //                throw new ArgumentOutOfRangeException("config", config, null);
                        //        }
                        //    }
                        //    else if (simAdr.StartByte == BP_18_009_RANGE)
                        //    {
                        //        switch (config)
                        //        {
                        //            case BU160_Config.KTU1_KTU2:
                        //                break;

                        //            case BU160_Config.KTU1:
                        //                SetModbusProviderHelper(tag, ports, modbusIp3Port0, modbusIp3Port1);
                        //                break;

                        //            case BU160_Config.KTU2:
                        //                break;

                        //            default:
                        //                throw new ArgumentOutOfRangeException("config", config, null);
                        //        }
                        //    }

                        //    #endregion

                        //    #region XX и КЗ

                        //    else if (simAdr.StartByte == XX_18_028_RANGE)
                        //    {
                        //        switch (config)
                        //        {
                        //            case BU160_Config.KTU1_KTU2:
                        //                break;

                        //            case BU160_Config.KTU1:
                        //                SetModbusProviderHelper(tag, ports, modbusIp3Port0, modbusIp3Port1);
                        //                break;

                        //            case BU160_Config.KTU2:
                        //                break;

                        //            default:
                        //                throw new ArgumentOutOfRangeException("config", config, null);
                        //        }
                        //    }
                        //    else if (simAdr.StartByte == XX_18_029_RANGE)
                        //    {
                        //        switch (config)
                        //        {
                        //            case BU160_Config.KTU1_KTU2:
                        //                SetModbusProviderHelper(tag, ports, modbusIp3Port0, modbusIp3Port1);
                        //                break;

                        //            case BU160_Config.KTU1:
                        //                SetModbusProviderHelper(tag, ports, modbusIp3Port0, modbusIp3Port1);
                        //                break;

                        //            case BU160_Config.KTU2:
                        //                break;

                        //            default:
                        //                throw new ArgumentOutOfRangeException("config", config, null);
                        //        }
                        //    }
                        //    else if (simAdr.StartByte == XX_18_030_RANGE && simAdr.Bit >= 0 && simAdr.Bit <= 1)
                        //    {
                        //        switch (config)
                        //        {
                        //            case BU160_Config.KTU1_KTU2:
                        //                break;

                        //            case BU160_Config.KTU1:
                        //                SetModbusProviderHelper(tag, ports, modbusIp3Port0, modbusIp3Port1);
                        //                break;

                        //            case BU160_Config.KTU2:
                        //                break;

                        //            default:
                        //                throw new ArgumentOutOfRangeException("config", config, null);
                        //        }
                        //    }
                        //    else if (simAdr.StartByte == XX_18_030_RANGE && simAdr.Bit >= 2 && simAdr.Bit <= 7)
                        //    {
                        //        switch (config)
                        //        {
                        //            case BU160_Config.KTU1_KTU2:
                        //                SetModbusProviderHelper(tag, ports, modbusIp3Port0, modbusIp3Port1);
                        //                break;

                        //            case BU160_Config.KTU1:
                        //                SetModbusProviderHelper(tag, ports, modbusIp3Port0, modbusIp3Port1);
                        //                break;

                        //            case BU160_Config.KTU2:
                        //                break;

                        //            default:
                        //                throw new ArgumentOutOfRangeException("config", config, null);
                        //        }
                        //    }
                        //    else if (simAdr.StartByte == XX_18_031_RANGE && simAdr.Bit == 0)
                        //    {
                        //        switch (config)
                        //        {
                        //            case BU160_Config.KTU1_KTU2:
                        //                SetModbusProviderHelper(tag, ports, modbusIp3Port0, modbusIp3Port1);
                        //                break;

                        //            case BU160_Config.KTU1:
                        //                SetModbusProviderHelper(tag, ports, modbusIp3Port0, modbusIp3Port1);
                        //                break;

                        //            case BU160_Config.KTU2:
                        //                break;

                        //            default:
                        //                throw new ArgumentOutOfRangeException("config", config, null);
                        //        }
                        //    }
                        //    else if (simAdr.StartByte == XX_18_031_RANGE && simAdr.Bit <= 2)
                        //    {
                        //        switch (config)
                        //        {
                        //            case BU160_Config.KTU1_KTU2:
                        //                break;

                        //            case BU160_Config.KTU1:
                        //                SetModbusProviderHelper(tag, ports, modbusIp3Port0, modbusIp3Port1);
                        //                break;

                        //            case BU160_Config.KTU2:
                        //                break;

                        //            default:
                        //                throw new ArgumentOutOfRangeException("config", config, null);
                        //        }
                        //    }
                        //    else if (simAdr.StartByte == XX_18_031_RANGE && simAdr.Bit >= 3 && simAdr.Bit <= 6)
                        //    {
                        //        switch (config)
                        //        {
                        //            case BU160_Config.KTU1_KTU2:
                        //                SetModbusProviderHelper(tag, ports, modbusIp3Port0, modbusIp3Port1);
                        //                break;

                        //            case BU160_Config.KTU1:
                        //                SetModbusProviderHelper(tag, ports, modbusIp3Port0, modbusIp3Port1);
                        //                break;

                        //            case BU160_Config.KTU2:
                        //                break;

                        //            default:
                        //                throw new ArgumentOutOfRangeException("config", config, null);
                        //        }
                        //    }
                        //    else if (simAdr.StartByte == XX_18_031_RANGE && simAdr.Bit == 7)
                        //    {
                        //        switch (config)
                        //        {
                        //            case BU160_Config.KTU1_KTU2:
                        //                break;

                        //            case BU160_Config.KTU1:
                        //                SetModbusProviderHelper(tag, ports, modbusIp3Port0, modbusIp3Port1);
                        //                break;

                        //            case BU160_Config.KTU2:
                        //                break;

                        //            default:
                        //                throw new ArgumentOutOfRangeException("config", config, null);
                        //        }
                        //    }
                        //    else if (simAdr.StartByte == XX_18_032_RANGE)
                        //    {
                        //        switch (config)
                        //        {
                        //            case BU160_Config.KTU1_KTU2:
                        //                break;

                        //            case BU160_Config.KTU1:
                        //                SetModbusProviderHelper(tag, ports, modbusIp3Port0, modbusIp3Port1);
                        //                break;

                        //            case BU160_Config.KTU2:
                        //                break;

                        //            default:
                        //                throw new ArgumentOutOfRangeException("config", config, null);
                        //        }
                        //    }
                        //    else if (simAdr.StartByte == XX_18_033_RANGE)
                        //    {
                        //        switch (config)
                        //        {
                        //            case BU160_Config.KTU1_KTU2:
                        //                break;

                        //            case BU160_Config.KTU1:
                        //                SetModbusProviderHelper(tag, ports, modbusIp3Port0, modbusIp3Port1);
                        //                break;

                        //            case BU160_Config.KTU2:
                        //                break;

                        //            default:
                        //                throw new ArgumentOutOfRangeException("config", config, null);
                        //        }
                        //    }

                        //    #endregion

                        //    else if ((simAdr.StartByte >= CLIMAT_MIN_RANGE && simAdr.StartByte <= CLIMAT_MAX_RANGE))
                        //    {
                        //        switch (config)
                        //        {
                        //            case BU160_Config.KTU1_KTU2:
                        //                SetModbusProviderHelper(tag, ports, modbusIp3Port0, modbusIp3Port1);
                        //                break;

                        //            case BU160_Config.KTU1:
                        //                SetModbusProviderHelper(tag, ports, modbusIp3Port0, modbusIp3Port1);
                        //                break;

                        //            case BU160_Config.KTU2:
                        //                break;

                        //            default:
                        //                throw new ArgumentOutOfRangeException("config", config, null);
                        //        }
                        //    }
                        //    else if ((simAdr.StartByte >= TAR_MIN_RANGE && simAdr.StartByte <= TAR_MAX_RANGE))
                        //    {
                        //        switch (config)
                        //        {
                        //            case BU160_Config.KTU1_KTU2:
                        //                break;

                        //            case BU160_Config.KTU1:
                        //                SetModbusProviderHelper(tag, ports, modbusIp3Port0, modbusIp3Port1);
                        //                break;

                        //            case BU160_Config.KTU2:
                        //                break;

                        //            default:
                        //                throw new ArgumentOutOfRangeException("config", config, null);
                        //        }
                        //    }

                        //    #region Таймеры

                        //    else if (simAdr.StartByte >= TIMER_1_MIN_RANGE && simAdr.StartByte <= TIMER_1_MAX_RANGE)
                        //    {
                        //        switch (config)
                        //        {
                        //            case BU160_Config.KTU1_KTU2:
                        //                SetModbusProviderHelper(tag, ports, modbusIp3Port0, modbusIp3Port1);
                        //                break;

                        //            case BU160_Config.KTU1:
                        //                SetModbusProviderHelper(tag, ports, modbusIp3Port0, modbusIp3Port1);
                        //                break;

                        //            case BU160_Config.KTU2:
                        //                break;

                        //            default:
                        //                throw new ArgumentOutOfRangeException("config", config, null);
                        //        }
                        //    }
                        //    else if ((simAdr.StartByte >= TIMER_2_MIN_RANGE && simAdr.StartByte <= TIMER_2_MAX_RANGE))
                        //    {
                        //        switch (config)
                        //        {
                        //            case BU160_Config.KTU1_KTU2:
                        //                break;

                        //            case BU160_Config.KTU1:
                        //                SetModbusProviderHelper(tag, ports, modbusIp3Port0, modbusIp3Port1);
                        //                break;

                        //            case BU160_Config.KTU2:
                        //                break;

                        //            default:
                        //                throw new ArgumentOutOfRangeException("config", config, null);
                        //        }
                        //    }

                        //    #endregion

                        //    #region IO

                        //    else if ((simAdr.StartByte >= IO_1_MIN_RANGE && simAdr.StartByte <= IO_1_MAX_RANGE))
                        //    {
                        //        switch (config)
                        //        {
                        //            case BU160_Config.KTU1_KTU2:
                        //                SetModbusProviderHelper(tag, ports, modbusIp3Port0, modbusIp3Port1);
                        //                break;

                        //            case BU160_Config.KTU1:
                        //                SetModbusProviderHelper(tag, ports, modbusIp3Port0, modbusIp3Port1);
                        //                break;

                        //            case BU160_Config.KTU2:
                        //                break;

                        //            default:
                        //                throw new ArgumentOutOfRangeException("config", config, null);
                        //        }
                        //    }
                        //    else if ((simAdr.StartByte >= IO_2_MIN_RANGE && simAdr.StartByte <= IO_2_MAX_RANGE))
                        //    {
                        //        switch (config)
                        //        {
                        //            case BU160_Config.KTU1_KTU2:
                        //                break;

                        //            case BU160_Config.KTU1:
                        //                SetModbusProviderHelper(tag, ports, modbusIp3Port0, modbusIp3Port1);
                        //                break;

                        //            case BU160_Config.KTU2:
                        //                break;

                        //            default:
                        //                throw new ArgumentOutOfRangeException("config", config, null);
                        //        }
                        //    }
                        //    else if ((simAdr.StartByte >= IO_3_MIN_RANGE && simAdr.StartByte <= IO_3_MAX_RANGE))
                        //    {
                        //        switch (config)
                        //        {
                        //            case BU160_Config.KTU1_KTU2:
                        //                SetModbusProviderHelper(tag, ports, modbusIp3Port0, modbusIp3Port1);
                        //                break;

                        //            case BU160_Config.KTU1:
                        //                SetModbusProviderHelper(tag, ports, modbusIp3Port0, modbusIp3Port1);
                        //                break;

                        //            case BU160_Config.KTU2:
                        //                break;

                        //            default:
                        //                throw new ArgumentOutOfRangeException("config", config, null);
                        //        }
                        //    }
                        //    else if ((simAdr.StartByte >= IO_4_MIN_RANGE && simAdr.StartByte <= IO_4_MAX_RANGE))
                        //    {
                        //        switch (config)
                        //        {
                        //            case BU160_Config.KTU1_KTU2:
                        //                break;

                        //            case BU160_Config.KTU1:
                        //                SetModbusProviderHelper(tag, ports, modbusIp3Port0, modbusIp3Port1);
                        //                break;

                        //            case BU160_Config.KTU2:
                        //                break;

                        //            default:
                        //                throw new ArgumentOutOfRangeException("config", config, null);
                        //        }
                        //    }
                        //    else if ((simAdr.StartByte >= IO_5_MIN_RANGE && simAdr.StartByte <= IO_5_MAX_RANGE))
                        //    {
                        //        switch (config)
                        //        {
                        //            case BU160_Config.KTU1_KTU2:
                        //                SetModbusProviderHelper(tag, ports, modbusIp3Port0, modbusIp3Port1);
                        //                break;

                        //            case BU160_Config.KTU1:
                        //                SetModbusProviderHelper(tag, ports, modbusIp3Port0, modbusIp3Port1);
                        //                break;

                        //            case BU160_Config.KTU2:
                        //                break;

                        //            default:
                        //                throw new ArgumentOutOfRangeException("config", config, null);
                        //        }
                        //    }
                        //    else if ((simAdr.StartByte >= IO_VAL_1_MIN_RANGE && simAdr.StartByte <= IO_VAL_1_MAX_RANGE))
                        //    {
                        //        switch (config)
                        //        {
                        //            case BU160_Config.KTU1_KTU2:
                        //                SetModbusProviderHelper(tag, ports, modbusIp3Port0, modbusIp3Port1);
                        //                break;

                        //            case BU160_Config.KTU1:
                        //                SetModbusProviderHelper(tag, ports, modbusIp3Port0, modbusIp3Port1);
                        //                break;

                        //            case BU160_Config.KTU2:
                        //                break;

                        //            default:
                        //                throw new ArgumentOutOfRangeException("config", config, null);
                        //        }
                        //    }
                        //    else if ((simAdr.StartByte >= IO_VAL_2_MIN_RANGE && simAdr.StartByte <= IO_VAL_2_MAX_RANGE))
                        //    {
                        //        switch (config)
                        //        {
                        //            case BU160_Config.KTU1_KTU2:
                        //                break;

                        //            case BU160_Config.KTU1:
                        //                SetModbusProviderHelper(tag, ports, modbusIp3Port0, modbusIp3Port1);
                        //                break;

                        //            case BU160_Config.KTU2:
                        //                break;

                        //            default:
                        //                throw new ArgumentOutOfRangeException("config", config, null);
                        //        }
                        //    }
                        //    else if ((simAdr.StartByte >= IO_VAL_3_MIN_RANGE && simAdr.StartByte <= IO_VAL_3_MAX_RANGE))
                        //    {
                        //        switch (config)
                        //        {
                        //            case BU160_Config.KTU1_KTU2:
                        //                SetModbusProviderHelper(tag, ports, modbusIp3Port0, modbusIp3Port1);
                        //                break;

                        //            case BU160_Config.KTU1:
                        //                SetModbusProviderHelper(tag, ports, modbusIp3Port0, modbusIp3Port1);
                        //                break;

                        //            case BU160_Config.KTU2:
                        //                break;

                        //            default:
                        //                throw new ArgumentOutOfRangeException("config", config, null);
                        //        }
                        //    }
                        //    else if ((simAdr.StartByte >= IO_VAL_4_MIN_RANGE && simAdr.StartByte <= IO_VAL_4_MAX_RANGE))
                        //    {
                        //        switch (config)
                        //        {
                        //            case BU160_Config.KTU1_KTU2:
                        //                SetModbusProviderHelper(tag, ports, modbusIp3Port0, modbusIp3Port1);
                        //                break;

                        //            case BU160_Config.KTU1:
                        //                SetModbusProviderHelper(tag, ports, modbusIp3Port0, modbusIp3Port1);
                        //                break;

                        //            case BU160_Config.KTU2:
                        //                break;

                        //            default:
                        //                throw new ArgumentOutOfRangeException("config", config, null);
                        //        }
                        //    }
                        //    else if ((simAdr.StartByte >= IO_VAL_5_MIN_RANGE && simAdr.StartByte <= IO_VAL_5_MAX_RANGE))
                        //    {
                        //        switch (config)
                        //        {
                        //            case BU160_Config.KTU1_KTU2:
                        //                break;

                        //            case BU160_Config.KTU1:
                        //                SetModbusProviderHelper(tag, ports, modbusIp3Port0, modbusIp3Port1);
                        //                break;

                        //            case BU160_Config.KTU2:
                        //                break;

                        //            default:
                        //                throw new ArgumentOutOfRangeException("config", config, null);
                        //        }
                        //    }
                        //    else if ((simAdr.StartByte >= IO_VAL_6_MIN_RANGE && simAdr.StartByte <= IO_VAL_6_MAX_RANGE))
                        //    {
                        //        switch (config)
                        //        {
                        //            case BU160_Config.KTU1_KTU2:
                        //                SetModbusProviderHelper(tag, ports, modbusIp3Port0, modbusIp3Port1);
                        //                break;

                        //            case BU160_Config.KTU1:
                        //                SetModbusProviderHelper(tag, ports, modbusIp3Port0, modbusIp3Port1);
                        //                break;

                        //            case BU160_Config.KTU2:
                        //                break;

                        //            default:
                        //                throw new ArgumentOutOfRangeException("config", config, null);
                        //        }
                        //    }
                        //    else if ((simAdr.StartByte >= SERVICE_MIN_RANGE && simAdr.StartByte <= SERVICE_MAX_RANGE))
                        //    {
                        //        switch (config)
                        //        {
                        //            case BU160_Config.KTU1_KTU2:
                        //                SetModbusProviderHelper(tag, ports, modbusIp3Port0, modbusIp3Port1);
                        //                break;

                        //            case BU160_Config.KTU1:
                        //                SetModbusProviderHelper(tag, ports, modbusIp3Port0, modbusIp3Port1);
                        //                break;

                        //            case BU160_Config.KTU2:
                        //                break;

                        //            default:
                        //                throw new ArgumentOutOfRangeException("config", config, null);
                        //        }
                        //    }
                        //    else if ((simAdr.StartByte >= MSAB_MIN_RANGE && simAdr.StartByte <= MSAB_MAX_RANGE))
                        //    {
                        //        switch (config)
                        //        {
                        //            case BU160_Config.KTU1_KTU2:
                        //                break;

                        //            case BU160_Config.KTU1:
                        //                SetModbusProviderHelper(tag, ports, modbusIp3Port0, modbusIp3Port1);
                        //                break;

                        //            case BU160_Config.KTU2:
                        //                break;

                        //            default:
                        //                throw new ArgumentOutOfRangeException("config", config, null);
                        //        }
                        //    }
                        //    #endregion

                        //    else if (simAdr.StartByte >= TDS_MIN_RANGE && simAdr.StartByte <= TDS_MAX_RANGE)
                        //    {
                        //        switch (config)
                        //        {
                        //            case BU160_Config.KTU1_KTU2:
                        //                break;

                        //            case BU160_Config.KTU1:
                        //                SetModbusProviderHelper(tag, ports, modbusIp3Port0, modbusIp3Port1);
                        //                break;

                        //            case BU160_Config.KTU2:
                        //                break;

                        //            default:
                        //                throw new ArgumentOutOfRangeException("config", config, null);
                        //        }
                        //    }
                        //}

                        //#endregion


                        //#region DB351

                        //if (simAdr.DB == SiemensDbName.DB351)
                        //{
                        //    #region Кнопки ПБ

                        //    if (simAdr.StartByte == BTN_DC_0000_RANGE)
                        //    {
                        //        switch (config)
                        //        {
                        //            case BU160_Config.KTU1_KTU2:
                        //                break;

                        //            case BU160_Config.KTU1:
                        //                SetModbusProviderHelper(tag, ports, modbusIp3Port0, modbusIp3Port1);
                        //                break;

                        //            case BU160_Config.KTU2:
                        //                break;

                        //            default:
                        //                throw new ArgumentOutOfRangeException("config", config, null);
                        //        }
                        //    }
                        //    else if (simAdr.StartByte == BTN_DC_0001_RANGE && simAdr.Bit <= 5)
                        //    {
                        //        switch (config)
                        //        {
                        //            case BU160_Config.KTU1_KTU2:
                        //                SetModbusProviderHelper(tag, ports, modbusIp3Port0, modbusIp3Port1);
                        //                break;

                        //            case BU160_Config.KTU1:
                        //                SetModbusProviderHelper(tag, ports, modbusIp3Port0, modbusIp3Port1);
                        //                break;

                        //            case BU160_Config.KTU2:
                        //                break;

                        //            default:
                        //                throw new ArgumentOutOfRangeException("config", config, null);
                        //        }
                        //    }
                        //    else if (simAdr.StartByte == BTN_DC_0001_RANGE && simAdr.Bit >= 6)
                        //    {
                        //        switch (config)
                        //        {
                        //            case BU160_Config.KTU1_KTU2:
                        //                break;

                        //            case BU160_Config.KTU1:
                        //                SetModbusProviderHelper(tag, ports, modbusIp3Port0, modbusIp3Port1);
                        //                break;

                        //            case BU160_Config.KTU2:
                        //                break;

                        //            default:
                        //                throw new ArgumentOutOfRangeException("config", config, null);
                        //        }
                        //    }
                        //    else if (simAdr.StartByte == BTN_DC_0002_RANGE && simAdr.Bit <= 5)
                        //    {
                        //        switch (config)
                        //        {
                        //            case BU160_Config.KTU1_KTU2:
                        //                break;

                        //            case BU160_Config.KTU1:
                        //                SetModbusProviderHelper(tag, ports, modbusIp3Port0, modbusIp3Port1);
                        //                break;

                        //            case BU160_Config.KTU2:
                        //                break;

                        //            default:
                        //                throw new ArgumentOutOfRangeException("config", config, null);
                        //        }
                        //    }
                        //    else if (simAdr.StartByte == BTN_DC_0002_RANGE && simAdr.Bit >= 6)
                        //    {
                        //        switch (config)
                        //        {
                        //            case BU160_Config.KTU1_KTU2:
                        //                SetModbusProviderHelper(tag, ports, modbusIp3Port0, modbusIp3Port1);
                        //                break;

                        //            case BU160_Config.KTU1:
                        //                SetModbusProviderHelper(tag, ports, modbusIp3Port0, modbusIp3Port1);
                        //                break;

                        //            case BU160_Config.KTU2:
                        //                break;

                        //            default:
                        //                throw new ArgumentOutOfRangeException("config", config, null);
                        //        }
                        //    }
                        //    else if (simAdr.StartByte == BTN_DC_0003_RANGE && simAdr.Bit <= 2)
                        //    {
                        //        switch (config)
                        //        {
                        //            case BU160_Config.KTU1_KTU2:
                        //                break;

                        //            case BU160_Config.KTU1:
                        //                SetModbusProviderHelper(tag, ports, modbusIp3Port0, modbusIp3Port1);
                        //                break;

                        //            case BU160_Config.KTU2:
                        //                break;

                        //            default:
                        //                throw new ArgumentOutOfRangeException("config", config, null);
                        //        }
                        //    }
                        //    else if (simAdr.StartByte == BTN_DC_0003_RANGE && simAdr.Bit == 3)
                        //    {
                        //        switch (config)
                        //        {
                        //            case BU160_Config.KTU1_KTU2:
                        //                SetModbusProviderHelper(tag, ports, modbusIp3Port0, modbusIp3Port1);
                        //                break;

                        //            case BU160_Config.KTU1:
                        //                SetModbusProviderHelper(tag, ports, modbusIp3Port0, modbusIp3Port1);
                        //                break;

                        //            case BU160_Config.KTU2:
                        //                break;

                        //            default:
                        //                throw new ArgumentOutOfRangeException("config", config, null);
                        //        }
                        //    }
                        //    else if (simAdr.StartByte == BTN_DC_0003_RANGE && simAdr.Bit >= 4)
                        //    {
                        //        switch (config)
                        //        {
                        //            case BU160_Config.KTU1_KTU2:
                        //                break;

                        //            case BU160_Config.KTU1:
                        //                SetModbusProviderHelper(tag, ports, modbusIp3Port0, modbusIp3Port1);
                        //                break;

                        //            case BU160_Config.KTU2:
                        //                break;

                        //            default:
                        //                throw new ArgumentOutOfRangeException("config", config, null);
                        //        }
                        //    }
                        //    else if (simAdr.StartByte == BTN_DC_0004_RANGE && simAdr.Bit <= 4)
                        //    {
                        //        switch (config)
                        //        {
                        //            case BU160_Config.KTU1_KTU2:
                        //                break;

                        //            case BU160_Config.KTU1:
                        //                SetModbusProviderHelper(tag, ports, modbusIp3Port0, modbusIp3Port1);
                        //                break;

                        //            case BU160_Config.KTU2:
                        //                break;

                        //            default:
                        //                throw new ArgumentOutOfRangeException("config", config, null);
                        //        }
                        //    }
                        //    else if (simAdr.StartByte == BTN_DC_0004_RANGE && simAdr.Bit == 5)
                        //    {
                        //        switch (config)
                        //        {
                        //            case BU160_Config.KTU1_KTU2:
                        //                SetModbusProviderHelper(tag, ports, modbusIp3Port0, modbusIp3Port1);
                        //                break;

                        //            case BU160_Config.KTU1:
                        //                SetModbusProviderHelper(tag, ports, modbusIp3Port0, modbusIp3Port1);
                        //                break;

                        //            case BU160_Config.KTU2:
                        //                break;

                        //            default:
                        //                throw new ArgumentOutOfRangeException("config", config, null);
                        //        }
                        //    }
                        //    else if (simAdr.StartByte == BTN_DC_0004_RANGE && simAdr.Bit >= 6)
                        //    {
                        //        switch (config)
                        //        {
                        //            case BU160_Config.KTU1_KTU2:
                        //                SetModbusProviderHelper(tag, ports, modbusIp3Port0, modbusIp3Port1);
                        //                break;

                        //            case BU160_Config.KTU1:
                        //                SetModbusProviderHelper(tag, ports, modbusIp3Port0, modbusIp3Port1);
                        //                break;

                        //            case BU160_Config.KTU2:
                        //                break;

                        //            default:
                        //                throw new ArgumentOutOfRangeException("config", config, null);
                        //        }
                        //    }
                        //    else if (simAdr.StartByte >= BTN_DC_0005_RANGE && simAdr.StartByte <= BTN_DC_0010_RANGE)
                        //    {
                        //        switch (config)
                        //        {
                        //            case BU160_Config.KTU1_KTU2:
                        //                SetModbusProviderHelper(tag, ports, modbusIp3Port0, modbusIp3Port1);
                        //                break;

                        //            case BU160_Config.KTU1:
                        //                SetModbusProviderHelper(tag, ports, modbusIp3Port0, modbusIp3Port1);
                        //                break;

                        //            case BU160_Config.KTU2:
                        //                break;

                        //            default:
                        //                throw new ArgumentOutOfRangeException("config", config, null);
                        //        }
                        //    }
                        //    else if (simAdr.StartByte == BTN_DC_0011_RANGE && simAdr.Bit <= 3)
                        //    {
                        //        switch (config)
                        //        {
                        //            case BU160_Config.KTU1_KTU2:
                        //                SetModbusProviderHelper(tag, ports, modbusIp3Port0, modbusIp3Port1);
                        //                break;

                        //            case BU160_Config.KTU1:
                        //                SetModbusProviderHelper(tag, ports, modbusIp3Port0, modbusIp3Port1);
                        //                break;

                        //            case BU160_Config.KTU2:
                        //                break;

                        //            default:
                        //                throw new ArgumentOutOfRangeException("config", config, null);
                        //        }
                        //    }
                        //    else if (simAdr.StartByte == BTN_DC_0011_RANGE && simAdr.Bit >= 4)
                        //    {
                        //        switch (config)
                        //        {
                        //            case BU160_Config.KTU1_KTU2:
                        //                break;

                        //            case BU160_Config.KTU1:
                        //                SetModbusProviderHelper(tag, ports, modbusIp3Port0, modbusIp3Port1);
                        //                break;

                        //            case BU160_Config.KTU2:
                        //                break;

                        //            default:
                        //                throw new ArgumentOutOfRangeException("config", config, null);
                        //        }
                        //    }
                        //    else if (simAdr.StartByte == BTN_DC_0012_RANGE && simAdr.Bit <= 2)
                        //    {
                        //        switch (config)
                        //        {
                        //            case BU160_Config.KTU1_KTU2:
                        //                break;

                        //            case BU160_Config.KTU1:
                        //                SetModbusProviderHelper(tag, ports, modbusIp3Port0, modbusIp3Port1);
                        //                break;

                        //            case BU160_Config.KTU2:
                        //                break;

                        //            default:
                        //                throw new ArgumentOutOfRangeException("config", config, null);
                        //        }
                        //    }
                        //    else if (simAdr.StartByte == BTN_DC_0012_RANGE && simAdr.Bit == 3)
                        //    {
                        //        switch (config)
                        //        {
                        //            case BU160_Config.KTU1_KTU2:
                        //                SetModbusProviderHelper(tag, ports, modbusIp3Port0, modbusIp3Port1);
                        //                break;

                        //            case BU160_Config.KTU1:
                        //                SetModbusProviderHelper(tag, ports, modbusIp3Port0, modbusIp3Port1);
                        //                break;

                        //            case BU160_Config.KTU2:
                        //                break;

                        //            default:
                        //                throw new ArgumentOutOfRangeException("config", config, null);
                        //        }
                        //    }
                        //    else if (simAdr.StartByte == BTN_DC_0012_RANGE && simAdr.Bit >= 4)
                        //    {
                        //        switch (config)
                        //        {
                        //            case BU160_Config.KTU1_KTU2:
                        //                break;

                        //            case BU160_Config.KTU1:
                        //                SetModbusProviderHelper(tag, ports, modbusIp3Port0, modbusIp3Port1);
                        //                break;

                        //            case BU160_Config.KTU2:
                        //                break;

                        //            default:
                        //                throw new ArgumentOutOfRangeException("config", config, null);
                        //        }
                        //    }
                        //    else if (simAdr.StartByte == BTN_DC_0013_RANGE)
                        //    {
                        //        switch (config)
                        //        {
                        //            case BU160_Config.KTU1_KTU2:
                        //                break;

                        //            case BU160_Config.KTU1:
                        //                SetModbusProviderHelper(tag, ports, modbusIp3Port0, modbusIp3Port1);
                        //                break;

                        //            case BU160_Config.KTU2:
                        //                break;

                        //            default:
                        //                throw new ArgumentOutOfRangeException("config", config, null);
                        //        }
                        //    }

                        //    #endregion

                        //    #region Байпасы

                        //    else if (simAdr.StartByte >= BTN_BP_2000_RANGE && simAdr.StartByte <= BTN_BP_2001_RANGE)
                        //    {
                        //        switch (config)
                        //        {
                        //            case BU160_Config.KTU1_KTU2:
                        //                SetModbusProviderHelper(tag, ports, modbusIp3Port0, modbusIp3Port1);
                        //                break;

                        //            case BU160_Config.KTU1:
                        //                SetModbusProviderHelper(tag, ports, modbusIp3Port0, modbusIp3Port1);
                        //                break;

                        //            case BU160_Config.KTU2:
                        //                break;

                        //            default:
                        //                throw new ArgumentOutOfRangeException("config", config, null);
                        //        }
                        //    }
                        //    else if (simAdr.StartByte >= BTN_BP_2002_RANGE && simAdr.StartByte <= BTN_BP_2003_RANGE)
                        //    {
                        //        switch (config)
                        //        {
                        //            case BU160_Config.KTU1_KTU2:
                        //                break;

                        //            case BU160_Config.KTU1:
                        //                SetModbusProviderHelper(tag, ports, modbusIp3Port0, modbusIp3Port1);
                        //                break;

                        //            case BU160_Config.KTU2:
                        //                break;

                        //            default:
                        //                throw new ArgumentOutOfRangeException("config", config, null);
                        //        }
                        //    }
                        //    else if (simAdr.StartByte == BTN_BP_2004_RANGE && simAdr.Bit <= 2)
                        //    {
                        //        switch (config)
                        //        {
                        //            case BU160_Config.KTU1_KTU2:
                        //                break;

                        //            case BU160_Config.KTU1:
                        //                SetModbusProviderHelper(tag, ports, modbusIp3Port0, modbusIp3Port1);
                        //                break;

                        //            case BU160_Config.KTU2:
                        //                break;

                        //            default:
                        //                throw new ArgumentOutOfRangeException("config", config, null);
                        //        }
                        //    }
                        //    else if (simAdr.StartByte == BTN_BP_2004_RANGE && simAdr.Bit >= 3)
                        //    {
                        //        switch (config)
                        //        {
                        //            case BU160_Config.KTU1_KTU2:
                        //                SetModbusProviderHelper(tag, ports, modbusIp3Port0, modbusIp3Port1);
                        //                break;

                        //            case BU160_Config.KTU1:
                        //                SetModbusProviderHelper(tag, ports, modbusIp3Port0, modbusIp3Port1);
                        //                break;

                        //            case BU160_Config.KTU2:
                        //                break;

                        //            default:
                        //                throw new ArgumentOutOfRangeException("config", config, null);
                        //        }
                        //    }
                        //    else if (simAdr.StartByte == BTN_BP_2005_RANGE)
                        //    {
                        //        switch (config)
                        //        {
                        //            case BU160_Config.KTU1_KTU2:
                        //                break;

                        //            case BU160_Config.KTU1:
                        //                SetModbusProviderHelper(tag, ports, modbusIp3Port0, modbusIp3Port1);
                        //                break;

                        //            case BU160_Config.KTU2:
                        //                break;

                        //            default:
                        //                throw new ArgumentOutOfRangeException("config", config, null);
                        //        }
                        //    }
                        //    else if (simAdr.StartByte == BTN_BP_2006_RANGE && simAdr.Bit <= 2)
                        //    {
                        //        switch (config)
                        //        {
                        //            case BU160_Config.KTU1_KTU2:
                        //                SetModbusProviderHelper(tag, ports, modbusIp3Port0, modbusIp3Port1);
                        //                break;

                        //            case BU160_Config.KTU1:
                        //                SetModbusProviderHelper(tag, ports, modbusIp3Port0, modbusIp3Port1);
                        //                break;

                        //            case BU160_Config.KTU2:
                        //                break;

                        //            default:
                        //                throw new ArgumentOutOfRangeException("config", config, null);
                        //        }
                        //    }
                        //    else if (simAdr.StartByte == BTN_BP_2006_RANGE && simAdr.Bit == 2)
                        //    {
                        //        switch (config)
                        //        {
                        //            case BU160_Config.KTU1_KTU2:
                        //                break;

                        //            case BU160_Config.KTU1:
                        //                SetModbusProviderHelper(tag, ports, modbusIp3Port0, modbusIp3Port1);
                        //                break;

                        //            case BU160_Config.KTU2:
                        //                break;

                        //            default:
                        //                throw new ArgumentOutOfRangeException("config", config, null);
                        //        }
                        //    }
                        //    else if (simAdr.StartByte == BTN_BP_2006_RANGE && simAdr.Bit >= 4)
                        //    {
                        //        switch (config)
                        //        {
                        //            case BU160_Config.KTU1_KTU2:
                        //                SetModbusProviderHelper(tag, ports, modbusIp3Port0, modbusIp3Port1);
                        //                break;

                        //            case BU160_Config.KTU1:
                        //                SetModbusProviderHelper(tag, ports, modbusIp3Port0, modbusIp3Port1);
                        //                break;

                        //            case BU160_Config.KTU2:
                        //                break;

                        //            default:
                        //                throw new ArgumentOutOfRangeException("config", config, null);
                        //        }
                        //    }
                        //    else if (simAdr.StartByte == BTN_BP_2007_RANGE && simAdr.Bit == 0)
                        //    {
                        //        switch (config)
                        //        {
                        //            case BU160_Config.KTU1_KTU2:
                        //                SetModbusProviderHelper(tag, ports, modbusIp3Port0, modbusIp3Port1);
                        //                break;

                        //            case BU160_Config.KTU1:
                        //                SetModbusProviderHelper(tag, ports, modbusIp3Port0, modbusIp3Port1);
                        //                break;

                        //            case BU160_Config.KTU2:
                        //                break;

                        //            default:
                        //                throw new ArgumentOutOfRangeException("config", config, null);
                        //        }
                        //    }
                        //    else if (simAdr.StartByte == BTN_BP_2007_RANGE && simAdr.Bit <= 2)
                        //    {
                        //        switch (config)
                        //        {
                        //            case BU160_Config.KTU1_KTU2:
                        //                break;

                        //            case BU160_Config.KTU1:
                        //                SetModbusProviderHelper(tag, ports, modbusIp3Port0, modbusIp3Port1);
                        //                break;

                        //            case BU160_Config.KTU2:
                        //                break;

                        //            default:
                        //                throw new ArgumentOutOfRangeException("config", config, null);
                        //        }
                        //    }
                        //    else if (simAdr.StartByte == BTN_BP_2007_RANGE && simAdr.Bit >= 3 && simAdr.Bit <= 6)
                        //    {
                        //        switch (config)
                        //        {
                        //            case BU160_Config.KTU1_KTU2:
                        //                SetModbusProviderHelper(tag, ports, modbusIp3Port0, modbusIp3Port1);
                        //                break;

                        //            case BU160_Config.KTU1:
                        //                SetModbusProviderHelper(tag, ports, modbusIp3Port0, modbusIp3Port1);
                        //                break;

                        //            case BU160_Config.KTU2:
                        //                break;

                        //            default:
                        //                throw new ArgumentOutOfRangeException("config", config, null);
                        //        }
                        //    }
                        //    else if (simAdr.StartByte == BTN_BP_2007_RANGE && simAdr.Bit == 7)
                        //    {
                        //        switch (config)
                        //        {
                        //            case BU160_Config.KTU1_KTU2:
                        //                break;

                        //            case BU160_Config.KTU1:
                        //                SetModbusProviderHelper(tag, ports, modbusIp3Port0, modbusIp3Port1);
                        //                break;

                        //            case BU160_Config.KTU2:
                        //                break;

                        //            default:
                        //                throw new ArgumentOutOfRangeException("config", config, null);
                        //        }
                        //    }
                        //    else if (simAdr.StartByte == BTN_BP_2008_RANGE && simAdr.Bit <= 1)
                        //    {
                        //        switch (config)
                        //        {
                        //            case BU160_Config.KTU1_KTU2:
                        //                break;

                        //            case BU160_Config.KTU1:
                        //                SetModbusProviderHelper(tag, ports, modbusIp3Port0, modbusIp3Port1);
                        //                break;

                        //            case BU160_Config.KTU2:
                        //                break;

                        //            default:
                        //                throw new ArgumentOutOfRangeException("config", config, null);
                        //        }
                        //    }
                        //    else if (simAdr.StartByte == BTN_BP_2008_RANGE && simAdr.Bit >= 2 && simAdr.Bit <= 5)
                        //    {
                        //        switch (config)
                        //        {
                        //            case BU160_Config.KTU1_KTU2:
                        //                SetModbusProviderHelper(tag, ports, modbusIp3Port0, modbusIp3Port1);
                        //                break;

                        //            case BU160_Config.KTU1:
                        //                SetModbusProviderHelper(tag, ports, modbusIp3Port0, modbusIp3Port1);
                        //                break;

                        //            case BU160_Config.KTU2:
                        //                break;

                        //            default:
                        //                throw new ArgumentOutOfRangeException("config", config, null);
                        //        }
                        //    }
                        //    else if (simAdr.StartByte == BTN_BP_2008_RANGE && simAdr.Bit >= 6 && simAdr.Bit <= 7)
                        //    {
                        //        switch (config)
                        //        {
                        //            case BU160_Config.KTU1_KTU2:
                        //                break;

                        //            case BU160_Config.KTU1:
                        //                SetModbusProviderHelper(tag, ports, modbusIp3Port0, modbusIp3Port1);
                        //                break;

                        //            case BU160_Config.KTU2:
                        //                break;

                        //            default:
                        //                throw new ArgumentOutOfRangeException("config", config, null);
                        //        }
                        //    }
                        //    else if (simAdr.StartByte == BTN_BP_2009_RANGE)
                        //    {
                        //        switch (config)
                        //        {
                        //            case BU160_Config.KTU1_KTU2:
                        //                break;

                        //            case BU160_Config.KTU1:
                        //                SetModbusProviderHelper(tag, ports, modbusIp3Port0, modbusIp3Port1);
                        //                break;

                        //            case BU160_Config.KTU2:
                        //                break;

                        //            default:
                        //                throw new ArgumentOutOfRangeException("config", config, null);
                        //        }
                        //    }

                        //    #endregion

                        //    else if (simAdr.StartByte >= BTN_CLIMAT_MIN_RANGE && simAdr.StartByte <= BTN_CLIMAT_MAX_RANGE)
                        //    {
                        //        switch (config)
                        //        {
                        //            case BU160_Config.KTU1_KTU2:
                        //                SetModbusProviderHelper(tag, ports, modbusIp3Port0, modbusIp3Port1);
                        //                break;

                        //            case BU160_Config.KTU1:
                        //                SetModbusProviderHelper(tag, ports, modbusIp3Port0, modbusIp3Port1);
                        //                break;

                        //            case BU160_Config.KTU2:
                        //                break;

                        //            default:
                        //                throw new ArgumentOutOfRangeException("config", config, null);
                        //        }
                        //    }
                        //    else if (simAdr.StartByte >= BTN_SETUP_1_MIN_RANGE && simAdr.StartByte <= BTN_SETUP_1_MAX_RANGE)
                        //    {
                        //        switch (config)
                        //        {
                        //            case BU160_Config.KTU1_KTU2:
                        //                break;

                        //            case BU160_Config.KTU1:
                        //                SetModbusProviderHelper(tag, ports, modbusIp3Port0, modbusIp3Port1);
                        //                break;

                        //            case BU160_Config.KTU2:
                        //                break;

                        //            default:
                        //                throw new ArgumentOutOfRangeException("config", config, null);
                        //        }
                        //    }
                        //    else if (simAdr.StartByte >= BTN_SETUP_2_MIN_RANGE && simAdr.StartByte <= BTN_SETUP_2_MAX_RANGE)
                        //    {
                        //        switch (config)
                        //        {
                        //            case BU160_Config.KTU1_KTU2:
                        //                SetModbusProviderHelper(tag, ports, modbusIp3Port0, modbusIp3Port1);
                        //                break;

                        //            case BU160_Config.KTU1:
                        //                SetModbusProviderHelper(tag, ports, modbusIp3Port0, modbusIp3Port1);
                        //                break;

                        //            case BU160_Config.KTU2:
                        //                break;

                        //            default:
                        //                throw new ArgumentOutOfRangeException("config", config, null);
                        //        }
                        //    }
                        //    else if (simAdr.StartByte >= BTN_SETUP_3_MIN_RANGE && simAdr.StartByte <= BTN_SETUP_3_MAX_RANGE)
                        //    {
                        //        switch (config)
                        //        {
                        //            case BU160_Config.KTU1_KTU2:
                        //                break;

                        //            case BU160_Config.KTU1:
                        //                SetModbusProviderHelper(tag, ports, modbusIp3Port0, modbusIp3Port1);
                        //                break;

                        //            case BU160_Config.KTU2:
                        //                break;

                        //            default:
                        //                throw new ArgumentOutOfRangeException("config", config, null);
                        //        }
                        //    }
                        //    else if (simAdr.StartByte >= BTN_TAR_MIN_RANGE && simAdr.StartByte <= BTN_TAR_MAX_RANGE)
                        //    {
                        //        switch (config)
                        //        {
                        //            case BU160_Config.KTU1_KTU2:
                        //                break;

                        //            case BU160_Config.KTU1:
                        //                SetModbusProviderHelper(tag, ports, modbusIp3Port0, modbusIp3Port1);
                        //                break;

                        //            case BU160_Config.KTU2:
                        //                break;

                        //            default:
                        //                throw new ArgumentOutOfRangeException("config", config, null);
                        //        }
                        //    }
                        //    else if (simAdr.StartByte >= BTN_TIME_1_MIN_RANGE && simAdr.StartByte <= BTN_TIME_1_MAX_RANGE)
                        //    {
                        //        switch (config)
                        //        {
                        //            case BU160_Config.KTU1_KTU2:
                        //                SetModbusProviderHelper(tag, ports, modbusIp3Port0, modbusIp3Port1);
                        //                break;

                        //            case BU160_Config.KTU1:
                        //                SetModbusProviderHelper(tag, ports, modbusIp3Port0, modbusIp3Port1);
                        //                break;

                        //            case BU160_Config.KTU2:
                        //                break;

                        //            default:
                        //                throw new ArgumentOutOfRangeException("config", config, null);
                        //        }
                        //    }
                        //    else if (simAdr.StartByte >= BTN_TIME_2_MIN_RANGE && simAdr.StartByte <= BTN_TIME_2_MAX_RANGE)
                        //    {
                        //        switch (config)
                        //        {
                        //            case BU160_Config.KTU1_KTU2:
                        //                break;

                        //            case BU160_Config.KTU1:
                        //                SetModbusProviderHelper(tag, ports, modbusIp3Port0, modbusIp3Port1);
                        //                break;

                        //            case BU160_Config.KTU2:
                        //                break;

                        //            default:
                        //                throw new ArgumentOutOfRangeException("config", config, null);
                        //        }
                        //    }

                        //    #region Датчики

                        //    else if (simAdr.StartByte >= BTN_SENSOR_1_MIN_RANGE && simAdr.StartByte <= BTN_SENSOR_1_MAX_RANGE)
                        //    {
                        //        switch (config)
                        //        {
                        //            case BU160_Config.KTU1_KTU2:
                        //                SetModbusProviderHelper(tag, ports, modbusIp3Port0, modbusIp3Port1);
                        //                break;

                        //            case BU160_Config.KTU1:
                        //                SetModbusProviderHelper(tag, ports, modbusIp3Port0, modbusIp3Port1);
                        //                break;

                        //            case BU160_Config.KTU2:
                        //                break;

                        //            default:
                        //                throw new ArgumentOutOfRangeException("config", config, null);
                        //        }
                        //    }
                        //    else if (simAdr.StartByte >= BTN_SENSOR_2_MIN_RANGE && simAdr.StartByte <= BTN_SENSOR_2_MAX_RANGE)
                        //    {
                        //        switch (config)
                        //        {
                        //            case BU160_Config.KTU1_KTU2:
                        //                break;

                        //            case BU160_Config.KTU1:
                        //                SetModbusProviderHelper(tag, ports, modbusIp3Port0, modbusIp3Port1);
                        //                break;

                        //            case BU160_Config.KTU2:
                        //                break;

                        //            default:
                        //                throw new ArgumentOutOfRangeException("config", config, null);
                        //        }
                        //    }
                        //    else if (simAdr.StartByte >= BTN_SENSOR_3_MIN_RANGE && simAdr.StartByte <= BTN_SENSOR_3_MAX_RANGE)
                        //    {
                        //        switch (config)
                        //        {
                        //            case BU160_Config.KTU1_KTU2:
                        //                SetModbusProviderHelper(tag, ports, modbusIp3Port0, modbusIp3Port1);
                        //                break;

                        //            case BU160_Config.KTU1:
                        //                SetModbusProviderHelper(tag, ports, modbusIp3Port0, modbusIp3Port1);
                        //                break;

                        //            case BU160_Config.KTU2:
                        //                break;

                        //            default:
                        //                throw new ArgumentOutOfRangeException("config", config, null);
                        //        }
                        //    }
                        //    else if (simAdr.StartByte >= BTN_SENSOR_4_MIN_RANGE && simAdr.StartByte <= BTN_SENSOR_4_MAX_RANGE)
                        //    {
                        //        switch (config)
                        //        {
                        //            case BU160_Config.KTU1_KTU2:
                        //                break;

                        //            case BU160_Config.KTU1:
                        //                SetModbusProviderHelper(tag, ports, modbusIp3Port0, modbusIp3Port1);
                        //                break;

                        //            case BU160_Config.KTU2:
                        //                break;

                        //            default:
                        //                throw new ArgumentOutOfRangeException("config", config, null);
                        //        }
                        //    }
                        //    else if (simAdr.StartByte >= BTN_SENSOR_5_MIN_RANGE && simAdr.StartByte <= BTN_SENSOR_5_MAX_RANGE)
                        //    {
                        //        switch (config)
                        //        {
                        //            case BU160_Config.KTU1_KTU2:
                        //                SetModbusProviderHelper(tag, ports, modbusIp3Port0, modbusIp3Port1);
                        //                break;

                        //            case BU160_Config.KTU1:
                        //                SetModbusProviderHelper(tag, ports, modbusIp3Port0, modbusIp3Port1);
                        //                break;

                        //            case BU160_Config.KTU2:
                        //                break;

                        //            default:
                        //                throw new ArgumentOutOfRangeException("config", config, null);
                        //        }
                        //    }
                        //    else if (simAdr.StartByte >= BTN_SENSOR_6_MIN_RANGE && simAdr.StartByte <= BTN_SENSOR_6_MAX_RANGE)
                        //    {
                        //        switch (config)
                        //        {
                        //            case BU160_Config.KTU1_KTU2:
                        //                break;

                        //            case BU160_Config.KTU1:
                        //                SetModbusProviderHelper(tag, ports, modbusIp3Port0, modbusIp3Port1);
                        //                break;

                        //            case BU160_Config.KTU2:
                        //                break;

                        //            default:
                        //                throw new ArgumentOutOfRangeException("config", config, null);
                        //        }
                        //    }
                        //    else if (simAdr.StartByte >= BTN_SENSOR_7_MIN_RANGE && simAdr.StartByte <= BTN_SENSOR_7_MAX_RANGE)
                        //    {
                        //        switch (config)
                        //        {
                        //            case BU160_Config.KTU1_KTU2:
                        //                SetModbusProviderHelper(tag, ports, modbusIp3Port0, modbusIp3Port1);
                        //                break;

                        //            case BU160_Config.KTU1:
                        //                SetModbusProviderHelper(tag, ports, modbusIp3Port0, modbusIp3Port1);
                        //                break;

                        //            case BU160_Config.KTU2:
                        //                break;

                        //            default:
                        //                throw new ArgumentOutOfRangeException("config", config, null);
                        //        }
                        //    }
                        //    else if (simAdr.StartByte >= BTN_SENSOR_8_MIN_RANGE && simAdr.StartByte <= BTN_SENSOR_8_MAX_RANGE)
                        //    {
                        //        switch (config)
                        //        {
                        //            case BU160_Config.KTU1_KTU2:
                        //                break;

                        //            case BU160_Config.KTU1:
                        //                SetModbusProviderHelper(tag, ports, modbusIp3Port0, modbusIp3Port1);
                        //                break;

                        //            case BU160_Config.KTU2:
                        //                break;

                        //            default:
                        //                throw new ArgumentOutOfRangeException("config", config, null);
                        //        }
                        //    }

                        //    #endregion

                        //    else if (simAdr.StartByte >= BTN_SERVICE_MIN_RANGE && simAdr.StartByte <= BTN_SERVICE_MAX_RANGE)
                        //    {
                        //        switch (config)
                        //        {
                        //            case BU160_Config.KTU1_KTU2:
                        //                SetModbusProviderHelper(tag, ports, modbusIp3Port0, modbusIp3Port1);
                        //                break;

                        //            case BU160_Config.KTU1:
                        //                SetModbusProviderHelper(tag, ports, modbusIp3Port0, modbusIp3Port1);
                        //                break;

                        //            case BU160_Config.KTU2:
                        //                break;

                        //            default:
                        //                throw new ArgumentOutOfRangeException("config", config, null);
                        //        }
                        //    }
                        //    else if (simAdr.StartByte >= BTN_MSAB_MIN_RANGE && simAdr.StartByte <= BTN_MSAB_MAX_RANGE)
                        //    {
                        //        switch (config)
                        //        {
                        //            case BU160_Config.KTU1_KTU2:
                        //                break;

                        //            case BU160_Config.KTU1:
                        //                SetModbusProviderHelper(tag, ports, modbusIp3Port0, modbusIp3Port1);
                        //                break;

                        //            case BU160_Config.KTU2:
                        //                break;

                        //            default:
                        //                throw new ArgumentOutOfRangeException("config", config, null);
                        //        }
                        //    }
                        //}

                        //#endregion

                    }
                }
            });
        }
    }
}
