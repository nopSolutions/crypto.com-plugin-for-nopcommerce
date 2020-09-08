using System.Runtime.Serialization;
using Newtonsoft.Json;
using Newtonsoft.Json.Converters;

namespace Nop.Plugin.Payments.Crypto.Models
{
    /// <summary>
    /// Represents a payment status
    /// </summary>
    [JsonConverter(typeof(StringEnumConverter))]
    public enum PaymentStatus
    {
        [EnumMember(Value = "pending")]
        Pending,

        [EnumMember(Value = "succeeded")]
        Succeeded,

        [EnumMember(Value = "cancelled")]
        Cancelled,
    }
}
