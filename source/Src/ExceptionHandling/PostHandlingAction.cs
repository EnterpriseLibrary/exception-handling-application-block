// Copyright (c) Microsoft Corporation. All rights reserved. See License.txt in the project root for license information.

namespace Microsoft.Practices.EnterpriseLibrary.ExceptionHandling
{
    /// <summary>
    /// Determines what action should occur after an exception is handled by the configured exception handling chain. 
    /// </summary>
    public enum PostHandlingAction
    {
        /// <summary>
        /// Indicates that no rethrow should occur.
        /// </summary>
        None = 0,
        /// <summary>
        /// Notify the caller that a rethrow is recommended.
        /// </summary>
        NotifyRethrow = 1,
        /// <summary>
        /// Throws the exception after the exception has been handled by all handlers in the chain.
        /// </summary>
        ThrowNewException = 2
    }
}
