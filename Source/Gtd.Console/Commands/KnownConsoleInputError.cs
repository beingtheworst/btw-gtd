using System;

namespace Gtd.Shell.Commands
{
    public sealed class KnownConsoleInputError : Exception
    {
        public KnownConsoleInputError(string message) : base(message)
        {
            
        }
    }
}