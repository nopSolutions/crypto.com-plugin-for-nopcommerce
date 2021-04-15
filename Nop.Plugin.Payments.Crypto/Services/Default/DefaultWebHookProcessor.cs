using System;
using System.Collections.Generic;
using System.IO;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Newtonsoft.Json;
using Nop.Plugin.Payments.Crypto.Models;
using Nop.Services.Common;
using Nop.Services.Logging;
using Nop.Services.Orders;

namespace Nop.Plugin.Payments.Crypto.Services
{
    /// <summary>
    /// Provides an default implementation to process the web hooks
    /// </summary>
    public class DefaultWebHookProcessor : IWebHookProcessor
    {
        #region Fields

        private readonly CryptoPaymentSettings _cryptoPaymentSettings;
        private readonly DefaultPaymentHttpClient _paymentApi;
        private readonly IGenericAttributeService _genericAttributeService;
        private readonly ILogger _logger;
        private readonly IOrderService _orderService;
        private readonly IOrderProcessingService _orderProcessingService;

        #endregion

        #region Ctor

        public DefaultWebHookProcessor(
            CryptoPaymentSettings cryptoPaymentSettings,
            DefaultPaymentHttpClient paymentApi,
            IGenericAttributeService genericAttributeService,
            ILogger logger,
            IOrderService orderService,
            IOrderProcessingService orderProcessingService)
        {
            _cryptoPaymentSettings = cryptoPaymentSettings;
            _paymentApi = paymentApi;
            _genericAttributeService = genericAttributeService;
            _logger = logger;
            _orderService = orderService;
            _orderProcessingService = orderProcessingService;
        }

        #endregion

        #region Methods

        /// <summary>
        /// Processes the web hook request
        /// </summary>
        /// <param name="httpRequest">The HTTP request</param>
        /// <returns>The <see cref="Task"/></returns>
        public virtual async Task ProcessAsync(HttpRequest httpRequest)
        {
            if (httpRequest == null)
                throw new ArgumentNullException(nameof(httpRequest));

            using var streamReader = new StreamReader(httpRequest.Body);
            var rawBody = await streamReader.ReadToEndAsync();

            if (await CanProcessAsync(httpRequest, rawBody))
            {
                WebHookRequest request = null;
                try
                {
                    request = JsonConvert.DeserializeObject<WebHookRequest>(rawBody);
                }
                catch (Exception exception)
                {
                    await _logger.ErrorAsync($"{Defaults.SystemName}: Invalid the web hook request deserialization.", exception);
                }

                if (request?.Data?.Object != null)
                {
                    if (request.Data.Object is RefundResponse refundResponse)
                        await ProcessRefundResponseAsync(request, refundResponse);
                }
            }
        }

        protected virtual async Task ProcessRefundResponseAsync(WebHookRequest request, RefundResponse refundResponse)
        {
            if (refundResponse.PaymentId == Guid.Empty)
                return;

            // only transferred refunds
            if (request.Type != "payment.refund_transferred")
                return;

            try
            {
                var paymentResponse = await _paymentApi.GetPaymentAsync(refundResponse.PaymentId);
                if (paymentResponse != null)
                {
                    if (!Guid.TryParse(paymentResponse.OrderId, out var orderGuid))
                        return;

                    var order = await _orderService.GetOrderByGuidAsync(orderGuid);
                    if (order != null)
                    {
                        var refundIds = await _genericAttributeService.GetAttributeAsync<List<string>>(order, Defaults.RefundIdAttributeName)
                            ?? new List<string>();
                        var refundId = refundResponse.Id.ToString();
                        if (!refundIds.Contains(refundId))
                        {
                            if (paymentResponse.Amount == refundResponse.Amount)
                            {
                                if (_orderProcessingService.CanRefundOffline(order))
                                    await _orderProcessingService.RefundOfflineAsync(order);
                            }
                            else
                            {
                                if (_orderProcessingService.CanPartiallyRefundOffline(order, refundResponse.Amount / 100))
                                    await _orderProcessingService.PartiallyRefundOfflineAsync(order, refundResponse.Amount / 100);
                            }

                            refundIds.Add(refundId);
                            await _genericAttributeService.SaveAttributeAsync(order, Defaults.RefundIdAttributeName, refundIds);
                        }
                    }
                }
            }
            catch (ApiException exception)
            {
                await _logger.ErrorAsync($"{Defaults.SystemName}: {exception.Message}", exception);
            }
        }

        protected virtual async Task<bool> CanProcessAsync(HttpRequest httpRequest, string rawBody)
        {
            if (!httpRequest.Headers.TryGetValue(Defaults.WebHooks.SignatureHeaderName, out var signature))
                return false;

            var parameters = signature.ToString().Split(',');
            if (parameters.Length != 2)
                return false;

            var timestampPair = parameters[0].Split('=');
            if (timestampPair.Length != 2)
                return false;

            var expectedSignaturePair = parameters[1].Split('=');
            if (expectedSignaturePair.Length != 2)
                return false;

            var timestamp = timestampPair[1];
            var expectedSignature = expectedSignaturePair[1];

            var payload = $"{timestamp}.{rawBody}";

            try
            {
                var actualSignature = HmacSha256Digest(_cryptoPaymentSettings.WebHookSignatureSecretKey, payload);
                return actualSignature == expectedSignature;
            }
            catch (Exception exception)
            {
                await _logger.ErrorAsync($"{Defaults.SystemName}: Invalid signature verification. Make sure that the correct 'Web hook signature secret' is specified in the plugin settings.", exception);
            }

            return false;
        }

        #endregion

        #region Utilities

        public static string HmacSha256Digest(string secret, string message)
        {
            var keyBytes = Encoding.ASCII.GetBytes(secret);
            var messageBytes = Encoding.ASCII.GetBytes(message);
            using var cryptographer = new HMACSHA256(keyBytes);

            var hashBytes = cryptographer.ComputeHash(messageBytes);

            return BitConverter.ToString(hashBytes).Replace("-", "").ToLower();
        }

        #endregion
    }
}
