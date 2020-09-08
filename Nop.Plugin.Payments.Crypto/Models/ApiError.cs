using Newtonsoft.Json;

namespace Nop.Plugin.Payments.Crypto.Models
{
    /// <summary>
    /// Represents a API error
    /// </summary>
    public class ApiError
    {
        #region Properties

        /// <summary>
        /// Gets or sets the error type
        /// </summary>
        [JsonProperty("type")] 
        public string Type { get; set; }

        /// <summary>
        /// Gets or sets the error code
        /// </summary>
        [JsonProperty("code")]
        public string Code { get; set; }

        /// <summary>
        /// Gets or sets the parameter on which the error occurred
        /// </summary>
        [JsonProperty("param")]
        public string Param { get; set; }

        #endregion
    }
}
