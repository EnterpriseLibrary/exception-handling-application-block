// Copyright (c) Microsoft Corporation. All rights reserved. See License.txt in the project root for license information.

using System;
using System.Configuration;
using System.IO;
using Microsoft.Practices.EnterpriseLibrary.Common.Configuration;
using Microsoft.Practices.EnterpriseLibrary.ExceptionHandling.Configuration;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace Microsoft.Practices.EnterpriseLibrary.ExceptionHandling.Tests
{
    [TestClass]
    public class ExceptionHandlingSettingsFixture
    {
        const string badString = "SomeBunkString984t487y";
        const string wrapPolicy = "Wrap Policy";
        const string wrapHandler = "WrapHandler";
        const string newWrapHandler = "WrapHandler2";
        const string customPolicy = "Custom Policy";
        const string exceptionType = "Exception";
        const string customHandler = "CustomHandler";

        ExceptionPolicyData WrapPolicy
        {
            get
            {
                string configPath = Path.Combine(AppContext.BaseDirectory, "Microsoft.Practices.EnterpriseLibrary.ExceptionHandling.Tests.dll.config");

                var configSource = new FileConfigurationSource(configPath);
                var policyFactory = new ExceptionPolicyFactory(configSource);
                ExceptionPolicy.SetExceptionManager(policyFactory.CreateManager(), false);

                ExceptionHandlingSettings settings = (ExceptionHandlingSettings)
                    configSource.GetSection(ExceptionHandlingSettings.SectionName);
                
                return settings.ExceptionPolicies.Get(wrapPolicy);
            }
        }

        [TestMethod]
        public void GetPolicyByNamePassTest()
        {
            ExceptionPolicyData testPolicy = WrapPolicy;
            Assert.IsNotNull(testPolicy);
        }

        [TestMethod]
        public void GetPolicyByNameFailTest()
        {
            string configPath = Path.Combine(AppContext.BaseDirectory, "Microsoft.Practices.EnterpriseLibrary.ExceptionHandling.Tests.dll.config");

            var configSource = new FileConfigurationSource(configPath);
            var policyFactory = new ExceptionPolicyFactory(configSource);
            ExceptionPolicy.SetExceptionManager(policyFactory.CreateManager(), false);

            ExceptionHandlingSettings settings = (ExceptionHandlingSettings)
                configSource.GetSection(ExceptionHandlingSettings.SectionName);
            settings.ExceptionPolicies.Get(badString);
        }

        [TestMethod]
        public void GetTypeByNamePassTest()
        {
            ExceptionTypeData testType = WrapPolicy.ExceptionTypes.Get(exceptionType);
            Assert.IsNotNull(testType);
            Assert.AreEqual(PostHandlingAction.ThrowNewException, testType.PostHandlingAction);
        }

        [TestMethod]
        public void GetTypeByNameFailTest()
        {
            ExceptionTypeData testType = WrapPolicy.ExceptionTypes.Get(badString);
            Assert.IsNull(testType);
        }

        [TestMethod]
        public void GetHandlerPassTest()
        {
            ExceptionTypeData testType = WrapPolicy.ExceptionTypes.Get(exceptionType);
            Assert.IsNotNull(testType);
            Assert.AreEqual(1, testType.ExceptionHandlers.Count);
            Assert.IsNotNull(testType.ExceptionHandlers.Get(wrapHandler));
        }

        [TestMethod]
        public void CanOpenAndSaveWithCustomHandler()
        {
            System.Configuration.Configuration config = ConfigurationManager.OpenExeConfiguration(ConfigurationUserLevel.None);
            ExceptionHandlingSettings settings = (ExceptionHandlingSettings)config.Sections[ExceptionHandlingSettings.SectionName];
            CustomHandlerData data = (CustomHandlerData)settings.ExceptionPolicies.Get(customPolicy).ExceptionTypes.Get(exceptionType).ExceptionHandlers.Get(customHandler);
            data.Attributes.Add("Money", "0");
            config.Save();

            ConfigurationManager.RefreshSection(ExceptionHandlingSettings.SectionName);
            settings = (ExceptionHandlingSettings)ConfigurationManager.GetSection(ExceptionHandlingSettings.SectionName);
            data = (CustomHandlerData)settings.ExceptionPolicies.Get(customPolicy).ExceptionTypes.Get(exceptionType).ExceptionHandlers.Get(customHandler);

            Assert.IsNotNull(data);
            Assert.AreEqual(3, data.Attributes.Count);
            Assert.AreEqual("0", data.Attributes.Get("Money"));
            data = null;
            config = null;

            // reset
            config = ConfigurationManager.OpenExeConfiguration(ConfigurationUserLevel.None);
            settings = (ExceptionHandlingSettings)config.Sections[ExceptionHandlingSettings.SectionName];
            data = (CustomHandlerData)settings.ExceptionPolicies.Get(customPolicy).ExceptionTypes.Get(exceptionType).ExceptionHandlers.Get(customHandler);
            data.Attributes.Remove("Money");
            config.Save();
            ConfigurationManager.RefreshSection(ExceptionHandlingSettings.SectionName);
            settings = (ExceptionHandlingSettings)ConfigurationManager.GetSection(ExceptionHandlingSettings.SectionName);
            data = (CustomHandlerData)settings.ExceptionPolicies.Get(customPolicy).ExceptionTypes.Get(exceptionType).ExceptionHandlers.Get(customHandler);
            Assert.AreEqual(2, data.Attributes.Count);
        }

        [TestMethod]
        public void CanOpenAndSaveWithWrapHandler()
        {
            // Load config from test config file
            string configPath = Path.Combine(AppContext.BaseDirectory, "Microsoft.Practices.EnterpriseLibrary.ExceptionHandling.Tests.dll.config");
            var configSource = new FileConfigurationSource(configPath);
            var settings = (ExceptionHandlingSettings)configSource.GetSection(ExceptionHandlingSettings.SectionName);

            // Load original handler and backup original name
            var policyData = settings.ExceptionPolicies.Get(wrapPolicy);
            var exceptionTypeData = policyData.ExceptionTypes.Get(exceptionType);
            var handlerData = (WrapHandlerData)exceptionTypeData.ExceptionHandlers.Get(wrapHandler);
            string originalName = handlerData.Name;

            // Rename the handler
            handlerData.Name = newWrapHandler;

            // Clone settings to avoid shared ownership errors
            var clonedSettings = new ExceptionHandlingSettings();
            var clonedPolicy = new ExceptionPolicyData(policyData.Name);
            var clonedExceptionType = new ExceptionTypeData
            {
                Name = exceptionTypeData.Name,
                TypeName = exceptionTypeData.TypeName,
                PostHandlingAction = exceptionTypeData.PostHandlingAction
            };

            foreach (ExceptionHandlerData handler in exceptionTypeData.ExceptionHandlers)
            {
                clonedExceptionType.ExceptionHandlers.Add(handler); // shallow copy; OK for this test
            }

            clonedPolicy.ExceptionTypes.Add(clonedExceptionType);
            clonedSettings.ExceptionPolicies.Add(clonedPolicy);

            // Load modifiable config
            var config = ConfigurationManager.OpenExeConfiguration(ConfigurationUserLevel.None);

            // Remove old section and add cloned one
            if (config.Sections[ExceptionHandlingSettings.SectionName] != null)
            {
                config.Sections.Remove(ExceptionHandlingSettings.SectionName);
            }
            config.Sections.Add(ExceptionHandlingSettings.SectionName, clonedSettings);

            config.Save(ConfigurationSaveMode.Modified);
            ConfigurationManager.RefreshSection(ExceptionHandlingSettings.SectionName);

            // Reload settings from saved config
            var reloadedConfigSource = new FileConfigurationSource(configPath);
            var reloadedSettings = (ExceptionHandlingSettings)reloadedConfigSource.GetSection(ExceptionHandlingSettings.SectionName);
            var reloadedHandler = (WrapHandlerData)reloadedSettings
                .ExceptionPolicies.Get(wrapPolicy)
                .ExceptionTypes.Get(exceptionType)
                .ExceptionHandlers.Get(newWrapHandler);

            // Assertions
            Assert.IsNotNull(reloadedHandler);
            Assert.AreEqual(newWrapHandler, reloadedHandler.Name);

            // Restore original name
            reloadedHandler.Name = originalName;

            var restorePolicy = new ExceptionPolicyData(wrapPolicy);
            var restoreExceptionType = new ExceptionTypeData
            {
                Name = exceptionType,
                TypeName = exceptionTypeData.TypeName,
                PostHandlingAction = exceptionTypeData.PostHandlingAction
            };

            foreach (ExceptionHandlerData handler in exceptionTypeData.ExceptionHandlers)
            {
                restoreExceptionType.ExceptionHandlers.Add(handler);
            }

            restorePolicy.ExceptionTypes.Add(restoreExceptionType);

            var restoredSettings = new ExceptionHandlingSettings();
            restoredSettings.ExceptionPolicies.Add(restorePolicy);

            if (config.Sections[ExceptionHandlingSettings.SectionName] != null)
            {
                config.Sections.Remove(ExceptionHandlingSettings.SectionName);
            }
            config.Sections.Add(ExceptionHandlingSettings.SectionName, restoredSettings);

            config.Save(ConfigurationSaveMode.Modified);
            ConfigurationManager.RefreshSection(ExceptionHandlingSettings.SectionName);
        }
    }
}
