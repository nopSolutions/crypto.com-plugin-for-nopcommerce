using Nop.Web.Framework.Models;
using Nop.Web.Framework.Mvc.ModelBinding;

namespace Nop.Plugin.Payments.Crypto.Models
{
    /// <summary>
    /// Represents a plugin configuration model
    /// </summary>
    public record ConfigurationModel : BaseNopModel
    {
        #region Properties

        public int ActiveStoreScopeConfiguration { get; set; }

        /// <summary>
        /// Gets or sets a value indicating whether to use sandbox environment
        /// </summary>
        [NopResourceDisplayName("Plugins.Payments.Crypto.Fields.UseSandbox")]
        public bool UseSandbox { get; set; }
        public bool UseSandbox_OverrideForStore { get; set; }

        /// <summary>
        /// Gets or sets a secret key
        /// </summary>
        [NopResourceDisplayName("Plugins.Payments.Crypto.Fields.SecretKey")]
        public string SecretKey { get; set; }
        public bool SecretKey_OverrideForStore { get; set; }

        /// <summary>
        /// Gets or sets a sandbox secret key
        /// </summary>
        [NopResourceDisplayName("Plugins.Payments.Crypto.Fields.SandboxSecretKey")]
        public string SandboxSecretKey { get; set; }
        public bool SandboxSecretKey_OverrideForStore { get; set; }

        /// <summary>
        /// Gets or sets a publishable key
        /// </summary>
        [NopResourceDisplayName("Plugins.Payments.Crypto.Fields.PublishableKey")]
        public string PublishableKey { get; set; }
        public bool PublishableKey_OverrideForStore { get; set; }

        /// <summary>
        /// Gets or sets a sandbox publishable key
        /// </summary>
        [NopResourceDisplayName("Plugins.Payments.Crypto.Fields.SandboxPublishableKey")]
        public string SandboxPublishableKey { get; set; }
        public bool SandboxPublishableKey_OverrideForStore { get; set; }

        /// <summary>
        /// Gets or sets a web hook signature secret key
        /// </summary>
        [NopResourceDisplayName("Plugins.Payments.Crypto.Fields.WebHookSignatureSecretKey")]
        public string WebHookSignatureSecretKey { get; set; }
        public bool WebHookSignatureSecretKey_OverrideForStore { get; set; }

        /// <summary>
        /// Gets or sets a sandbox web hook signature secret key
        /// </summary>
        [NopResourceDisplayName("Plugins.Payments.Crypto.Fields.SandboxWebHookSignatureSecretKey")]
        public string SandboxWebHookSignatureSecretKey { get; set; }
        public bool SandboxWebHookSignatureSecretKey_OverrideForStore { get; set; }

        /// <summary>
        /// Gets or sets an additional fee
        /// </summary>
        [NopResourceDisplayName("Plugins.Payments.Crypto.Fields.AdditionalFee")]
        public decimal AdditionalFee { get; set; }
        public bool AdditionalFee_OverrideForStore { get; set; }

        /// <summary>
        /// Gets or sets a value indicating whether to "additional fee" is specified as percentage. true - percentage, false - fixed value.
        /// </summary>
        [NopResourceDisplayName("Plugins.Payments.Crypto.Fields.AdditionalFeePercentage")]
        public bool AdditionalFeePercentage { get; set; }
        public bool AdditionalFeePercentage_OverrideForStore { get; set; }

        #endregion
    }
}
