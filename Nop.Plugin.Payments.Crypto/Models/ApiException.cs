using System;

namespace Nop.Plugin.Payments.Crypto.Models
{
    /// <summary>
    /// Represents a API exception
    /// </summary>
    public class ApiException : Exception
    {
        #region Properties

        /// <summary>
        /// Gets or sets the error code (HTTP status code)
        /// </summary>
        /// <value>The error code (HTTP status code)</value>
        public int ErrorCode { get; set; }

        /// <summary>
        /// Gets or sets the error content.
        /// </summary>
        /// <value>The error content (Http response body).</value>
        public ApiErrorResponse ErrorContent { get; private set; }

        #endregion

        #region Ctor

        /// <summary>
        /// Initializes a new instance of the <see cref="ApiException"/> class.
        /// </summary>
        /// <param name="errorCode">The HTTP status code.</param>
        /// <param name="message">The error message.</param>
        public ApiException(int errorCode, string message)
            : base(message)
        {
            ErrorCode = errorCode;
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="ApiException"/> class.
        /// </summary>
        /// <param name="errorCode">The HTTP status code.</param>
        /// <param name="message">The error message.</param>
        /// <param name="errorContent">The error content.</param>
        public ApiException(int errorCode, string message, ApiErrorResponse errorContent = null)
            : this(errorCode, message)
        {
            ErrorContent = errorContent;
        }

        #endregion
    }
}
