using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Controls;

namespace Shared.SharedPresentation.ValidationRules
{
    public class NameRule : ValidationRule
    {
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
