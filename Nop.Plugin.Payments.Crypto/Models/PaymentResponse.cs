using System;
using System.Collections.Generic;
using Newtonsoft.Json;

namespace Nop.Plugin.Payments.Crypto.Models
{
    /// <summary>
    /// Represents a payment response
    /// </summary>
    public class PaymentResponse
    {
        #region Properties

        /// <summary>
        /// Gets or sets the unique identifier for the object
        /// </summary>
        [JsonProperty("id")]
        public Guid Id { get; set; }

        /// <summary>
        /// Gets or sets the amount, denoted in cents. e.g., 100 cents to charge $1.00
        /// </summary>
        [JsonProperty("amount")]
        public int Amount { get; set; }

        /// <summary>
        /// Gets or sets the amount in cents refunded (can be less than the amount attribute on the payment if a partial refund was issued)
        /// </summary>
        [JsonProperty("amount_refunded")]
        public int AmountRefunded { get; set; }

        /// <summary>
        /// Gets or sets the time at which the object was created. Measured in seconds since the Unix epoch
        /// </summary>
        [JsonProperty("created")]
        public long Created { get; set; }

        /// <summary>
        /// Gets or sets the currency of the crypto to be collected for this payment
        /// </summary>
        [JsonProperty("crypto_currency")]
        public string CryptoCurrency { get; set; }

        /// <summary>
        /// Gets or sets the amount of crypto currency to be collected for this payment
        /// </summary>
        [JsonProperty("crypto_amount")]
        public string CryptoAmount { get; set; }

        /// <summary>
        /// Gets or sets the currency
        /// </summary>
        [JsonProperty("currency")]
        public string Currency { get; set; }

        /// <summary>
        /// Gets or sets the customer id
        /// </summary>
        [JsonProperty("customer_id")]
        public string CustomerId { get; set; }

        /// <summary>
        /// Gets or sets the URL which contains the data that the wallet apps need to complete the payment
        /// </summary>
        [JsonProperty("data_url")]
        public string DataUrl { get; set; }

        /// <summary>
        /// Gets or sets the arbitrary string attached to the object. Often useful for displaying to users
        /// </summary>
        [JsonProperty("description")]
        public string Description { get; set; }

        /// <summary>
        /// Gets or sets the value indicating whether to use live mode, has true if the object exists in live mode or the value false if the object exists in test mode
        /// </summary>
        [JsonProperty("live_mode")]
        public bool LiveMode { get; set; }

        /// <summary>
        /// Gets or sets the order id
        /// </summary>
        [JsonProperty("order_id")]
        public string OrderId { get; set; }

        /// <summary>
        /// Gets or sets the name of the merchant collecting this payment
        /// </summary>
        [JsonProperty("recipient")]
        public string Recipient { get; set; }

        /// <summary>
        /// Gets or sets the value indicating whether the charge has been fully refunded. If the charge is only partially refunded, this attribute will still be false
        /// </summary>
        [JsonProperty("refunded")]
        public bool Refunded { get; set; }

        /// <summary>
        /// Gets or sets the status of the payment
        /// </summary>
        [JsonProperty("status")]
        public PaymentStatus Status { get; set; }

        /// <summary>
        /// Gets or sets the time window in seconds before the payment expires
        /// </summary>
        [JsonProperty("time_window")]
        public int TimeWindow { get; set; }

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
