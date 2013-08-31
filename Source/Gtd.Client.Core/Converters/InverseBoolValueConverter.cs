using System;
using Cirrious.CrossCore.Converters;

namespace Gtd.Client.Core.Converters
{
    // converts from a bool to a bool, basically flip bool to OPPOSITE of its current value
    public class InverseBoolValueConverter : MvxValueConverter<bool, bool>
    {
        protected override bool Convert(bool value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {
            return !value;
        }
    }
}

