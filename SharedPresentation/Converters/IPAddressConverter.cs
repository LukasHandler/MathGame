//-----------------------------------------------------------------------
// <copyright file="IPAddressConverter.cs" company="Lukas Handler">
//     Lukas Handler
// </copyright>
// <summary>
// This file represents the IP address to string converter.
// </summary>
//-----------------------------------------------------------------------
namespace Shared.SharedPresentation.Converters
{
    using System;
    using System.Globalization;
    using System.Net;
    using System.Windows.Data;

    /// <summary>
    /// This class represents the IP address to string converter.
    /// </summary>
    /// <seealso cref="System.Windows.Data.IValueConverter" />
    public class IPAddressConverter : IValueConverter
    {
        /// <summary>
        /// Converts the specified value.
        /// </summary>
        /// <param name="value">The value.</param>
        /// <param name="targetType">Type of the target.</param>
        /// <param name="parameter">The parameter.</param>
        /// <param name="culture">The culture.</param>
        /// <returns>The converted value.</returns>
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (value != null)
            {
                return System.Convert.ToString(value);
            }
            else
            {
                return null;
            }
        }

        /// <summary>
        /// Converts the string back to an IP address.
        /// </summary>
        /// <param name="value">The value.</param>
        /// <param name="targetType">Type of the target.</param>
        /// <param name="parameter">The parameter.</param>
        /// <param name="culture">The culture.</param>
        /// <returns>The IP address.</returns>
        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            return IPAddress.Parse((string)value);
        }
    }
}
