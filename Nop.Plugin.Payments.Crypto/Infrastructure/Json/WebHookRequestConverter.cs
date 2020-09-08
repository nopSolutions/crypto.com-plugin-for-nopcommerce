using System;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using Nop.Plugin.Payments.Crypto.Models;

namespace Nop.Plugin.Payments.Crypto.Infrastructure.Json
{
    /// <summary>
    /// Represents an json converter to <see cref="WebHookRequest" />.
    /// </summary>
    public class WebHookRequestConverter : JsonCreationConverter<WebHookRequest>
    {
        #region Methods

        /// <summary>
        /// Create an instance of object, based on properties in the JSON object.
        /// </summary>
        /// <param name="objectType">The type of object expected.</param>
        /// <param name="jObject">The contents of JSON object that will be deserialized.</param>
        /// <param name="serializer">The calling serializer.</param>
        /// <returns>The object value.</returns>
        protected override WebHookRequest Create(Type objectType, JObject jObject, JsonSerializer serializer)
        {
            var result = new WebHookRequest();

            if (jObject.HasValues && jObject["type"] != null)
            {
                result.Data = new WebHookData
                {
                    Object = jObject["type"].Value<string>() switch
                    {
                        var refundType when
                            refundType == "payment.refund_requested" ||
                            refundType == "payment.refund_transferred" => jObject["data"]["object"].ToObject<RefundResponse>(serializer),
                        var paymentType when
                            paymentType == "payment.created" ||
                            paymentType == "payment.captured" => jObject["data"]["object"].ToObject<PaymentResponse>(serializer),
                        _ => null
                    }
                };
            }

            return result;
        }
            
        #endregion
    }
}
