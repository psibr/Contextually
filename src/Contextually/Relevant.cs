using System;
using System.Collections.Specialized;
using System.Threading;

namespace Contextually
{
    /// <summary>
    /// Allows the creation of Info blocks and reading the current context.
    /// </summary>
    public static class Relevant
    {
        /// <summary>
        /// Retrieve the full set of values from all enclosing Info blocks.
        /// </summary>
        /// <returns>Full set of values from all enclosing Info blocks.</returns>
        public static NameValueCollection Info()
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
        internal static AsyncLocal<InfoBlock> Head { get; } = new AsyncLocal<InfoBlock>();

        /// <summary>
        /// Starts a new Info block with a set of values.
        /// </summary>
        /// <param name="info">The values this Info block represents.</param>
        /// <returns>An <see cref="IDisposable"/> object for use with a using block.</returns>
        public static IDisposable Info(NameValueCollection info)
        {
            Head.Value = new InfoBlock(Head.Value, info);

            return Head.Value;
        }
    }
}