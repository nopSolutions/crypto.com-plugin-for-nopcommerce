using System;
using System.Net.Http;
using System.Threading.Tasks;
using Nop.Plugin.Payments.Crypto.Models;

namespace Nop.Plugin.Payments.Crypto.Services
{
    /// <summary>
    /// Provides an default implementation the HTTP client to interact with the refund endpoint.
    /// </summary>
    public class DefaultRefundHttpClient : BaseHttpClient
    {
        #region Ctor

        public DefaultRefundHttpClient(CryptoPaymentSettings settings, HttpClient httpClient)
            : base(settings, httpClient)
        {
        }

        #endregion

        #region Methods

        public Task<RefundResponse> GetRefundAsync(Guid refundId)
        {
            if (refundId == Guid.Empty)
                throw new ApiException(400, $"Error when calling '{nameof(GetRefundAsync)}'. HTTP status code - 400. Refund id should not be empty guid.");

            return GetAsync<RefundResponse>(Defaults.Api.Endpoints.RefundPath);
        }

        public Task<RefundResponse> CreateRefundAsync(CreateRefundRequest request)
        {
            if (request == null)
                throw new ArgumentNullException(nameof(request));

            return PostAsync<RefundResponse>(Defaults.Api.Endpoints.RefundPath, request);
        }

        #endregion
    }
}
