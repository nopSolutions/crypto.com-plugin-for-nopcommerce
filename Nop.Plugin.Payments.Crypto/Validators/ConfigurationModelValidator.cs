using System;
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
        #region Ctor

        public ConfigurationModelValidator(
            ISettingService settingService,
            IStoreContext storeContext,
            ILocalizationService localizationService)
        {
            var storeScope = storeContext.ActiveStoreScopeConfiguration;
            var settings = settingService.LoadSetting<CryptoPaymentSettings>(storeScope);

            bool isSandbox(ConfigurationModel model) => model.UseSandbox || (settings.UseSandbox && !model.UseSandbox_OverrideForStore && storeScope > 0);

            RuleFor(model => model.SecretKey)
                .NotEmpty()
                .WithMessage(localizationService.GetResource("Plugins.Payments.Crypto.Fields.SecretKey.Required"))
                .When(model => !isSandbox(model));
            RuleFor(model => model.SandboxSecretKey)
                .NotEmpty()
                .WithMessage(localizationService.GetResource("Plugins.Payments.Crypto.Fields.SandboxSecretKey.Required"))
                .When(model => isSandbox(model));

            RuleFor(model => model.PublishableKey)
                .NotEmpty()
                .WithMessage(localizationService.GetResource("Plugins.Payments.Crypto.Fields.PublishableKey.Required"))
                .When(model => !isSandbox(model));
            RuleFor(model => model.SandboxPublishableKey)
                .NotEmpty()
                .WithMessage(localizationService.GetResource("Plugins.Payments.Crypto.Fields.SandboxPublishableKey.Required"))
                .When(model => isSandbox(model));

            RuleFor(model => model.WebHookSignatureSecretKey)
                .NotEmpty()
                .WithMessage(localizationService.GetResource("Plugins.Payments.Crypto.Fields.WebHookSignatureSecretKey.Required"))
                .When(model => !isSandbox(model));
            RuleFor(model => model.SandboxWebHookSignatureSecretKey)
                .NotEmpty()
                .WithMessage(localizationService.GetResource("Plugins.Payments.Crypto.Fields.SandboxWebHookSignatureSecretKey.Required"))
                .When(model => isSandbox(model));

            RuleFor(model => model.AdditionalFee)
                .GreaterThanOrEqualTo(0)
                .WithMessage(localizationService.GetResource("Plugins.Payments.Crypto.Fields.AdditionalFee.ShouldBeGreaterThanOrEqualZero"));
        }

        #endregion
    }
}
