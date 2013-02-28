using System;
using System.Globalization;

namespace Gtd.Shell
{
    public static class FriendlyDateParser
    {
        public delegate DateTime Change(float diff, DateTime source);

        static bool TryRepresent(string value, string[] suffix, Change producer, out DateTime result)
        {
            result = DateTime.MinValue;
            foreach (var s in suffix)
            {
                if (!value.EndsWith(s, StringComparison.InvariantCultureIgnoreCase))
                    continue;
                var trimmed = value.Remove(value.Length - s.Length, s.Length).TrimEnd();
                float res;

                if (!float.TryParse(trimmed, NumberStyles.Float | NumberStyles.AllowThousands, CultureInfo.InvariantCulture, out res))
                {
                    continue;
                }
                result =producer(res, DateTime.Now);
                return true;
            }
            return false;
        }

        public static bool TryParseDate(string value, out DateTime span)
        {
            span = DateTime.MinValue;
            if (string.IsNullOrWhiteSpace(value))
                return false;

            value = value.Trim().ToLowerInvariant();

            switch (value)
            {
                case "now":
                    span = DateTime.UtcNow;
                    return true;
                case "today":
                    span = DateTime.UtcNow.Date.AddHours(8);
                    return true;
                case "tomorrow":
                    span = DateTime.UtcNow.Date.AddHours(8).AddDays(1);
                    return true;
            }

            try
            {
                if (TryRepresent(value, new[] {"w", "wk", "week"}, (diff, source) => source.AddDays(7 * diff) , out span))
                    return true;
                if (TryRepresent(value, new[] {"d", "day", "days"},(diff, source) => source.AddDays(diff), out span))
                    return true;
                if (TryRepresent(value, new[] {"m", "mth", "month"}, (diff, source) => source.AddMonths((int)diff),out span)) ;

                

            }
            catch (Exception ex)
            {
                
             
            }
            return false;

        }
    }
}