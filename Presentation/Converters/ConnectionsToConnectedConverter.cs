//-----------------------------------------------------------------------
// <copyright file="ConnectionsToConnectedConverter.cs" company="Lukas Handler">
//     Lukas Handler
// </copyright>
// <summary>
// This file represents the connection count to connected converter.
// </summary>
//-----------------------------------------------------------------------
namespace Presentation.Converters
{
    using System;
    using System.Globalization;
    using System.Windows.Data;

    /// <summary>
    /// This class represents the connection count to connected converter.
    /// </summary>
    /// <seealso cref="System.Windows.Data.IValueConverter" />
    public class ConnectionsToConnectedConverter : IValueConverter
    {
        /// <summary>
        /// Converts the specified value.
        /// </summary>
        /// <param name="value">The value.</param>
        /// <param name="targetType">Type of the target.</param>
        /// <param name="parameter">The parameter.</param>
        /// <param name="culture">The culture.</param>
        /// <returns>True or false depending on the connection count.</returns>
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (value == null || (string)value == string.Empty || System.Convert.ToInt32((string)value) == 0)
            {
                return true;
            }
            else
            {
                return false;
            }
        }

        /// <summary>
        /// Converts back, not used in the project.
        /// </summary>
        /// <param name="value">The value.</param>
        /// <param name="targetType">Type of the target.</param>
        /// <param name="parameter">The parameter.</param>
        /// <param name="culture">The culture.</param>
        /// <returns>An exception.</returns>
        /// <exception cref="System.NotImplementedException">Throw default exception when called.</exception>
        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}
