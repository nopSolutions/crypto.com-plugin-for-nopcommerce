using Newtonsoft.Json;

namespace Nop.Plugin.Payments.Crypto.Models
{
    /// <summary>
    /// Represents a web hook data
    /// </summary>
    public class WebHookData
    {
        #region Properties

        /// <summary>
        /// Gets or sets the object
        /// </summary>
        [JsonProperty("object")]
        public object Object { get; set; }

        #endregion
    }
}
