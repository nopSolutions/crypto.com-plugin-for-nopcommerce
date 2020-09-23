﻿using System;
using System.Collections.Generic;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Infrastructure;
using Microsoft.AspNetCore.Mvc.Routing;
using Nop.Core;
using Nop.Core.Domain.Cms;
using Nop.Core.Domain.Orders;
using Nop.Core.Http.Extensions;
using Nop.Plugin.Payments.Crypto.Models;
using Nop.Plugin.Payments.Crypto.Services;
using Nop.Services.Cms;
using Nop.Services.Common;
using Nop.Services.Configuration;
using Nop.Services.Localization;
using Nop.Services.Logging;
using Nop.Services.Payments;
using Nop.Services.Plugins;
using Nop.Web.Framework.Infrastructure;

namespace Nop.Plugin.Payments.Crypto
{
    /// <summary>
    /// Represents a payment method implementation
    /// </summary>
    public class CryptoPaymentMethod : BasePlugin, IPaymentMethod, IWidgetPlugin
    {
        #region Fields

        private readonly CryptoPaymentSettings _cryptoPaymentSettings;
        private readonly DefaultPaymentHttpClient _paymentApi;
        private readonly DefaultRefundHttpClient _refundApi;
        private readonly IActionContextAccessor _actionContextAccessor;
        private readonly IGenericAttributeService _genericAttributeService;
        private readonly IPaymentService _paymentService;
        private readonly ILocalizationService _localizationService;
        private readonly ILogger _logger;
        private readonly ISettingService _settingService;
        private readonly IUrlHelperFactory _urlHelperFactory;
        private readonly WidgetSettings _widgetSettings;

        #endregion

        #region Ctor

        public CryptoPaymentMethod(
            CryptoPaymentSettings cryptoPaymentSettings,
            DefaultPaymentHttpClient paymentApi,
            DefaultRefundHttpClient refundApi,
            IActionContextAccessor actionContextAccessor,
            IGenericAttributeService genericAttributeService,
            IPaymentService paymentService,
            ILocalizationService localizationService,
            ILogger logger,
            ISettingService settingService,
            IUrlHelperFactory urlHelperFactory,
            WidgetSettings widgetSettings)
        {
            _cryptoPaymentSettings = cryptoPaymentSettings;
            _paymentApi = paymentApi;
            _refundApi = refundApi;
            _actionContextAccessor = actionContextAccessor;
            _genericAttributeService = genericAttributeService;
            _paymentService = paymentService;
            _localizationService = localizationService;
            _logger = logger;
            _settingService = settingService;
            _urlHelperFactory = urlHelperFactory;
            _widgetSettings = widgetSettings;
        }

        #endregion

        #region Methods

        /// <summary>
        /// Process a payment
        /// </summary>
        /// <param name="processPaymentRequest">Payment info required for an order processing</param>
        /// <returns>Process payment result</returns>
        public ProcessPaymentResult ProcessPayment(ProcessPaymentRequest processPaymentRequest)
        {
            var paymentIdKey = _localizationService.GetResource("Plugins.Payments.Crypto.PaymentId");
            if (!processPaymentRequest.CustomValues.TryGetValue(paymentIdKey, out var rawPaymentId))
                throw new NopException("Failed to get the Crypto payment id");

            if (string.IsNullOrEmpty(rawPaymentId?.ToString()) || !Guid.TryParse(rawPaymentId.ToString(), out var paymentId))
                throw new NopException("Failed to parse the Crypto payment id");

            try
            {
                var paymentResponse = _paymentApi.GetPaymentAsync(paymentId).GetAwaiter().GetResult();
                if (paymentResponse?.Status == PaymentStatus.Succeeded)
                {
                    return new ProcessPaymentResult
                    {
                        NewPaymentStatus = Core.Domain.Payments.PaymentStatus.Paid,
                        CaptureTransactionId = paymentResponse.Id.ToString()
                    };
                }
            }
            catch (ApiException exception)
            {
                _logger.Error($"{Defaults.SystemName}: {exception.Message}", exception);
            }

            return new ProcessPaymentResult { Errors = new[] { "Failed to get the Crypto payment id" } };
        }

        /// <summary>
        /// Post process payment (used by payment gateways that require redirecting to a third-party URL)
        /// </summary>
        /// <param name="postProcessPaymentRequest">Payment info required for an order processing</param>
        public void PostProcessPayment(PostProcessPaymentRequest postProcessPaymentRequest)
        {
            
        }

