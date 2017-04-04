using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Remoting.Messaging;
using System.Text;
using System.Threading.Tasks;
using System.Threading;

namespace System.Threading
{
    /// <summary>
    /// Polyfill for AsyncLocal functionality added in NETSTANDARD1.3 and .NET 4.6
    /// </summary>
    /// <typeparam name="T">
    /// The type of the value to encapsulate and follow.
    /// </typeparam>
    internal class AsyncLocal<T>
    {
        private string UniqueDataItemKey { get; }

        public AsyncLocal()
        {
            UniqueDataItemKey = Guid.NewGuid().ToString();
        }

        /// <summary>
        /// Gets or set the value to flow with <see cref="ExecutionContext"/>.
        /// </summary>
        public T Value
        {
            get
            {
                return (T)CallContext.LogicalGetData(UniqueDataItemKey);
            }
            set
            {
                CallContext.LogicalSetData(UniqueDataItemKey, value);
            }
        }

    }
}