using Microsoft.AspNetCore.Mvc;
using Nop.Plugin.Payments.Crypto.Models;
using Nop.Plugin.Payments.Crypto.Services;
using Nop.Web.Framework.Controllers;

namespace Nop.Plugin.Payments.Crypto.Controllers
{
    public class CryptoWebHookController : BasePluginController
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