        /// <summary>
        /// Returns a value indicating whether payment method should be hidden during checkout
        /// </summary>
        /// <param name="cart">Shopping cart</param>
        /// <returns>true - hide; false - display.</returns>
        public bool HidePaymentMethod(IList<ShoppingCartItem> cart)
        {
            //you can put any logic here
            //for example, hide this payment method if all products in the cart are downloadable
            //or hide this payment method if current customer is from certain country
            return false;
        }

        /// <summary>
        /// Gets additional handling fee
        /// </summary>
        /// <param name="cart">Shopping cart</param>
        /// <returns>Additional handling fee</returns>
        public decimal GetAdditionalHandlingFee(IList<ShoppingCartItem> cart)
        {
            if (cart == null)
                throw new ArgumentNullException(nameof(cart));

            return _paymentService.CalculateAdditionalFee(cart, _cryptoPaymentSettings.AdditionalFee, _cryptoPaymentSettings.AdditionalFeePercentage);
        }

        /// <summary>
        /// Captures payment
        /// </summary>
        /// <param name="capturePaymentRequest">Capture payment request</param>
        /// <returns>Capture payment result</returns>
        public CapturePaymentResult Capture(CapturePaymentRequest capturePaymentRequest)
        {
            return new CapturePaymentResult { Errors = new[] { "Capture method not supported" } };
        }

        /// <summary>
        /// Refunds a payment
        /// </summary>
        /// <param name="refundPaymentRequest">Request</param>
        /// <returns>Result</returns>
        public RefundPaymentResult Refund(RefundPaymentRequest refundPaymentRequest)
        {
            if (refundPaymentRequest == null)
                throw new ArgumentNullException(nameof(refundPaymentRequest));

            var refundRequest = new CreateRefundRequest
            {
                PaymentId = Guid.Parse(refundPaymentRequest.Order.CaptureTransactionId),
                Amount = (int)(refundPaymentRequest.AmountToRefund * 100)
            };

            try
            {
                var refundResponse = _refundApi.CreateRefundAsync(refundRequest).GetAwaiter().GetResult();
                if (refundResponse == null)
                    return new RefundPaymentResult { Errors = new[] { "No refund response" } };

                var refundIds = _genericAttributeService.GetAttribute<List<string>>(refundPaymentRequest.Order, Defaults.RefundIdAttributeName)
                    ?? new List<string>();
                var refundId = refundResponse.Id.ToString();
                if (!refundIds.Contains(refundId))
                    refundIds.Add(refundId);
                _genericAttributeService.SaveAttribute(refundPaymentRequest.Order, Defaults.RefundIdAttributeName, refundIds);

                return new RefundPaymentResult
                {
                    NewPaymentStatus = refundPaymentRequest.IsPartialRefund
                        ? Core.Domain.Payments.PaymentStatus.PartiallyRefunded
                        : Core.Domain.Payments.PaymentStatus.Refunded
                };
            }
            catch (ApiException exception)
            {
                _logger.Error($"{Defaults.SystemName}: {exception.Message}", exception);
                return new RefundPaymentResult { Errors = new[] { exception.Message } };
            }
        }

        /// <summary>
        /// Voids a payment
        /// </summary>
        /// <param name="voidPaymentRequest">Request</param>
        /// <returns>Result</returns>
        public VoidPaymentResult Void(VoidPaymentRequest voidPaymentRequest)
        {
            return new VoidPaymentResult { Errors = new[] { "Void method not supported" } };
        }

        /// <summary>
        /// Process recurring payment
        /// </summary>
        /// <param name="processPaymentRequest">Payment info required for an order processing</param>
        /// <returns>Process payment result</returns>
        public ProcessPaymentResult ProcessRecurringPayment(ProcessPaymentRequest processPaymentRequest)
        {
            return new ProcessPaymentResult { Errors = new[] { "Recurring payment not supported" } };
        }

        /// <summary>
        /// Cancels a recurring payment
        /// </summary>
        /// <param name="cancelPaymentRequest">Request</param>
        /// <returns>Result</returns>
        public CancelRecurringPaymentResult CancelRecurringPayment(CancelRecurringPaymentRequest cancelPaymentRequest)
        {
            return new CancelRecurringPaymentResult { Errors = new[] { "Recurring payment not supported" } };
        }

        /// <summary>
        /// Gets a value indicating whether customers can complete a payment after order is placed but not completed (for redirection payment methods)
        /// </summary>
        /// <param name="order">Order</param>
        /// <returns>Result</returns>
        public bool CanRePostProcessPayment(Order order)
        {
            if (order == null)
                throw new ArgumentNullException(nameof(order));

            return false;
        }

