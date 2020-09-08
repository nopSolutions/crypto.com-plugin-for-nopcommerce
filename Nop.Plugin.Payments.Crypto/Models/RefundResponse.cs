using System;
using Newtonsoft.Json;

namespace Nop.Plugin.Payments.Crypto.Models
{
    /// <summary>
    /// Represents a refund response
    /// </summary>
    public class RefundResponse
    {
        #region Properties

        /// <summary>
        /// Gets or sets the unique identifier for the object
        /// </summary>
        [JsonProperty("id")]
        public Guid Id { get; set; }

        /// <summary>
        /// Gets or sets the status of the refund
        /// </summary>
        [JsonProperty("status")]
        public RefundStatus Status { get; set; }

        /// <summary>
        /// Gets or sets the unique identifier for the payment object
        /// </summary>
        [JsonProperty("payment_id")]
        public Guid PaymentId { get; set; }

        /// <summary>
        /// Gets or sets the positive integer representing how much to collect in the smallest currency unit
        /// </summary>
        [JsonProperty("amount")]
        public decimal Amount { get; set; }

        /// <summary>
        /// Gets or sets the currency
        /// </summary>
        [JsonProperty("currency")]
        public string Currency { get; set; }

        /// <summary>
        /// Gets or sets the arbitrary string attached to the object. Often useful for displaying to users. (Available on non-card refunds only)
        /// </summary>
        [JsonProperty("description")]
        public string Description { get; set; }

        #endregion
    }
}
