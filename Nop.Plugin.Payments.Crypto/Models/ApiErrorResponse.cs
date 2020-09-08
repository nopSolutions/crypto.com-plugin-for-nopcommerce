using Newtonsoft.Json;

namespace Nop.Plugin.Payments.Crypto.Models
{
    /// <summary>
    /// Represents a API error response
    /// </summary>
    public class ApiErrorResponse
    {
        #region Properties

        /// <summary>
        /// Gets or sets the API error
        /// </summary>
        [JsonProperty("error")]
        public ApiError Error { get; set; }

        #endregion
    }
}
