using System;

namespace Gtd.Pcl.CoreDomain
{
    static class Enforce
    {
        public static void NotEmpty(Guid value, string parameterName)
        {
            if (value == Guid.Empty)
                throw new ArgumentException(string.Format("Guid {0} can't be empty", parameterName), parameterName);
        }

        public static void NotEmpty(string value, string parameterName)
        {
            if (string.IsNullOrEmpty(value))
                throw new ArgumentException(string.Format("String parameter {0} can't be empty", parameterName), parameterName);
        }

        public static void NotEmpty(ProjectId id, string parameterName)
        {
            if (id.IsEmpty)
                throw new ArgumentException(string.Format("ProjectId {0} can't be empty", parameterName), parameterName);
        }
    }
}
