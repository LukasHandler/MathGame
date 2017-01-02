//-----------------------------------------------------------------------
// <copyright file="BooleanToVisibilityConverter.cs" company="Lukas Handler">
//     Lukas Handler
// </copyright>
// <summary>
// This file represents the boolean to visibility converter.
// </summary>
//-----------------------------------------------------------------------
namespace Shared.SharedPresentation.Converters
{
    using System;
    using System.Windows;
    using System.Windows.Data;

    /// <summary>
    /// This class represents the boolean to visibility converter.
    /// </summary>
    /// <seealso cref="System.Windows.Data.IValueConverter" />
    public class BooleanToVisibilityConverter : IValueConverter
    {
        /// <summary>
        /// Converts the specified value.
        /// </summary>
        /// <param name="value">The value.</param>
        /// <param name="targetType">Type of the target.</param>
        /// <param name="parameter">The parameter.</param>
        /// <param name="culture">The culture.</param>
        /// <returns>The converted value.</returns>
        public object Convert(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {
            if (value is bool && (bool)value)
            {
                return Visibility.Visible;
            }

            return Visibility.Hidden;
        }

        /// <summary>
        /// Converts the visibility to boolean back.
        /// </summary>
        /// <param name="value">The value.</param>
        /// <param name="targetType">Type of the target.</param>
        /// <param name="parameter">The parameter.</param>
        /// <param name="culture">The culture.</param>
        /// <returns>The converted value.</returns>
        public object ConvertBack(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {
            if (value is Visibility && (Visibility)value == Visibility.Visible)
            {
                return true;
            }

            return false;
        }
    }
}