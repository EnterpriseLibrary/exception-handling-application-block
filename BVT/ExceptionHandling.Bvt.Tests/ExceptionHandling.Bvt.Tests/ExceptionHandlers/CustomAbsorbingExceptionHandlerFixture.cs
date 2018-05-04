﻿// Copyright (c) Microsoft Corporation. All rights reserved. See License.txt in the project root for license information.

using EnterpriseLibrary.Common.Configuration;
using EnterpriseLibrary.ExceptionHandling.Bvt.Tests.TestUtilities;
using EnterpriseLibrary.ExceptionHandling.Configuration;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.Data;

namespace EnterpriseLibrary.ExceptionHandling.Bvt.Tests.ExceptionHandlers
{
    [TestClass]
    public class CustomAbsorbingExceptionHandlerFixture
    {
        private IConfigurationSource configurationSource;
        private ExceptionManager exceptionManager;

        [TestInitialize]
        public void Initialize()
        {
            this.configurationSource = new FileConfigurationSource(@"configurations\\ExceptionHandlers.config");

            this.exceptionManager = ((ExceptionHandlingSettings)this.configurationSource.GetSection("exceptionHandling")).BuildExceptionManager();
        }

        [TestCleanup]
        public void Cleanup()
        {
            ExceptionPolicy.Reset();

            if (this.configurationSource != null)
            {
                this.configurationSource.Dispose();
            }
        }

        [TestMethod]
        public void ThrowsExceptionHandlingExceptionWhenPostHandlingIsThrowNewExceptionAndExceptionShouldBeHandled()
        {
            var originalException = new DBConcurrencyException();

            ExceptionAssertHelper.Throws<ExceptionHandlingException>(() =>
                this.exceptionManager.HandleException(originalException, "Absorbing DBConcurrencyException, ThrowNewException postHandling"));
        }

        [TestMethod]
        public void ReturnsExceptionHandlingExceptionInOutParameterAndRecommendsRethrowingWhenPostHandlingIsThrowNewExceptionAndExceptionShouldBeHandled()
        {
            Exception exceptionToThrow;
            bool rethrowRecommended = this.exceptionManager.HandleException(
                new DBConcurrencyException(),
                "Absorbing DBConcurrencyException, ThrowNewException postHandling",
                out exceptionToThrow);

            Assert.IsTrue(rethrowRecommended);
            Assert.IsNotNull(exceptionToThrow);
            Assert.IsTrue(typeof(ExceptionHandlingException).IsAssignableFrom(exceptionToThrow.GetType()));
        }

        [TestMethod]
        public void DoesNotThrowWhenPostHandlingIsNotifyRethrowAndExceptionShouldBeHandled()
        {
            var originalException = new DBConcurrencyException();

            ExceptionAssertHelper.DoesNotThrow(() =>
                this.exceptionManager.HandleException(originalException, "Absorbing DBConcurrencyException, NotifyRethrow postHandling"));
        }

        [TestMethod]
        public void ReturnsNullInOutParameterAndRecommendsRethrowingWhenPostHandlingIsNotifyRethrowAndExceptionShouldBeHandled()
        {
            Exception exceptionToThrow;
            bool rethrowRecommended = this.exceptionManager.HandleException(
                new DBConcurrencyException(),
                "Absorbing DBConcurrencyException, NotifyRethrow postHandling",
                out exceptionToThrow);

            Assert.IsTrue(rethrowRecommended);
            Assert.IsNull(exceptionToThrow);
        }
    }
}
