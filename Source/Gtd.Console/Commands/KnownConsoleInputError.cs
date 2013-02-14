using System;

namespace Gtd.Shell.Commands
{
    public sealed class KnownConsoleInputError : Exception
    {
        public KnownConsoleInputError(string message) : base(message)
        {
            
        }

        public KnownConsoleInputError(string format, params object[] args) : base (string.Format(format,args))
        {
            
        }
    }
}