// Copyright (c) Microsoft Corporation. All rights reserved. See License.txt in the project root for license information.

using System;
using EnterpriseLibrary.Common.Configuration.Fluent;
using EnterpriseLibrary.ExceptionHandling;
using EnterpriseLibrary.ExceptionHandling.Configuration;
using EnterpriseLibrary.Common.Properties;
using System.Globalization;

namespace EnterpriseLibrary.Common.Configuration
{
    /// <summary>
    /// Defines configuration extensions to <see cref="IExceptionConfigurationAddExceptionHandlers"/> for <see cref="ReplaceHandler"/>
    /// configuration.
    /// </summary>
    public static class ReplaceWithHandlerLoggingConfigurationSourceBuilderExtensions
    {
        /// <summary>
        /// Replace exception with new exception type.
        /// </summary>
        /// <typeparam name="T">Replacement <see cref="Exception"/> type.</typeparam>
        /// <returns></returns>
        public static IExceptionConfigurationReplaceWithProvider ReplaceWith<T>(this IExceptionConfigurationAddExceptionHandlers context) where T: Exception
        {
            return ReplaceWith(context, typeof (T));
        }

        /// <summary>
        /// Replace exception with new exception type.
        /// </summary>
        /// <param name="context">Interface to extend to add ReplaceWith options.</param>
        /// <param name="replacingExceptionType">Replacement <see cref="Exception"/> type.</param>
        /// <returns></returns>
        public static IExceptionConfigurationReplaceWithProvider ReplaceWith(this IExceptionConfigurationAddExceptionHandlers context, Type replacingExceptionType)
        {
            if (replacingExceptionType == null) 
                throw new ArgumentNullException("replacingExceptionType");

            if (!typeof(Exception).IsAssignableFrom(replacingExceptionType))
                throw new ArgumentException(string.Format(CultureInfo.CurrentCulture,
                    Resources.ExceptionTypeMustDeriveFromType, typeof(Exception)), "replacingExceptionType");

            return new ExceptionConfigurationReplaceWithBuilder(context, replacingExceptionType);
        }

        private class ExceptionConfigurationReplaceWithBuilder : ExceptionHandlerConfigurationExtension, IExceptionConfigurationReplaceWithProvider
        {
            private readonly ReplaceHandlerData replaceHandlerData;

            public ExceptionConfigurationReplaceWithBuilder(IExceptionConfigurationAddExceptionHandlers context, Type replacingExceptionType) :
                base(context)
            {
                replaceHandlerData = new ReplaceHandlerData()
                                         {
                                             Name = replacingExceptionType.FullName,
                                             ReplaceExceptionType = replacingExceptionType
                                         };

                base.CurrentExceptionTypeData.ExceptionHandlers.Add(replaceHandlerData);
            }


            public IExceptionConfigurationForExceptionTypeOrPostHandling UsingMessage(string message)
            {
                replaceHandlerData.ExceptionMessage = message;
                return this;
            }

            public IExceptionConfigurationForExceptionTypeOrPostHandling UsingResourceMessage(Type resourceType, string resourceName)
            {
                replaceHandlerData.ExceptionMessageResourceType = resourceType.AssemblyQualifiedName;
                replaceHandlerData.ExceptionMessageResourceName = resourceName;
                return this;
            }

            public ReplaceHandlerData GetHandler()
            {
                return replaceHandlerData;
            }
        }
    }
}
