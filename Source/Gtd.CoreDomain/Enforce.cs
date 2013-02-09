using System;

namespace Gtd.CoreDomain
{
    static class Enforce
    {
        public static void NotEmpty(Guid value, string parameterName)
        {
            if (value == Guid.Empty)
                throw new ArgumentException(string.Format("Parameter {0} can't be empty", parameterName), parameterName);
        }

        public static void NotEmpty(string value, string parameterName)
        {
            if (string.IsNullOrEmpty(value))
                throw new ArgumentException(string.Format("Parameter {0} can't be empty", parameterName), parameterName);
        }
    }
}