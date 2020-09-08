using System;
using System.Net.Http;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Net.Http.Headers;
using Newtonsoft.Json;
using Nop.Core;
using Nop.Plugin.Payments.Crypto.Models;

namespace Nop.Plugin.Payments.Crypto.Services
{
    /// <summary>
    /// Provides an abstraction for the HTTP client to interact with the endponit.
    /// </summary>
    public abstract class BaseHttpClient
    {
        #region Properties

        public HttpClient HttpClient { get; }

        #endregion

        #region Ctor

        public BaseHttpClient(CryptoPaymentSettings settings, HttpClient httpClient)
        {
            httpClient.BaseAddress = new Uri(Defaults.Api.BaseUrl);
            httpClient.Timeout = TimeSpan.FromSeconds(Defaults.Api.DefaultTimeout);
            httpClient.DefaultRequestHeaders.Add(HeaderNames.UserAgent, Defaults.Api.UserAgent);
            httpClient.DefaultRequestHeaders.Add(HeaderNames.Authorization, $"Bearer {settings.SecretKey}");

            HttpClient = httpClient;
        }

        #endregion

        #region Methods

        protected virtual Task<TResponse> GetAsync<TResponse>(string requestUri, [CallerMemberName] string callerName = "")
        {
            return CallAsync<TResponse>(() => HttpClient.GetAsync(requestUri), callerName);
        }

        protected virtual Task<TResponse> PostAsync<TResponse>(string requestUri, object request = null, [CallerMemberName] string callerName = "")
        {
            HttpContent body = null;
            if (request != null)
            {
                var content = JsonConvert.SerializeObject(request);
                body = new StringContent(content, Encoding.UTF8, MimeTypes.ApplicationJson);
            }

            return CallAsync<TResponse>(() => HttpClient.PostAsync(requestUri, body), callerName);
        }

        protected virtual async Task<TResponse> CallAsync<TResponse>(Func<Task<HttpResponseMessage>> requestFunc, [CallerMemberName] string callerName = "")
        {
            try
            {
                var response = await requestFunc();
                var responseContent = await response.Content.ReadAsStringAsync();

                var statusCode = (int)response.StatusCode;
                if (statusCode >= 400)
                {
                    // throw exception with deserialized error
                    var errorResponse = JsonConvert.DeserializeObject<ApiErrorResponse>(responseContent);
                    var message = $"Error when calling '{callerName}'. HTTP status code - {statusCode}. ";
                    if (errorResponse?.Error != null)
                    {
                        message += @$"
                            Error type - '{errorResponse.Error.Type}'.
                            Error code - '{errorResponse.Error.Code}'.
                            Error param - '{errorResponse.Error.Param}'.";
                    }

                    throw new ApiException(statusCode, message, errorResponse);
                }

                return JsonConvert.DeserializeObject<TResponse>(responseContent);
            }
            catch (Exception exception)
            {
                throw new ApiException(500, $"Error when calling '{callerName}'. HTTP status code - 500. {exception.Message}");
            }
        }

        #endregion
    }
}
