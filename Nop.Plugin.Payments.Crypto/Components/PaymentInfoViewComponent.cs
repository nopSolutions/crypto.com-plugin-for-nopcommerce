using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Nop.Plugin.Payments.Crypto.Services;
using Nop.Web.Framework.Components;

namespace Nop.Plugin.Payments.Crypto.Components
{
    /// <summary>
    /// Represents a view component to display payment info in public store
    /// </summary>
    [ViewComponent(Name = Defaults.PAYMENT_INFO_VIEW_COMPONENT_NAME)]
    public class PaymentInfoViewComponent : NopViewComponent
    {
        #region Fields

        private readonly IPaymentInfoFactory _paymentInfoFactory;

        #endregion

        #region Ctor

        public PaymentInfoViewComponent(IPaymentInfoFactory paymentInfoFactory)
        {
            _paymentInfoFactory = paymentInfoFactory;
        }

        #endregion

        #region Methods

        public async Task<IViewComponentResult> InvokeAsync()
        {
            var model = await _paymentInfoFactory.CreatePaymentInfoAsync();
            return View("~/Plugins/Payments.Crypto/Views/PaymentInfo.cshtml", model);
        }

        #endregion
    }
}
