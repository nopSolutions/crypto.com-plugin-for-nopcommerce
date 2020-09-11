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
        public IActionResult Handle()
        {
            _webHookProcessor.ProcessAsync(HttpContext.Request);
            return Ok();
        }

        #endregion
    }
}
