using System;
using Newtonsoft.Json;

namespace Nop.Plugin.Payments.Crypto.Models
{
    /// <summary>
    /// Represents a request to create refund
    /// </summary>
    public class CreateRefundRequest
    {
        #region Properties

        /// <summary>
        /// Gets or sets the unique identifier for the payment object
        /// </summary>
        /// <remarks>
        /// Required parameter
        /// </remarks>
        [JsonProperty("payment_id")]
        public Guid PaymentId { get; set; }

        /// <summary>
        /// Gets or sets the positive integer representing how much to collect in the smallest currency unit
        /// </summary>
        /// <remarks>
        /// Should be positive integer or zero. Required parameter
        /// </remarks>
        [JsonProperty("amount")]
        public int Amount { get; set; }

        /// <summary>
        /// Gets or sets the arbitrary string attached to the object. Often useful for displaying to users. (Available on non-card refunds only)
        /// </summary>
        /// <remarks>
        /// Optional parameter
        /// </remarks>
        [JsonProperty("description")]
        public string Description { get; set; }

        #endregion
    }
}