        /// <summary>
        /// Validate payment form
        /// </summary>
        /// <param name="form">The parsed form values</param>
        /// <returns>List of validating errors</returns>
        public IList<string> ValidatePaymentForm(IFormCollection form)
        {
            return new List<string>();
        }

        /// <summary>
        /// Get payment information
        /// </summary>
        /// <param name="form">The parsed form values</param>
        /// <returns>Payment info holder</returns>
        public ProcessPaymentRequest GetPaymentInfo(IFormCollection form)
        {
            if (form == null)
                throw new ArgumentNullException(nameof(form));

            return _actionContextAccessor.ActionContext.HttpContext.Session.Get<ProcessPaymentRequest>(Defaults.PaymentRequestSessionKey);
        }

        /// <summary>
        /// Gets a configuration page URL
        /// </summary>
        public override string GetConfigurationPageUrl()
        {
            return _urlHelperFactory.GetUrlHelper(_actionContextAccessor.ActionContext).RouteUrl(Defaults.ConfigurationRouteName);
        }

        /// <summary>
        /// Gets a name of a view component for displaying plugin in public store ("payment info" checkout step)
        /// </summary>
        /// <returns>View component name</returns>
        public string GetPublicViewComponentName()
        {
            return Defaults.PAYMENT_INFO_VIEW_COMPONENT_NAME;
        }

        public IList<string> GetWidgetZones()
        {
            return new List<string>
            {
                PublicWidgetZones.CheckoutPaymentInfoBottom,
                PublicWidgetZones.OpcContentBefore,
            };
        }

        public string GetWidgetViewComponentName(string widgetZone)
        {
            if (widgetZone == null)
                throw new ArgumentNullException(nameof(widgetZone));

            if (widgetZone.Equals(PublicWidgetZones.CheckoutPaymentInfoBottom) ||
                widgetZone.Equals(PublicWidgetZones.OpcContentBefore))
            {
                return Defaults.CheckoutSdk.SCRIPT_VIEW_COMPONENT_NAME;
            }

            return string.Empty;
        }

        /// <summary>
        /// Install the plugin
        /// </summary>
        public override void Install()
        {
            if (!_widgetSettings.ActiveWidgetSystemNames.Contains(Defaults.SystemName))
            {
                _widgetSettings.ActiveWidgetSystemNames.Add(Defaults.SystemName);
                _settingService.SaveSetting(_widgetSettings);
            }

            _settingService.SaveSetting(new CryptoPaymentSettings
            {
                UseSandbox = true
            });

            _localizationService.AddPluginLocaleResource(new Dictionary<string, string>
            {
                ["Plugins.Payments.Crypto.Fields.AdditionalFee"] = "Additional fee",
                ["Plugins.Payments.Crypto.Fields.AdditionalFee.Hint"] = "Enter additional fee to charge your customers.",
                ["Plugins.Payments.Crypto.Fields.AdditionalFee.ShouldBeGreaterThanOrEqualZero"] = "The additional fee should be greater than or equal 0",
                ["Plugins.Payments.Crypto.Fields.AdditionalFeePercentage"] = "Additional fee. Use percentage",
                ["Plugins.Payments.Crypto.Fields.AdditionalFeePercentage.Hint"] = "Determines whether to apply a percentage additional fee to the order total. If not enabled, a fixed value is used.",
                ["Plugins.Payments.Crypto.Fields.PublishableKey"] = "Publishable key",
                ["Plugins.Payments.Crypto.Fields.PublishableKey.Hint"] = "Enter the publishable key for the live environment.",
                ["Plugins.Payments.Crypto.Fields.PublishableKey.Required"] = "Publishable key is required",
                ["Plugins.Payments.Crypto.Fields.SecretKey"] = "Secret key",
                ["Plugins.Payments.Crypto.Fields.SecretKey.Hint"] = "Enter the secret key for the live environment.",
                ["Plugins.Payments.Crypto.Fields.SecretKey.Required"] = "Secret key is required",
                ["Plugins.Payments.Crypto.Fields.SandboxPublishableKey"] = "Sandbox publishable key",
                ["Plugins.Payments.Crypto.Fields.SandboxPublishableKey.Hint"] = "Enter the sandbox publishable key for the sandbox environment.",
                ["Plugins.Payments.Crypto.Fields.SandboxPublishableKey.Required"] = "Sandbox publishable key is required",
                ["Plugins.Payments.Crypto.Fields.SandboxSecretKey"] = "Sandbox secret key",
                ["Plugins.Payments.Crypto.Fields.SandboxSecretKey.Hint"] = "Enter the sandbox secret key for the sandbox environment.",
                ["Plugins.Payments.Crypto.Fields.SandboxSecretKey.Required"] = "Sandbox secret key is required",
                ["Plugins.Payments.Crypto.Fields.SandboxWebHookSignatureSecretKey"] = "Sandbox web hook signature secret",
                ["Plugins.Payments.Crypto.Fields.SandboxWebHookSignatureSecretKey.Hint"] = "Enter the sandbox web hook signature secret for the sandbox environment.",
                ["Plugins.Payments.Crypto.Fields.SandboxWebHookSignatureSecretKey.Required"] = "Sandbox web hook signature secret is required",
                ["Plugins.Payments.Crypto.Fields.UseSandbox"] = "Use sandbox",
                ["Plugins.Payments.Crypto.Fields.UseSandbox.Hint"] = "Determine whether to use the sandbox environment for testing purposes.",
                ["Plugins.Payments.Crypto.Fields.WebHookSignatureSecretKey"] = "Web hook signature secret",
                ["Plugins.Payments.Crypto.Fields.WebHookSignatureSecretKey.Hint"] = "Enter the web hook signature secret for the live environment.",
                ["Plugins.Payments.Crypto.Fields.WebHookSignatureSecretKey.Required"] = "Web hook signature secret is required",
                ["Plugins.Payments.Crypto.Instructions"] = @"
                    <div style=""margin: 0 0 10px;"">
                        For plugin configuration, follow these steps:<br />
                        <br />
                        1. You will need a Crypto Merchant account. If you don't already have one, you can sign up here: <a href=""https://merchant.crypto.com/users/sign_up"" target=""_blank"">https://merchant.crypto.com/users/sign_up</a><br />
                        2. Sign in to 'Crypto Merchant Dashboard'. Go to 'Developers' tab, copy 'Publishable Key', 'Secret Key' and paste it into the same fields below.<br />
                        3. Go to 'Developers' &#8594; 'Webhooks' tab. Create the new web hook with URL <em>{0}</em>. Copy the 'Signature secret' of the created web hook and paste into the same field below.<br />
                    </div>
                ",
                ["Plugins.Payments.Crypto.PaymentMethodDescription"] = "Pay by Crypto",
                ["Plugins.Payments.Crypto.PaymentInfoIsNotConfigured"] = "Plugin is not configured correctly.",
                ["Plugins.Payments.Crypto.PaymentId"] = "Crypto payment id",
                ["Plugins.Payments.Crypto.Payment.Successful"] = "We have received your payment. Thanks!"
            });

            base.Install();
        }

