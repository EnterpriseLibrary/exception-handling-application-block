// Copyright (c) Microsoft Corporation. All rights reserved. See License.txt in the project root for license information.

using EnterpriseLibrary.Common.Configuration;
using EnterpriseLibrary.ExceptionHandling.Configuration;
using System;
using System.Collections.Specialized;

namespace EnterpriseLibrary.ExceptionHandling.Bvt.Tests.TestObjects
{
    [ConfigurationElementType(typeof(CustomHandlerData))]
    public class AbsorbingExceptionHandler : IExceptionHandler
    {
        public AbsorbingExceptionHandler()
        {
        }

        public AbsorbingExceptionHandler(NameValueCollection attributes)
            : this()
        {
        }

        public Exception HandleException(Exception exception, Guid correlationID)
        {
            return null;
        }
    }
}
