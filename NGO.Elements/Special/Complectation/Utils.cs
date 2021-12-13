using Monokot.Hmi.Core.Framework.Storage;
using Monokot.Hmi.Core.Framework.Trees;
using Monokot.Hmi.Core.Tags;
using Monokot.Hmi.Core.Utils;
using Monokot.Hmi.StdProviders.Modbus;
using Newtonsoft.Json;
using System;
using System.IO;
using System.Reflection;

namespace NGO.Elements.Special.Complectation
{
    public static class Utils
    {
        public static void SetFastwelTags(HmiDataFile file)
        {
            var provider = file.DataProvidersModule.DataProvidersRoot.Items["ModbusDriver"] as ModbusDataProvider;
            provider.Port = 502;
            provider.FirstWordLowIn32Bit = true;
            provider.HoldingRegistersCount = 64;
            provider.InternalRegistersCount = 64;

            provider.Host = "192.168.1.18";

            var provider2 = file.DataProvidersModule.DataProvidersRoot.Items["modbus_506"] as ModbusDataProvider;
            provider2.Host = provider.Host;
            provider2.Port = 502;
            provider2.FirstWordLowIn32Bit = true;



            HmiUtils.RecursiveNodeAction<TagsHmiNode, string, HmiTag>(file.TagsRoot, n =>
            {
                foreach (var tag in n.Items)
                {
                    // tegi svazannie s otobrazeniem daty
                    if (!(tag.DataProvider is ModbusDataProvider))
                        continue;

                    tag.DataProvider = provider;

                    var address = tag.DataAddress as ModbusDataAddress;

                    if (address != null)
                    {
                        if (address.AddressTableInfo.AddressTable == Monokot.Hmi.StdProviders.Modbus.Utils.ModbusAddressTableInfo.ModbusAddressTable.HoldingRegisters)
                        {
                            if (0 < address.StartAddress && address.StartAddress < 7)
                            {
                                address.StartAddress = address.StartAddress;
                            }

                            if (1000 < address.StartAddress && address.StartAddress < 1008)
                            {
                                address.StartAddress = address.StartAddress - 993;
                            }

                            if (2000 < address.StartAddress && address.StartAddress < 2003)
                            {
                                address.StartAddress = address.StartAddress - 1988;
                            }

                            if (3000 < address.StartAddress && address.StartAddress < 3032)
                            {
                                address.StartAddress = address.StartAddress - 2986;
                            }

                            if (4000 < address.StartAddress && address.StartAddress < 4093)
                            {
                                address.StartAddress = address.StartAddress - 3955;
                            }

                            // real
                            if (4500 < address.StartAddress && address.StartAddress < 4538)
                            {
                                address.StartAddress = address.StartAddress - 4314;
                            }


                            if (4538 < address.StartAddress && address.StartAddress < 4552)
                            {
                                address.StartAddress = address.StartAddress - 4401;
                            }

                            // real
                            if (4551 < address.StartAddress && address.StartAddress < 4555)
                            {
                                address.StartAddress = address.StartAddress - 4327;
                            }


                            // calc
                            if (4559 < address.StartAddress && address.StartAddress < 4573)
                            {
                                address.StartAddress = address.StartAddress - 4322;
                            }

                            // real
                            if (5000 < address.StartAddress && address.StartAddress < 5492)
                            {
                                address.StartAddress = address.StartAddress - 4763;
                            }

                            if (6000 < address.StartAddress && address.StartAddress < 6013)
                            {
                                address.StartAddress = address.StartAddress - 5850;
                            }

                            if (address.StartAddress == 7000)
                            {
                                address.StartAddress = address.StartAddress - 6838;
                            }

                            // real
                            if (7500 < address.StartAddress && address.StartAddress < 7596)
                            {
                                address.StartAddress = address.StartAddress - 6758;
                            }


                            if (7752 < address.StartAddress && address.StartAddress < 7758)
                            {
                                address.StartAddress = address.StartAddress - 7589;
                            }

                            if (7752 < address.StartAddress && address.StartAddress < 7758)
                            {
                                address.StartAddress = address.StartAddress - 7589;
                            }

                            if (address.StartAddress == 7795)
                            {
                                address.StartAddress = address.StartAddress - 7626;
                            }

                            if (7782 < address.StartAddress && address.StartAddress < 7803)
                            {
                                address.StartAddress = address.StartAddress - 7613;
                            }

                            if (address.StartAddress == 10584)
                            {
                                address.StartAddress = address.StartAddress - 9765;
                            }


                        }

                        if (address.AddressTableInfo.AddressTable == Monokot.Hmi.StdProviders.Modbus.Utils.ModbusAddressTableInfo.ModbusAddressTable.InternalRegisters)
                        {
                            if (0 < address.StartAddress && address.StartAddress < 69)
                            {
                                //address.StartAddress = address.StartAddress;
                            }

                            else if (1000 < address.StartAddress && address.StartAddress < 1071)
                            {
                                address.StartAddress = address.StartAddress - 930;
                            }

                            else if (2000 < address.StartAddress && address.StartAddress < 2071)
                            {
                                address.StartAddress = address.StartAddress - 1860;
                            }

                            else if (3000 < address.StartAddress && address.StartAddress < 3132)
                            {
                                address.StartAddress = address.StartAddress - 2790;
                            }

                            else if (3145 < address.StartAddress && address.StartAddress < 3150)
                            {
                                address.StartAddress = address.StartAddress - 2785;
                            }

                            else if (4000 < address.StartAddress && address.StartAddress < 4054)
                            {
                                address.StartAddress = address.StartAddress - 3636;
                            }

                            else if (4060 < address.StartAddress && address.StartAddress < 4074)
                            {
                                address.StartAddress = address.StartAddress - 3643;
                            }

                            else if (5000 < address.StartAddress && address.StartAddress < 5051)
                            {
                                address.StartAddress = address.StartAddress - 4570;
                            }

                            else if (6000 < address.StartAddress && address.StartAddress < 6031)
                            {
                                address.StartAddress = address.StartAddress - 5520;
                            }

                            else if (6500 < address.StartAddress && address.StartAddress < 6511)
                            {
                                address.StartAddress = address.StartAddress - 5990;
                            }

                            else if (6520 < address.StartAddress && address.StartAddress < 6531)
                            {
                                address.StartAddress = address.StartAddress - 6000;
                            }

                            else if (7000 < address.StartAddress && address.StartAddress < 7098)
                            {
                                address.StartAddress = address.StartAddress - 6469;
                            }

                            else if (7100 < address.StartAddress && address.StartAddress < 7141)
                            {
                                address.StartAddress = address.StartAddress - 6470;
                            }

                            else if (7150 < address.StartAddress && address.StartAddress < 7191)
                            {
                                address.StartAddress = address.StartAddress - 6480;
                            }

                            else if (7200 < address.StartAddress && address.StartAddress < 7211)
                            {
                                address.StartAddress = address.StartAddress - 6484;
                            }

                            else if (7251 < address.StartAddress && address.StartAddress < 7266)
                            {
                                address.StartAddress = address.StartAddress - 6520;
                            }

                            else if (7303 < address.StartAddress && address.StartAddress < 7314)
                            {
                                address.StartAddress = address.StartAddress - 6557;
                            }

                            else if (7353 < address.StartAddress && address.StartAddress < 7363)
                            {
                                address.StartAddress = address.StartAddress - 6597;
                            }

                            else if (7500 < address.StartAddress && address.StartAddress < 7506)
                            {
                                address.StartAddress = address.StartAddress - 6734;
                            }

                            else if (7550 < address.StartAddress && address.StartAddress < 7556)
                            {
                                address.StartAddress = address.StartAddress - 6774;
                            }

                            else if (7575 < address.StartAddress && address.StartAddress < 7581)
                            {
                                address.StartAddress = address.StartAddress - 6794;
                            }

                            else if (7600 < address.StartAddress && address.StartAddress < 7606)
                            {
                                address.StartAddress = address.StartAddress - 6814;
                            }

                            else if (7625 < address.StartAddress && address.StartAddress < 7631)
                            {
                                address.StartAddress = address.StartAddress - 6834;
                            }

                            else if (7650 < address.StartAddress && address.StartAddress < 7656)
                            {
                                address.StartAddress = address.StartAddress - 6854;
                            }

                            else if (7675 < address.StartAddress && address.StartAddress < 7656)
                            {
                                address.StartAddress = address.StartAddress - 6874;
                            }

                            else if (7700 < address.StartAddress && address.StartAddress < 7706)
                            {
                                address.StartAddress = address.StartAddress - 6894;
                            }

                            else if (7725 < address.StartAddress && address.StartAddress < 7731)
                            {
                                address.StartAddress = address.StartAddress - 6914;
                            }

                            else if (7750 < address.StartAddress && address.StartAddress < 7756)
                            {
                                address.StartAddress = address.StartAddress - 6934;
                            }

                            else if (7775 < address.StartAddress && address.StartAddress < 7781)
                            {
                                address.StartAddress = address.StartAddress - 6954;
                            }

                            // real
                            else if (8000 < address.StartAddress && address.StartAddress < 8492)
                            {
                                address.StartAddress = address.StartAddress - 6873;
                            }

                            else if (8500 < address.StartAddress && address.StartAddress < 8520)
                            {
                                address.StartAddress = address.StartAddress - 7674;
                            }

                            else if (8520 < address.StartAddress && address.StartAddress < 8531)
                            {
                                address.StartAddress = address.StartAddress - 7664;
                            }

                            else if (9000 < address.StartAddress && address.StartAddress < 9011)
                            {
                                address.StartAddress = address.StartAddress - 8134;
                            }

                            else if (9014 < address.StartAddress && address.StartAddress < 9020)
                            {
                                address.StartAddress = address.StartAddress - 8138;
                            }

                            else if (9023 < address.StartAddress && address.StartAddress < 9034)
                            {
                                address.StartAddress = address.StartAddress - 8142;
                            }

                            else if (10000 < address.StartAddress && address.StartAddress < 10105)
                            {
                                address.StartAddress = address.StartAddress - 9109;
                            }

                            // real
                            else if (10500 < address.StartAddress && address.StartAddress < 10538)
                            {
                                address.StartAddress = address.StartAddress - 8881;
                            }

                            else if (10537 < address.StartAddress && address.StartAddress < 10552)
                            {
                                address.StartAddress = address.StartAddress - 9543;
                            }

                            // real
                            else if (10551 < address.StartAddress && address.StartAddress < 10555)
                            {
                                address.StartAddress = address.StartAddress - 8894;
                            }

                            // real
                            else if (10650 < address.StartAddress && address.StartAddress < 10678)
                            {
                                address.StartAddress = address.StartAddress - 8989;
                            }

                            else if (11000 < address.StartAddress && address.StartAddress < 11961)
                            {
                                address.StartAddress = address.StartAddress - 9992;
                            }

                            else if (11960 < address.StartAddress && address.StartAddress < 11970)
                            {
                                address.StartAddress = address.StartAddress - 10911;
                            }

                            // real
                            else if (11150 < address.StartAddress && address.StartAddress < 11238)
                            {
                                address.StartAddress = address.StartAddress - 9467;
                            }

                            // real
                            else if (11500 < address.StartAddress && address.StartAddress < 11614)
                            {
                                address.StartAddress = address.StartAddress - 9739;
                            }

                            // real
                            else if (12000 < address.StartAddress && address.StartAddress < 12200)
                            {
                                address.StartAddress = address.StartAddress - 10125;
                            }

                            // real
                            else if (12500 < address.StartAddress && address.StartAddress < 12596)
                            {
                                address.StartAddress = address.StartAddress - 10425;
                            }

                            else if (12752 < address.StartAddress && address.StartAddress < 12778)
                            {
                                address.StartAddress = address.StartAddress - 10591;
                            }

                            else if (12752 < address.StartAddress && address.StartAddress < 12763)
                            {
                                address.StartAddress = address.StartAddress - 11694;
                            }

                            else if (13000 < address.StartAddress && address.StartAddress < 13035)
                            {
                                address.StartAddress = address.StartAddress - 11932;
                            }

                            else if (64000 < address.StartAddress && address.StartAddress < 64200)
                            {
                                address.StartAddress = address.StartAddress - 64000;
                            }
                        }
                    }
                }
            });
        }

        public static PlcType loadPlcConfig(string path)
        {
            var uri = new Uri(Assembly.GetExecutingAssembly().GetName().CodeBase);
            var file = Path.GetDirectoryName(uri.LocalPath.ToLower()) + path;

            try
            {
                return JsonConvert.DeserializeObject<PlcType>(File.ReadAllText(file));
            }
            catch (Exception x)
            {
                return PlcType.Default;
            }
        }
    }
}
