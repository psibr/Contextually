using System.Collections;
using System.Runtime.Remoting;
using System.Runtime.Remoting.Messaging;
using Contextually.Reflection;

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
        private readonly string _name = Guid.NewGuid().ToString("N");

        /// <summary>
        /// Gets or set the value to flow with <see cref="ExecutionContext"/>.
        /// </summary>
        public T Value
        {
            get
            {
                if (CallContext.LogicalGetData(_name) is ObjectHandle slot)
                    return (T)slot.Unwrap();
                return default(T);
            }
            set
            {
                // Mimic the implementation of AsyncLocal<T>
                var executionContext = Thread.CurrentThread.GetMutableExecutionContext();
                var logicalCallContext = executionContext.GetLogicalCallContext();
                var datastore = logicalCallContext.GetDatastore();
                var datastoreCopy = datastore == null ? new Hashtable() : new Hashtable(datastore);
                var slot = new ObjectHandle(value);
                datastoreCopy[_name] = slot;
                logicalCallContext.SetDatastore(datastoreCopy);
            }
        }
    }
}