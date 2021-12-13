using Newtonsoft.Json;
using Newtonsoft.Json.Converters;
using System.ComponentModel;

namespace NGO.Elements.Special.Complectation
{
    [JsonConverter(typeof(StringEnumConverter))]
    public enum Complectations
    {
        [Description("КТУ1 + КТУ2")]
        KTU1_KTU2,
        [Description("КТУ1")]
        KTU1,
        [Description("КТУ2")]
        KTU2,
    }
}
