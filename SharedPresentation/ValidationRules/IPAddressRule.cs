//-----------------------------------------------------------------------
// <copyright file="IPAddressRule.cs" company="Lukas Handler">
//     Lukas Handler
// </copyright>
// <summary>
// This file represents the IP address validation rule.
// </summary>
//-----------------------------------------------------------------------
namespace Shared.SharedPresentation.ValidationRules
{
    using System.Globalization;
    using System.Net;
    using System.Windows.Controls;

    /// <summary>
    /// This class represents the IP address validation rule.
    /// </summary>
    /// <seealso cref="System.Windows.Controls.ValidationRule" />
    public class IPAddressRule : ValidationRule
    {
        /// <summary>
        /// Validates the specified value.
        /// </summary>
        /// <param name="value">The value.</param>
        /// <param name="cultureInfo">The culture information.</param>
        /// <returns>True if the value can be converted to an IP address.</returns>
        public override ValidationResult Validate(object value, CultureInfo cultureInfo)
        {
            IPAddress notUsedResult = null;
            if (IPAddress.TryParse((string)value, out notUsedResult))
            {
                return new ValidationResult(true, null);
            }
            else
            {
                return new ValidationResult(false, null);
            }
        }
    }
}
