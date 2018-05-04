﻿// Copyright (c) Microsoft Corporation. All rights reserved. See License.txt in the project root for license information.

using System;
using System.Collections.Specialized;
using System.Diagnostics;
using EnterpriseLibrary.Common.Configuration.Fluent;
using EnterpriseLibrary.ExceptionHandling.Configuration;

namespace EnterpriseLibrary.Common.Configuration
{
    /// <summary>
    /// Provides a base extensible class for handler configuration extensions.  This class eases the handling 
    /// of the <see cref="IExceptionConfigurationAddExceptionHandlers"/> that is the typical entry point
    /// for most exception handler's fluent configuration interface.
    /// </summary>
    public abstract class ExceptionHandlerConfigurationExtension : IExceptionConfigurationForExceptionTypeOrPostHandling, IExceptionHandlerExtension
    {
        /// <summary>
        /// Initializes a new instance of the ExceptoinHandlerConfigurationExtensions
        /// </summary>
        /// <param name="context">The context for configuration.</param>
        /// <remarks>
        /// This constructor expects to the find the implementor of <paramref name="context"/> provide
        /// the <see cref="IExceptionConfigurationForExceptionTypeOrPostHandling"/> and <see cref="IExceptionHandlerExtension"/> interfaces.
        /// </remarks>
        protected ExceptionHandlerConfigurationExtension(IExceptionConfigurationAddExceptionHandlers context)
        {
            this.Context = (IExceptionConfigurationForExceptionTypeOrPostHandling)context;
            Debug.Assert(typeof (IExceptionHandlerExtension).IsAssignableFrom(context.GetType()));
        }

        /// <summary>
        /// The context for the extending handler in the fluent interface.  The extension interface
        /// is expected to return this context to enable continuation of configuring ExceptionHandling.
        /// </summary>
        protected IExceptionConfigurationForExceptionTypeOrPostHandling Context { get; private set; }


        /// <summary>
        /// The current exception type being built in the fluent interface.  Inheritors genereally should 
        /// add their <see cref="ExceptionHandlerData"/> to this during construction.
        /// </summary>
        public ExceptionTypeData CurrentExceptionTypeData
        {
            get
            {
                return ((IExceptionHandlerExtension)Context).CurrentExceptionTypeData;
            }
        }

        IExceptionConfigurationForExceptionType IExceptionConfigurationThenDoPostHandlingAction.ThenDoNothing()
        {
            return Context.ThenDoNothing();
        }

        IExceptionConfigurationForExceptionType IExceptionConfigurationThenDoPostHandlingAction.ThenNotifyRethrow()
        {
            return Context.ThenNotifyRethrow();
        }

        IExceptionConfigurationForExceptionType IExceptionConfigurationThenDoPostHandlingAction.ThenThrowNewException()
        {
            return Context.ThenThrowNewException();
        }

        IExceptionConfigurationForExceptionType IExceptionConfigurationGivenPolicyWithName.GivenPolicyWithName(string name)
        {
            return Context.GivenPolicyWithName(name);
        }

    }
}
