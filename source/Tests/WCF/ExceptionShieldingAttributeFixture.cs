﻿// Copyright (c) Microsoft Corporation. All rights reserved. See License.txt in the project root for license information.

using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace EnterpriseLibrary.ExceptionHandling.WCF.Tests
{
    [TestClass]
    public class ExceptionShieldingAttributeFixture
    {
        [TestMethod]
        public void CanCreateInstance()
        {
            ExceptionShieldingAttribute instance = new ExceptionShieldingAttribute();
            Assert.IsNotNull(instance);
            Assert.AreEqual(ExceptionShielding.DefaultExceptionPolicy, instance.ExceptionPolicyName);
        }

        [TestMethod]
        public void CanCreateInstanceWithPolicyName()
        {
            ExceptionShieldingAttribute instance = new ExceptionShieldingAttribute("Policy");
            Assert.AreEqual("Policy", instance.ExceptionPolicyName);
        }

        [TestMethod]
        public void CanAssignExceptionPolicyName()
        {
            ExceptionShieldingAttribute shielding = new ExceptionShieldingAttribute();
            shielding.ExceptionPolicyName = "MyPolicy";
            Assert.AreEqual("MyPolicy", shielding.ExceptionPolicyName);
        }

        [TestMethod]
        public void ShouldGetDefaultValueOnNullExceptionPolicyName()
        {
            ExceptionShieldingAttribute shielding = new ExceptionShieldingAttribute();
            shielding.ExceptionPolicyName = null;
            Assert.AreEqual(ExceptionShielding.DefaultExceptionPolicy, shielding.ExceptionPolicyName);
        }

        [TestMethod]
        public void ShouldGetDefaultValueOnEmptyShieldingAttribute()
        {
            ExceptionShieldingAttribute shielding = new ExceptionShieldingAttribute();
            shielding.ExceptionPolicyName = "";
            Assert.AreEqual(ExceptionShielding.DefaultExceptionPolicy, shielding.ExceptionPolicyName);
        }
    }
}
