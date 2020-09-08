using System.Threading.Tasks;
using Nop.Plugin.Payments.Crypto.Models;

namespace Nop.Plugin.Payments.Crypto.Services
{
    /// <summary>
    /// Provides an abstraction for factory to create the <see cref="PaymentInfoModel"/>
    /// </summary>
    public interface IPaymentInfoFactory
    {
        /// <summary>
        /// Creates the payment info model
        /// </summary>
        /// <returns>The <see cref="Task"/> containing the payment info model</returns>
        Task<PaymentInfoModel> CreatePaymentInfoAsync();
    }
}
