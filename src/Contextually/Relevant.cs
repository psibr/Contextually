using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Collections.Concurrent;
using System.Threading;

namespace Contextually
{
    /// <summary>
    /// Allows the creation of Info blocks and reading the current context.
    /// </summary>
    public static class Relevant
    {
        private static RelevantInfoContainer RootContainer { get; } = new RelevantInfoContainer();

        private static ConcurrentDictionary<string, RelevantInfoContainer> NamedContainers { get; } 
            = new ConcurrentDictionary<string, RelevantInfoContainer>();

        /// <summary>
        /// Retrieve the full set of values from all enclosing Info blocks.
        /// </summary>
        /// <returns>Full set of values from all enclosing Info blocks.</returns>
        public static NameValueCollection Info() => Info(containerName: null);

        /// <summary>
        /// Retrieve the full set of values from all enclosing Info blocks.
        /// </summary>
        /// <returns>Full set of values from all enclosing Info blocks.</returns>
        public static NameValueCollection Info(string containerName = null)
        {
            if(containerName == null)
                return RootContainer.Info();
            else
            {
                if(!NamedContainers.TryGetValue(containerName, out var container))
                {
                    container = new RelevantInfoContainer();

                    if (!NamedContainers.TryAdd(containerName, container))
                        container = NamedContainers[containerName];               
                }

                return container.Info();
            }
        }

        /// <summary>
        /// Starts a new Info block with a set of values.
        /// </summary>
        /// <param name="info">The values this Info block represents.</param>
        /// <param name="containerName">Name of the container to write into.</param>
        /// <returns>An <see cref="IDisposable"/> object for use with a using block.</returns>
        public static IDisposable Info(NameValueCollection info, string containerName = null)
        {
            if(containerName == null)
                return RootContainer.Info(info);
            else
            {
                if(!NamedContainers.TryGetValue(containerName, out var container))
                {
                    container = new RelevantInfoContainer();

                    if (!NamedContainers.TryAdd(containerName, container))
                        container = NamedContainers[containerName];
                }

                return container.Info(info);
            }
        }

        /// <summary>
        /// Starts a new Info block with a set of values.
        /// </summary>
        /// <param name="name">The name of a value.</param>
        /// <param name="value">The value.</param>
        /// <returns>An <see cref="IDisposable"/> object for use with a using block.</returns>
        public static IDisposable Info(string name, string value) =>
            Info(new NameValueCollection(1)
            {
                [name] = value
            });

        /// <summary>
        /// Starts a new Info block with a set of values.
        /// </summary>
        /// <param name="tuple">The name-value tuple this Info block represents.</param>
        /// <returns>An <see cref="IDisposable"/> object for use with a using block.</returns>
        public static IDisposable Info((string name, string value) tuple) =>
            Info(new NameValueCollection(1)
            {
                [tuple.name] = tuple.value
            });

        /// <summary>
        /// Starts a new Info block with a set of values.
        /// </summary>
        /// <param name="tuple1">A name-value tuple this Info block represents.</param>
        /// <param name="tuple2">A name-value tuple this Info block represents.</param>
        /// <returns>An <see cref="IDisposable"/> object for use with a using block.</returns>
        public static IDisposable Info(
            (string name, string value) tuple1,
            (string name, string value) tuple2) =>
            Info(new NameValueCollection(2)
            {
                [tuple1.name] = tuple1.value,
                [tuple2.name] = tuple2.value
            });

        /// <summary>
        /// Starts a new Info block with a set of values.
        /// </summary>
        /// <param name="tuple1">A name-value tuple this Info block represents.</param>
        /// <param name="tuple2">A name-value tuple this Info block represents.</param>
        /// <param name="tuple3">A name-value tuple this Info block represents.</param>
        /// <returns>An <see cref="IDisposable"/> object for use with a using block.</returns>
        public static IDisposable Info(
            (string name, string value) tuple1,
            (string name, string value) tuple2,
            (string name, string value) tuple3) =>
            Info(new NameValueCollection(3)
            {
                [tuple1.name] = tuple1.value,
                [tuple2.name] = tuple2.value,
                [tuple3.name] = tuple3.value,
            });

        /// <summary>
        /// Starts a new Info block with a set of values.
        /// </summary>
        /// <param name="tuple1">A name-value tuple this Info block represents.</param>
        /// <param name="tuple2">A name-value tuple this Info block represents.</param>
        /// <param name="tuple3">A name-value tuple this Info block represents.</param>
        /// <param name="tuple4">A name-value tuple this Info block represents.</param>
        /// <returns>An <see cref="IDisposable"/> object for use with a using block.</returns>
        public static IDisposable Info(
            (string name, string value) tuple1,
            (string name, string value) tuple2,
            (string name, string value) tuple3,
            (string name, string value) tuple4) =>
            Info(new NameValueCollection(4)
            {
                [tuple1.name] = tuple1.value,
                [tuple2.name] = tuple2.value,
                [tuple3.name] = tuple3.value,
                [tuple4.name] = tuple4.value
            });

        /// <summary>
        /// Starts a new Info block with a set of values.
        /// </summary>
        /// <param name="tuples">The collection of name-value tuples this Info block represents.</param>
        /// <returns>An <see cref="IDisposable"/> object for use with a using block.</returns>
        public static IDisposable Info(params (string name, string value)[] tuples)
        {
            var nameValueCollection = new NameValueCollection(tuples.Length);
            foreach (var tuple in tuples)
                nameValueCollection.Add(tuple.name, tuple.value);
            return Info(nameValueCollection);
        }
    }

    /// <summary>
    /// Allows the creation of Info blocks and reading the current context. 
    /// Used for including context in libraries without exposing publicly.
    /// </summary>
    public class RelevantInfoContainer
    {
        /// <summary>
        /// Retrieve the full set of values from all enclosing Info blocks.
        /// </summary>
        /// <returns>Full set of values from all enclosing Info blocks.</returns>
        public NameValueCollection Info()
        {
            var current = Head.Value;
            var info = new NameValueCollection();

            while(current != null)
            {
                info.Add(current.CurrentInfo);

                current = current.ParentBlock;
            }

            return info;
        }

        /// <summary>
        /// The head node which is an Info block (may point to parent blocks).
        /// </summary>
        internal AsyncLocal<InfoBlock> Head { get; } = new AsyncLocal<InfoBlock>();

        /// <summary>
        /// Starts a new Info block with a set of values.
        /// </summary>
        /// <param name="info">The values this Info block represents.</param>
        /// <returns>An <see cref="IDisposable"/> object for use with a using block.</returns>
        public IDisposable Info(NameValueCollection info)
        {
            if(Head.Value == null)
                Head.Value = new InfoBlock(this, info);
            else
                Head.Value = new InfoBlock(Head.Value, info);

            return Head.Value;
        }
    }
}