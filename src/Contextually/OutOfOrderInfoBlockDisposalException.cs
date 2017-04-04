using System;

namespace Contextually
{
    /// <summary>
    /// Indicates an InfoBlock was disposed in an improper order.
    /// </summary>
    public class OutOfOrderInfoBlockDisposalException : Exception
    {
        internal  OutOfOrderInfoBlockDisposalException() : this(DefaultMessage) { }
        internal OutOfOrderInfoBlockDisposalException(string message) : base(message) { }
        internal OutOfOrderInfoBlockDisposalException(string message, Exception inner) : base(message, inner) { }

        private static string DefaultMessage =>
            "Disposing out of order will cause context to \"bleed\" " +
            "from the current scope. Consider this a fatal exception.";
    }
}
