using System;
using Newtonsoft.Json;
using Nop.Plugin.Payments.Crypto.Infrastructure.Json;

namespace Nop.Plugin.Payments.Crypto.Models
{
    /// <summary>
    /// Represents a web hook request
    /// </summary>
    [JsonConverter(typeof(WebHookRequestConverter))]
    public class WebHookRequest
    {
        #region Properties

        /// <summary>
        /// Gets or sets the unique identifier for the object
        /// </summary>
        [JsonProperty("id")]
        public Guid Id { get; set; }

        /// <summary>
        /// Gets or sets the object type
        /// </summary>
        [JsonProperty("object_type")]
        public string ObjectType { get; set; }
        
        /// <summary>
        /// Gets or sets the type
        /// </summary>
        [JsonProperty("type")]
        public string Type { get; set; }

        /// <summary>
        /// Gets or sets the time at which the object was created. Measured in seconds since the Unix epoch
        /// </summary>
        [JsonProperty("created")]
        public long Created { get; set; }

        /// <summary>
        /// Gets or sets the data
        /// </summary>
        [JsonProperty("data")]
        public WebHookData Data { get; set; }

        #endregion
    }
}