        /// <summary>
        /// Uninstall the plugin
        /// </summary>
        public override void Uninstall()
        {
            if (_widgetSettings.ActiveWidgetSystemNames.Contains(Defaults.SystemName))
            {
                _widgetSettings.ActiveWidgetSystemNames.Remove(Defaults.SystemName);
                _settingService.SaveSetting(_widgetSettings);
            }

            _settingService.DeleteSetting<CryptoPaymentSettings>();

            _localizationService.DeletePluginLocaleResources("Plugins.Payments.Crypto");

            base.Uninstall();
        }

        #endregion

        #region Properties

        /// <summary>
        /// Gets a value indicating whether capture is supported
        /// </summary>
        public bool SupportCapture => false;

        /// <summary>
        /// Gets a value indicating whether partial refund is supported
        /// </summary>
        public bool SupportPartiallyRefund => true;

        /// <summary>
        /// Gets a value indicating whether refund is supported
        /// </summary>
        public bool SupportRefund => true;

        /// <summary>
        /// Gets a value indicating whether void is supported
        /// </summary>
        public bool SupportVoid => false;

        /// <summary>
        /// Gets a recurring payment type of payment method
        /// </summary>
        public RecurringPaymentType RecurringPaymentType => RecurringPaymentType.NotSupported;

        /// <summary>
        /// Gets a payment method type
        /// </summary>
        public PaymentMethodType PaymentMethodType => PaymentMethodType.Standard;

        /// <summary>
        /// Gets a value indicating whether we should display a payment information page for this plugin
        /// </summary>
        public bool SkipPaymentInfo => false;

        /// <summary>
        /// Gets a payment method description that will be displayed on checkout pages in the public store
        /// </summary>
        public string PaymentMethodDescription => _localizationService.GetResource("Plugins.Payments.Crypto.PaymentMethodDescription");

        /// <summary>
        /// Gets a value indicating whether to hide this plugin on the widget list page in the admin area
        /// </summary>
        public bool HideInWidgetList => true;

        #endregion
    }
}
