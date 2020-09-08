using FluentValidation;
using Nop.Plugin.Payments.Crypto.Models;
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

        public ConfigurationModelValidator(ILocalizationService localizationService)
        {
            RuleFor(model => model.SecretKey)
                .NotEmpty()
                .WithMessage(localizationService.GetResource("Plugins.Payments.Crypto.Fields.SecretKey.Required"))
                .When(model => !model.UseSandbox);
            RuleFor(model => model.SandboxSecretKey)
                .NotEmpty()
                .WithMessage(localizationService.GetResource("Plugins.Payments.Crypto.Fields.SandboxSecretKey.Required"))
                .When(model => model.UseSandbox);

            RuleFor(model => model.PublishableKey)
                .NotEmpty()
                .WithMessage(localizationService.GetResource("Plugins.Payments.Crypto.Fields.PublishableKey.Required"))
                .When(model => !model.UseSandbox);
            RuleFor(model => model.SandboxPublishableKey)
                .NotEmpty()
                .WithMessage(localizationService.GetResource("Plugins.Payments.Crypto.Fields.SandboxPublishableKey.Required"))
                .When(model => model.UseSandbox);

            RuleFor(model => model.WebHookSignatureSecretKey)
                .NotEmpty()
                .WithMessage(localizationService.GetResource("Plugins.Payments.Crypto.Fields.WebHookSignatureSecretKey.Required"))
                .When(model => !model.UseSandbox);
            RuleFor(model => model.SandboxWebHookSignatureSecretKey)
                .NotEmpty()
                .WithMessage(localizationService.GetResource("Plugins.Payments.Crypto.Fields.SandboxWebHookSignatureSecretKey.Required"))
                .When(model => model.UseSandbox);

            RuleFor(model => model.AdditionalFee)
                .GreaterThanOrEqualTo(0)
                .WithMessage(localizationService.GetResource("Plugins.Payments.Crypto.Fields.AdditionalFee.ShouldBeGreaterThanOrEqualZero"));
        }

        #endregion
    }
}
