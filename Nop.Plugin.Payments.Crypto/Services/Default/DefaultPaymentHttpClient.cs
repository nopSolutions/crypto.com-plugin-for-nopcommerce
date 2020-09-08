using System;
using System.Net.Http;
using System.Threading.Tasks;
using Nop.Plugin.Payments.Crypto.Models;

namespace Nop.Plugin.Payments.Crypto.Services
{
    /// <summary>
    /// Provides an default implementation the HTTP client to interact with the payment endpoint.
    /// </summary>
    public class DefaultPaymentHttpClient : BaseHttpClient
    {
        #region Ctor

        public DefaultPaymentHttpClient(CryptoPaymentSettings settings, HttpClient httpClient)
            : base(settings, httpClient)
        {
        }

        #endregion

        #region Methods

        public Task<PaymentResponse> GetPaymentAsync(Guid paymentId)
        {
            if (paymentId == Guid.Empty)
                throw new ApiException(400, $"Error when calling '{nameof(GetPaymentAsync)}'. HTTP status code - 400. Payment id should not be empty guid.");

            return GetAsync<PaymentResponse>($"{Defaults.Api.Endpoints.PaymentPath}/{paymentId}");
        }

        public Task<PaymentResponse> CreatePaymentAsync(CreatePaymentRequest request)
        {
            if (request == null)
                throw new ArgumentNullException(nameof(request));

            return PostAsync<PaymentResponse>(Defaults.Api.Endpoints.PaymentPath, request);
        }

        public Task<PaymentResponse> CancelPaymentAsync(Guid paymentId)
        {
            if (paymentId == Guid.Empty)
                throw new ApiException(400, $"Error when calling '{nameof(CancelPaymentAsync)}'. HTTP status code - 400. Payment id should not be empty guid.");

            return PostAsync<PaymentResponse>($"{Defaults.Api.Endpoints.PaymentPath}/{paymentId}/cancel");
        }

        #endregion
    }
}
