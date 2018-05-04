﻿// Copyright (c) Microsoft Corporation. All rights reserved. See License.txt in the project root for license information.

using System;
using System.Configuration;
using EnterpriseLibrary.ExceptionHandling.Configuration;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace EnterpriseLibrary.ExceptionHandling.Tests
{
    [TestClass]
    public class ReadWriteConfigurationChangingFixture
    {
        const string fileName = "test.exe.config";

        [TestInitialize]
        public void RemoveExistingSectionFromConfigFile()
        {
            ExeConfigurationFileMap fileMap = new ExeConfigurationFileMap();
            fileMap.ExeConfigFilename = fileName;
            System.Configuration.Configuration config = ConfigurationManager.OpenMappedExeConfiguration(fileMap, ConfigurationUserLevel.None);
            config.Sections.Remove(ExceptionHandlingSettings.SectionName);
            config.Save();
        }

        [TestMethod]
        public void CanSerializeAndDeserializeSettings()
        {
            const string policyName = "policyName";
            const string typeName = "typeName";
            const string handlerName = "handlerName";
            const string handler1Name = "handler1Name";

            ExceptionHandlingSettings settings = new ExceptionHandlingSettings();
            ExceptionPolicyData policyData = new ExceptionPolicyData(policyName);
            ExceptionTypeData typeData = new ExceptionTypeData(typeName, typeof(Exception), PostHandlingAction.None);
            typeData.ExceptionHandlers.Add(new WrapHandlerData(handlerName, "foo", typeof(InvalidCastException).AssemblyQualifiedName));
            typeData.ExceptionHandlers.Add(new ReplaceHandlerData(handler1Name, "foo", typeof(InvalidCastException).AssemblyQualifiedName));
            policyData.ExceptionTypes.Add(typeData);
            settings.ExceptionPolicies.Add(policyData);

            ExeConfigurationFileMap fileMap = new ExeConfigurationFileMap();
            fileMap.ExeConfigFilename = fileName;
            System.Configuration.Configuration config = ConfigurationManager.OpenMappedExeConfiguration(fileMap, ConfigurationUserLevel.None);
            config.Sections.Add(ExceptionHandlingSettings.SectionName, settings);
            config.Save();

            config = ConfigurationManager.OpenMappedExeConfiguration(fileMap, ConfigurationUserLevel.None);
            settings = (ExceptionHandlingSettings)config.Sections[ExceptionHandlingSettings.SectionName];

            Assert.AreEqual(1, settings.ExceptionPolicies.Count);
            Assert.AreEqual(1, settings.ExceptionPolicies.Get(0).ExceptionTypes.Count);
            Assert.AreEqual(2, settings.ExceptionPolicies.Get(0).ExceptionTypes.Get(0).ExceptionHandlers.Count);
        }
    }
}
