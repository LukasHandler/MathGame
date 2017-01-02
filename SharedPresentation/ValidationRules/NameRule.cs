//-----------------------------------------------------------------------
// <copyright file="NameRule.cs" company="Lukas Handler">
//     Lukas Handler
// </copyright>
// <summary>
// This file represents the name validation rule.
// </summary>
//-----------------------------------------------------------------------
namespace Shared.SharedPresentation.ValidationRules
{
    using System.Globalization;
    using System.Windows.Controls;

    /// <summary>
    /// This class represents the name validation rule.
    /// </summary>
    /// <seealso cref="System.Windows.Controls.ValidationRule" />
    public class NameRule : ValidationRule
    {
        /// <summary>
        /// Validates the specified value.
        /// </summary>
        /// <param name="value">The value.</param>
        /// <param name="cultureInfo">The culture information.</param>
        /// <returns>True if the value is not empty or null.</returns>
        public override ValidationResult Validate(object value, CultureInfo cultureInfo)
        {
            string serverName = (string)value;

            if (serverName == null || serverName.Trim() == string.Empty)
            {
                return new ValidationResult(false, null);
            }
            else
            {
                return new ValidationResult(true, null);
            }
        }
    }
}
