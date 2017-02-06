using System;
using System.Collections.Specialized;

namespace Contextually
{
    /// <summary>
    /// Component that uses the IDisposable pattern 
    /// to add possibly nested contextual information, similar to a method stack.
    /// </summary>
    internal class InfoBlock : IDisposable
    {
        /// <summary>
        /// Create a new Info block with new values and potentially a parent block.
        /// </summary>
        /// <param name="parent">Optional parent block.</param>
        /// <param name="info">Required values for this block.</param>
        internal InfoBlock(InfoBlock parent, NameValueCollection info)
        {
            ParentBlock = parent;

            CurrentInfo = info;
        }

        /// <summary>
        /// A reference to the parent enclosing Info block.
        /// </summary>
        internal InfoBlock ParentBlock { get; private set; }

        /// <summary>
        /// The values assigned in the nearest enclosing Info block.
        /// </summary>
        internal NameValueCollection CurrentInfo { get; private set; }

        /// <summary>
        /// Disposes of the context and sets the active context to the parent, if any.
        /// </summary>
        public void Dispose()
        {
            // Dump our reference, since we don't need it. Optimizing for GC.
            CurrentInfo = null;

            // Set the head of the context linked-list to the parent of this.
            Relevant.Head.Value = ParentBlock;

            // Dump our reference, since we don't need it. Optimizing for GC.
            ParentBlock = null;
        }
    }
}


