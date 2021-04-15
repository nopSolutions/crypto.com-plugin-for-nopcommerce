using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Nop.Core;
using Nop.Plugin.Payments.Crypto.Models;
using Nop.Services.Configuration;
using Nop.Services.Localization;
using Nop.Services.Messages;
using Nop.Services.Security;
using Nop.Web.Framework;
using Nop.Web.Framework.Controllers;
using Nop.Web.Framework.Mvc.Filters;

namespace Nop.Plugin.Payments.Crypto.Controllers
{
    [Area(AreaNames.Admin)]
    [AuthorizeAdmin]
    [AutoValidateAntiforgeryToken]
    public class CryptoConfigurationController : BasePluginController
    {
        #region Fields

        private readonly ILocalizationService _localizationService;
        private readonly INotificationService _notificationService;
        private readonly IPermissionService _permissionService;
        private readonly ISettingService _settingService;
        private readonly IStoreContext _storeContext;

        #endregion

        #region Ctor

        public CryptoConfigurationController(
            ILocalizationService localizationService,
            INotificationService notificationService,
            IPermissionService permissionService,
            ISettingService settingService,
            IStoreContext storeContext)
        {
            _localizationService = localizationService;
            _notificationService = notificationService;
            _permissionService = permissionService;
            _settingService = settingService;
            _storeContext = storeContext;
        }

        #endregion

        #region Methods

        public async Task<IActionResult> Configure()
        {
            if (!await _permissionService.AuthorizeAsync(StandardPermissionProvider.ManagePaymentMethods))
                return AccessDeniedView();

            //load settings for a chosen store scope
            var storeScope = await _storeContext.GetActiveStoreScopeConfigurationAsync();
            var settings = await _settingService.LoadSettingAsync<CryptoPaymentSettings>(storeScope);

            var model = new ConfigurationModel
            {
                UseSandbox = settings.UseSandbox,
                AdditionalFee = settings.AdditionalFee,
                AdditionalFeePercentage = settings.AdditionalFeePercentage,
                ActiveStoreScopeConfiguration = storeScope
            };

            if (model.UseSandbox)
            {
                model.SandboxSecretKey = settings.SecretKey;
                model.SandboxPublishableKey = settings.PublishableKey;
                model.SandboxWebHookSignatureSecretKey = settings.WebHookSignatureSecretKey;
            }
            else
            {
                model.SecretKey = settings.SecretKey;
                model.PublishableKey = settings.PublishableKey;
                model.WebHookSignatureSecretKey = settings.WebHookSignatureSecretKey;
            }

            if (storeScope > 0)
            {
                model.UseSandbox_OverrideForStore = await _settingService.SettingExistsAsync(settings, x => x.UseSandbox, storeScope);
                model.AdditionalFee_OverrideForStore = await _settingService.SettingExistsAsync(settings, x => x.AdditionalFee, storeScope);
                model.AdditionalFeePercentage_OverrideForStore = await _settingService.SettingExistsAsync(settings, x => x.AdditionalFeePercentage, storeScope);

                if (model.UseSandbox)
                {
                    model.SandboxSecretKey_OverrideForStore = await _settingService.SettingExistsAsync(settings, x => x.SecretKey, storeScope);
                    model.SandboxPublishableKey_OverrideForStore = await _settingService.SettingExistsAsync(settings, x => x.PublishableKey, storeScope);
                    model.SandboxWebHookSignatureSecretKey_OverrideForStore = await _settingService.SettingExistsAsync(settings, x => x.WebHookSignatureSecretKey, storeScope);
                }
                else
                {
                    model.SecretKey_OverrideForStore = await _settingService.SettingExistsAsync(settings, x => x.SecretKey, storeScope);
                    model.PublishableKey_OverrideForStore = await _settingService.SettingExistsAsync(settings, x => x.PublishableKey, storeScope);
                    model.WebHookSignatureSecretKey_OverrideForStore = await _settingService.SettingExistsAsync(settings, x => x.WebHookSignatureSecretKey, storeScope);
                }
            }

            return View("~/Plugins/Payments.Crypto/Views/Configure.cshtml", model);
        }

        [HttpPost]
        public async Task<IActionResult> Configure(ConfigurationModel model)
        {
            if (!await _permissionService.AuthorizeAsync(StandardPermissionProvider.ManagePaymentMethods))
                return AccessDeniedView();

            if (!ModelState.IsValid)
                return await Configure();

            //load settings for a chosen store scope
            var storeScope = await _storeContext.GetActiveStoreScopeConfigurationAsync();
            var settings = await _settingService.LoadSettingAsync<CryptoPaymentSettings>(storeScope);

            if (model.UseSandbox || (settings.UseSandbox && !model.UseSandbox_OverrideForStore && storeScope > 0))
            {
                settings.SecretKey = model.SandboxSecretKey;
                settings.PublishableKey = model.SandboxPublishableKey;
                settings.WebHookSignatureSecretKey = model.SandboxWebHookSignatureSecretKey;

                await _settingService.SaveSettingOverridablePerStoreAsync(settings, setting => setting.SecretKey, model.SandboxSecretKey_OverrideForStore, storeScope, false);
                await _settingService.SaveSettingOverridablePerStoreAsync(settings, setting => setting.PublishableKey, model.SandboxPublishableKey_OverrideForStore, storeScope, false);
                await _settingService.SaveSettingOverridablePerStoreAsync(settings, setting => setting.WebHookSignatureSecretKey, model.SandboxWebHookSignatureSecretKey_OverrideForStore, storeScope, false);
            }
            else
            {
                settings.SecretKey = model.SecretKey;
                settings.PublishableKey = model.PublishableKey;
                settings.WebHookSignatureSecretKey = model.WebHookSignatureSecretKey;

                await _settingService.SaveSettingOverridablePerStoreAsync(settings, setting => setting.SecretKey, model.SecretKey_OverrideForStore, storeScope, false);
                await _settingService.SaveSettingOverridablePerStoreAsync(settings, setting => setting.PublishableKey, model.PublishableKey_OverrideForStore, storeScope, false);
                await _settingService.SaveSettingOverridablePerStoreAsync(settings, setting => setting.WebHookSignatureSecretKey, model.WebHookSignatureSecretKey_OverrideForStore, storeScope, false);
            }

            settings.UseSandbox = model.UseSandbox;
            settings.AdditionalFee = model.AdditionalFee;
            settings.AdditionalFeePercentage = model.AdditionalFeePercentage;

            await _settingService.SaveSettingOverridablePerStoreAsync(settings, setting => setting.UseSandbox, model.UseSandbox_OverrideForStore, storeScope, false);
            await _settingService.SaveSettingOverridablePerStoreAsync(settings, setting => setting.AdditionalFee, model.AdditionalFee_OverrideForStore, storeScope, false);
            await _settingService.SaveSettingOverridablePerStoreAsync(settings, setting => setting.AdditionalFeePercentage, model.AdditionalFeePercentage_OverrideForStore, storeScope, false);
            await _settingService.ClearCacheAsync();

            _notificationService.SuccessNotification(await _localizationService.GetResourceAsync("Admin.Plugins.Saved"));

            return RedirectToAction("Configure");
        }

        #endregion
    }
}
