using System;
using System.Threading.Tasks;
using FluentValidation;
using Nop.Core;
using Nop.Plugin.Payments.Crypto.Models;
using Nop.Services.Configuration;
using Nop.Services.Localization;
using Nop.Web.Framework.Validators;

namespace Nop.Plugin.Payments.Crypto.Validators
{
    /// <summary>
    /// Represents a validator for <see cref="ConfigurationModel"/>
    /// </summary>
    public class ConfigurationModelValidator : BaseNopValidator<ConfigurationModel>
    {
        private readonly ISettingService _settingService;
        private readonly IStoreContext _storeContext;

        #region Ctor

        public ConfigurationModelValidator(
            ISettingService settingService,
            IStoreContext storeContext,
            ILocalizationService localizationService)
        {
            _settingService = settingService;
            _storeContext = storeContext;

            RuleFor(model => model.SecretKey)
                .NotEmpty()
                .WithMessageAwait(localizationService.GetResourceAsync("Plugins.Payments.Crypto.Fields.SecretKey.Required"))
                .WhenAwait(async model => !await IsSandboxAsync(model));
            RuleFor(model => model.SandboxSecretKey)
                .NotEmpty()
                .WithMessageAwait(localizationService.GetResourceAsync("Plugins.Payments.Crypto.Fields.SandboxSecretKey.Required"))
                .WhenAwait(async model => await IsSandboxAsync(model));

            RuleFor(model => model.PublishableKey)
                .NotEmpty()
                .WithMessageAwait(localizationService.GetResourceAsync("Plugins.Payments.Crypto.Fields.PublishableKey.Required"))
                .WhenAwait(async model => !await IsSandboxAsync(model));
            RuleFor(model => model.SandboxPublishableKey)
                .NotEmpty()
                .WithMessageAwait(localizationService.GetResourceAsync("Plugins.Payments.Crypto.Fields.SandboxPublishableKey.Required"))
                .WhenAwait(async model => await IsSandboxAsync(model));

            RuleFor(model => model.WebHookSignatureSecretKey)
                .NotEmpty()
                .WithMessageAwait(localizationService.GetResourceAsync("Plugins.Payments.Crypto.Fields.WebHookSignatureSecretKey.Required"))
                .WhenAwait(async model => !await IsSandboxAsync(model));
            RuleFor(model => model.SandboxWebHookSignatureSecretKey)
                .NotEmpty()
                .WithMessageAwait(localizationService.GetResourceAsync("Plugins.Payments.Crypto.Fields.SandboxWebHookSignatureSecretKey.Required"))
                .WhenAwait(async model => await IsSandboxAsync(model));

            RuleFor(model => model.AdditionalFee)
                .GreaterThanOrEqualTo(0)
                .WithMessageAwait(localizationService.GetResourceAsync("Plugins.Payments.Crypto.Fields.AdditionalFee.ShouldBeGreaterThanOrEqualZero"));
        }

        private async Task<bool> IsSandboxAsync(ConfigurationModel model)
        {
            var storeScope = await _storeContext.GetActiveStoreScopeConfigurationAsync();
            var settings = await _settingService.LoadSettingAsync<CryptoPaymentSettings>(storeScope);
            
            return model.UseSandbox || (settings.UseSandbox && !model.UseSandbox_OverrideForStore && storeScope > 0);
        }

        #endregion
    }
}
