using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Threading;

namespace Contextually
{
    /// <summary>
    /// Allows the creation of Info blocks and reading the current context.
    /// </summary>
    public static class Relevant
    {
        private static RelevantInfoContainer RootContainer { get; } = new RelevantInfoContainer();

        private static IDictionary<string, RelevantInfoContainer> NamedContainers { get; } = new Dictionary<string, RelevantInfoContainer>();

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
                    NamedContainers.Add(containerName, container);                    
                }

                return container.Info();
            }
        }

        /// <summary>
        /// Starts a new Info block with a set of values.
        /// </summary>
        /// <param name="info">The values this Info block represents.</param>
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
                    NamedContainers.Add(containerName, container);                    
                }

                return container.Info(info);
            }
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