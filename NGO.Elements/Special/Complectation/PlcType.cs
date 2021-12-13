using Newtonsoft.Json;
using Newtonsoft.Json.Converters;

namespace NGO.Elements.Special.Complectation
{
    [JsonConverter(typeof(StringEnumConverter))]
    public enum PlcType : int
    {
        Default,
        Fastwel
    }
}
