// Copyright (c) Microsoft Corporation. All rights reserved. See License.txt in the project root for license information.

using System;
using System.Collections.Generic;
using System.Configuration;
using System.Globalization;
using System.Threading;
using Microsoft.Practices.EnterpriseLibrary.ExceptionHandling.Configuration;
using Microsoft.Practices.EnterpriseLibrary.ExceptionHandling.Tests.Properties;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace Microsoft.Practices.EnterpriseLibrary.ExceptionHandling.Tests
{
    [TestClass]
    public class WrapHandlerFixture
    {
        const string message = "message";

        [TestInitialize]
        public void Initialize()
        {
            var handler = new WrapHandler(Resources.ExceptionMessage, typeof(ApplicationException));

            var entry = new ExceptionPolicyEntry(
                typeof(Exception),
                PostHandlingAction.None,
                new IExceptionHandler[] { handler }
            );

            var policyDefinition = new ExceptionPolicyDefinition(
                "LocalizedWrapPolicy",
                new List<ExceptionPolicyEntry> { entry }
            );

            var manager = new ExceptionManager(policyDefinition); 

            ExceptionPolicy.SetExceptionManager(manager, false);
        }

        [TestCleanup]
        public void Cleanup()
        {
            ExceptionPolicy.Reset();
        }

        [TestMethod]
        [ExpectedException(typeof(ArgumentException))]
        public void HandlerThrowsWhenNotWrappingAnException()
        {
            WrapHandler handler = new WrapHandler(message, typeof(object));
            handler.HandleException(new ApplicationException(), Guid.NewGuid());
        }

        [TestMethod]
        [ExpectedException(typeof(ArgumentNullException))]
        public void ConstructingWithNullExceptionTypeThrows()
        {
            WrapHandler handler = new WrapHandler(message, null);
        }

        [TestMethod]
        public void CanWrapException()
        {
            WrapHandler handler = new WrapHandler(message, typeof(ApplicationException));
            Exception ex = handler.HandleException(new InvalidOperationException(), Guid.NewGuid());

            Assert.AreEqual(typeof(ApplicationException), ex.GetType());
            Assert.AreEqual(typeof(ApplicationException), handler.WrapExceptionType);
            Assert.AreEqual(message, ex.Message);
            Assert.AreEqual(typeof(InvalidOperationException), ex.InnerException.GetType());
        }

        [TestMethod]
        public void WrapExceptionReturnsLocalizedMessage()
        {
            Exception exceptionToWrap = new Exception();
            Exception thrownException;
            ExceptionPolicy.HandleException(exceptionToWrap, "LocalizedWrapPolicy", out thrownException);

            Assert.AreEqual(Resources.ExceptionMessage, thrownException.Message);
        }

        [TestMethod]
        public void WrapExceptionReturnsLocalizedMessageBasedOnCurentUICulture()
        {
            CultureInfo originalCultureInfo = Thread.CurrentThread.CurrentUICulture;
            try
            {
                Thread.CurrentThread.CurrentUICulture = new CultureInfo("nl-nl");

                Exception exceptionToWrap = new Exception();
                Exception thrownException;
                ExceptionPolicy.HandleException(exceptionToWrap, "LocalizedWrapPolicy", out thrownException);

                Assert.AreEqual(Resources.ExceptionMessage, thrownException.Message);
            }
            finally
            {
                Thread.CurrentThread.CurrentUICulture = originalCultureInfo;
            }
        }
    }
}
