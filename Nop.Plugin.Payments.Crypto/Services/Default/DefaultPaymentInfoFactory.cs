using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Nop.Core;
using Nop.Core.Domain.Directory;
using Nop.Core.Domain.Orders;
using Nop.Core.Http.Extensions;
using Nop.Plugin.Payments.Crypto.Models;
using Nop.Services.Directory;
using Nop.Services.Localization;
using Nop.Services.Logging;
using Nop.Services.Orders;
using Nop.Services.Payments;

namespace Nop.Plugin.Payments.Crypto.Services
{
    /// <summary>
    /// Provides an default implementation for factory to create the payment info model
    /// </summary>
    public class DefaultPaymentInfoFactory : IPaymentInfoFactory
    {
        #region Properties

        private readonly DefaultPaymentHttpClient _paymentApi;
        private readonly ICurrencyService _currencyService;
        private readonly ILocalizationService _localizationService;
        private readonly ILogger _logger;
        private readonly IPaymentService _paymentService;
        private readonly IShoppingCartService _shoppingCartService;
        private readonly IOrderTotalCalculationService _orderTotalCalculationService;
        private readonly IHttpContextAccessor _httpContextAccessor;
        private readonly IStoreContext _storeContext;
        private readonly IWorkContext _workContext;
        private readonly CurrencySettings _currencySettings;
        private readonly CryptoPaymentSettings _cryptoPaymentSettings;

        #endregion

        #region Ctor

        public DefaultPaymentInfoFactory(
            DefaultPaymentHttpClient paymentApi,
            ICurrencyService currencyService,
            ILocalizationService localizationService,
            ILogger logger,
            IPaymentService paymentService,
            IShoppingCartService shoppingCartService,
            IOrderTotalCalculationService orderTotalCalculationService,
            IHttpContextAccessor httpContextAccessor,
            IStoreContext storeContext,
            IWorkContext workContext,
            CurrencySettings currencySettings,
            CryptoPaymentSettings cryptoPaymentSettings)
        {
            _paymentApi = paymentApi;
            _currencyService = currencyService;
            _localizationService = localizationService;
            _logger = logger;
            _currencySettings = currencySettings;
            _paymentService = paymentService;
            _shoppingCartService = shoppingCartService;
            _workContext = workContext;
            _orderTotalCalculationService = orderTotalCalculationService;
            _httpContextAccessor = httpContextAccessor;
            _storeContext = storeContext;
            _cryptoPaymentSettings = cryptoPaymentSettings;
        }

        #endregion

        #region Methods

        /// <summary>
        /// Creates the payment info model
        /// </summary>
        /// <returns>The <see cref="Task"/> containing the payment info model</returns>
        public virtual async Task<PaymentInfoModel> CreatePaymentInfoAsync()
        {
            if (string.IsNullOrWhiteSpace(_cryptoPaymentSettings.SecretKey))
                return null;

            var customer = _workContext.CurrentCustomer;
            if (customer == null)
                return null;

            var request = new CreatePaymentRequest();

            // set currency
            if (_workContext.WorkingCurrency == null)
                return null;

            var currency = _currencyService.GetCurrencyById(_currencySettings.PrimaryStoreCurrencyId);
            if (currency == null)
                return null;

            request.Currency = currency.CurrencyCode;

            // set payment amount
            var cart = _shoppingCartService.GetShoppingCart(customer, ShoppingCartType.ShoppingCart, _storeContext.CurrentStore.Id);
            if (cart?.Any() == true)
            {
                var subTotal = _orderTotalCalculationService.GetShoppingCartTotal(cart);

                request.Amount = subTotal.HasValue
                    ? (int)(subTotal.Value * 100)
                    : 0;
            }

            // set order id
            var httpContext = _httpContextAccessor.HttpContext;

            var processPaymentRequest = new ProcessPaymentRequest();
            _paymentService.GenerateOrderGuid(processPaymentRequest);
            request.OrderId = processPaymentRequest.OrderGuid.ToString();

            try
            {
                var paymentResponse = await _paymentApi.CreatePaymentAsync(request);
                if (paymentResponse != null)
                {
                    var paymentId = paymentResponse.Id.ToString();

                    processPaymentRequest.CustomValues.Add(_localizationService.GetResource("Plugins.Payments.Crypto.PaymentId"), paymentId);
                    httpContext.Session.Set(Defaults.PaymentRequestSessionKey, processPaymentRequest);

                    return new PaymentInfoModel
                    {
                        PaymentId = paymentId
                    };
                }
            }
            catch (ApiException exception)
            {
                _logger.Error($"{Defaults.SystemName}: {exception.Message}", exception, customer);
            }

            return null;
        }

        #endregion
    }
}
