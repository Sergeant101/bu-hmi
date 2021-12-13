using Newtonsoft.Json;
using Newtonsoft.Json.Converters;
using System.ComponentModel;

namespace NGO.Elements.Special.Complectation
{
    [JsonConverter(typeof(StringEnumConverter))]
    public enum ZakazType
    {
        [Description("54038")]
        zakaz54038,
        [Description("54042")]
        zakaz54042,
        [Description("54044")]
        zakaz54044,
        [Description("54056")]
        zakaz54056
    }
}
