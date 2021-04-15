using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Nop.Plugin.Payments.Crypto.Services;

namespace Nop.Plugin.Payments.Crypto.Controllers
{
    public class CryptoWebHookController : Controller
    {
        #region Fields

        private readonly IWebHookProcessor _webHookProcessor;

        #endregion

        #region Ctor

        public CryptoWebHookController(IWebHookProcessor webHookProcessor)
        {
            _webHookProcessor = webHookProcessor;
        }

        #endregion

        #region Methods

        [HttpPost]
        public async Task<IActionResult> Handle()
        {
            await _webHookProcessor.ProcessAsync(HttpContext.Request);
            return Ok();
        }

        #endregion
    }
}
