using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;

namespace Nop.Plugin.Payments.Crypto.Services
{
    /// <summary>
    /// Provides a abstraction to process the web hooks
    /// </summary>
    public interface IWebHookProcessor
    {
        /// <summary>
        /// Processes the web hook request
        /// </summary>
        /// <param name="httpRequest">The HTTP request</param>
        /// <returns>The <see cref="Task"/></returns>
        Task ProcessAsync(HttpRequest httpRequest);
    }
}
