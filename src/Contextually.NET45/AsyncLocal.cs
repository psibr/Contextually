using System.Collections.Generic;
using System.Runtime.Remoting.Messaging;

namespace System.Threading
{
    internal static class AsyncLocal
    {
        private const string ContextPropertyNamePrefix = "ctxal"; // short from "Contextually Async Local"

        private static int _nameCounter;

        public static string AllocateName()
        {
            var index = Interlocked.Increment(ref _nameCounter);
            return ContextPropertyNamePrefix + index;
        }
    }

    /// <summary>
    /// Polyfill for AsyncLocal functionality added in NETSTANDARD1.3 and .NET 4.6
    /// </summary>
    /// <typeparam name="T">
    /// The type of the value to encapsulate and follow.
    /// </typeparam>
    internal sealed class AsyncLocal<T>
    {
        private static readonly Dictionary<int, T> _valueMap = new Dictionary<int, T>();
        private static readonly ReaderWriterLockSlim _lock = new ReaderWriterLockSlim();
        private static int _valueCounter;

        private readonly string _name = AsyncLocal.AllocateName();

        private int? CurrentValueId
        {
            get
            {
                return (int?)CallContext.LogicalGetData(_name);
            }
            set
            {
                if (value.HasValue)
                {
                    CallContext.LogicalSetData(_name, value.Value);
                }
                else
                {
                    CallContext.FreeNamedDataSlot(_name);
                }
            }
        }

        /// <summary>
        /// Gets or set the value to flow with <see cref="ExecutionContext"/>.
        /// </summary>
        public T Value
        {
            get
            {
                object value = null;

                int? id = CurrentValueId;
                if (id.HasValue)
                {
                    _lock.EnterReadLock();
                    try
                    {
                        value = _valueMap[id.Value];
                    }
                    finally
                    {
                        _lock.ExitReadLock();
                    }
                }

                return value == null ? default(T) : (T)value;
            }
            set
            {
                int? id = CurrentValueId;
                if (value == null)
                {
                    if (id.HasValue)
                    {
                        _lock.EnterWriteLock();
                        try
                        {
                            _valueMap.Remove(id.Value);
                        }
                        finally
                        {
                            _lock.ExitWriteLock();
                        }
                    }
                    CurrentValueId = null;
                }
                else
                {
                    if (!id.HasValue)
                    {
                        id = Interlocked.Increment(ref _valueCounter);
                        CurrentValueId = id;
                    }

                    _lock.EnterWriteLock();
                    try
                    {
                        _valueMap[id.Value] = value;
                    }
                    finally
                    {
                        _lock.ExitWriteLock();
                    }
                }
            }
        }
    }
}