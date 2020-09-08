using System.Collections.Generic;
using Newtonsoft.Json;

namespace Nop.Plugin.Payments.Crypto.Models
{
    /// <summary>
    /// Represents a request to create payment
    /// </summary>
    public class CreatePaymentRequest
    {
        #region Properties

        /// <summary>
        /// Gets or sets the amount, denoted in cents. e.g., 100 cents to charge $1.00
        /// </summary>
        /// <remarks>
        /// Should be positive integer or zero. Required parameter
        /// </remarks>
        [JsonProperty("amount")]
        public int Amount { get; set; }

        /// <summary>
        /// Gets or sets the currency
        /// </summary>
        /// <remarks>
        /// Required parameter
        /// </remarks>
        [JsonProperty("currency")]
        public string Currency { get; set; }

        /// <summary>
        /// Gets or sets the customer id
        /// </summary>
        /// <remarks>
        /// Optional parameter
        /// </remarks>
        [JsonProperty("customer_id")]
        public string CustomerId { get; set; }

        /// <summary>
        /// Gets or sets the order id
        /// </summary>
        /// <remarks>
        /// Optional parameter
        /// </remarks>
        [JsonProperty("order_id")]
        public string OrderId { get; set; }

        /// <summary>
        /// Gets or sets the arbitrary string attached to the object. Often useful for displaying to users
        /// </summary>
        /// <remarks>
        /// Optional parameter
        /// </remarks>
        [JsonProperty("description")]
        public string Description { get; set; }

        /// <summary>
        /// Gets or sets the key-value pairs. This can be useful for storing additional information about the object in a structured format
        /// </summary>
        /// <remarks>
        /// Optional parameter
        /// </remarks>
        [JsonProperty("metadata")]
        public IDictionary<string, string> MetaData { get; set; }

        #endregion
    }
}
