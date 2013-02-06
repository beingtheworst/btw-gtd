using System;
using System.Globalization;

namespace Gtd.Shell.LesserEvils
{
    /// <summary>Pretty format utils</summary>
    public static class FormatEvil
    {
        public static string TwitterOffestUtc(DateTime dateInUtc)
        {
            if (dateInUtc.Year == 1)
                return "";
            var now = DateTime.UtcNow;

            if (now.Year != dateInUtc.Year)
            {
                return dateInUtc.ToString("dd MMM yyyy", CultureInfo.InvariantCulture);
            }

            var offset = now - dateInUtc;
            double delta = offset.TotalSeconds;
            var abs = Math.Abs(delta);
            var sign = delta > 0 ? "" : " from now";
            if (abs <= 2)
                return "now";

            // less than min
            if (abs < 60)
            {
                return Math.Round(abs) + "s" + sign;
            }

            // less than hour
            if (abs < 60 * 60)
            {
                return Math.Round(abs / 60) + "m" + sign;
            }
            // less than a day
            if (abs < 60 * 60 * 24)
            {
                return Math.Round(abs / 60 / 60) + "h" + sign;
            }

            return dateInUtc.ToString("dd MMM", CultureInfo.InvariantCulture);
        }
    }
}