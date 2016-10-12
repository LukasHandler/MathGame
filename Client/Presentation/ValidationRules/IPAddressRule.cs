using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Controls;

namespace Client.Presentation.ValidationRules
{
    public class IPAddressRule : ValidationRule
    {
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
