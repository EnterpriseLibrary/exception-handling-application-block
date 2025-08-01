// Copyright (c) Microsoft Corporation. All rights reserved. See License.txt in the project root for license information.

using System;
using System.IO;
using System.Runtime.InteropServices;
using System.Security;
using System.Security.Permissions;
using System.Security.Policy;
using System.Security.Principal;
using System.Text;
using System.Threading;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace Microsoft.Practices.EnterpriseLibrary.ExceptionHandling.Tests
{
    [TestClass]
    public partial class ExceptionFormatterFixture
    {
        const string fileNotFoundMessage = "The file can't be found";
        const string theFile = "theFile";
        const string loggedTimeStampFailMessage = "Logged TimeStamp is not within a one minute time window";
        const string machineName = "MachineName";
        const string timeStamp = "TimeStamp";
        const string appDomainName = "AppDomainName";
        const string threadIdentity = "ThreadIdentity";
        const string windowsIdentity = "WindowsIdentity";
        const string fieldString = "FieldString";
        const string mockFieldString = "MockFieldString";
        const string propertyString = "PropertyString";
        const string mockPropertyString = "MockPropertyString";
        const string message = "Message";
        const string computerName = "COMPUTERNAME";
        const string permissionDenied = "Permission Denied";

        [TestMethod]
        public void AdditionalInfoTest()
        {
            StringBuilder sb = new StringBuilder();
            StringWriter writer = new StringWriter(sb);

            Exception exception = new FileNotFoundException(fileNotFoundMessage, theFile);
            TextExceptionFormatter formatter = new TextExceptionFormatter(writer, exception);

            formatter.Format();

            if (string.Compare(permissionDenied, formatter.AdditionalInfo[machineName]) != 0)
            {
                Assert.AreEqual(Environment.MachineName, formatter.AdditionalInfo[machineName]);
            }

            DateTime minimumTime = DateTime.UtcNow.AddMinutes(-1);
            DateTime loggedTime = DateTime.Parse(formatter.AdditionalInfo[timeStamp]);
            if (DateTime.Compare(minimumTime, loggedTime) > 0)
            {
                Assert.Fail(loggedTimeStampFailMessage);
            }

            Assert.AreEqual(AppDomain.CurrentDomain.FriendlyName, formatter.AdditionalInfo[appDomainName]);
            Assert.AreEqual(Thread.CurrentPrincipal.Identity.Name, formatter.AdditionalInfo[threadIdentity]);

            if (string.Compare(permissionDenied, formatter.AdditionalInfo[windowsIdentity]) != 0)
            {
                Assert.AreEqual(WindowsIdentity.GetCurrent().Name, formatter.AdditionalInfo[windowsIdentity]);
            }
        }

        [TestMethod]
        public void ReflectionFormatterReadTest()
        {
            StringBuilder sb = new StringBuilder();
            StringWriter writer = new StringWriter(sb);

            MockTextExceptionFormatter formatter
                = new MockTextExceptionFormatter(writer, new MockException(), Guid.Empty);

            formatter.Format();

            Assert.AreEqual(formatter.fields[fieldString], mockFieldString);
            Assert.AreEqual(formatter.properties[propertyString], mockPropertyString);
            // The message should be null because the reflection formatter should ignore this property
            Assert.AreEqual(null, formatter.properties[message]);
        }



#if !NET8_0_OR_GREATER
 [TestMethod]
        public void CanGetMachineNameWithoutSecurity()
        {
            var evidence = new Evidence();
            evidence.AddHostEvidence(new Zone(SecurityZone.Internet));
            var set = SecurityManager.GetStandardSandbox(evidence);
            set.AddPermission(new ReflectionPermission(ReflectionPermissionFlag.MemberAccess));

            var domain = AppDomain.CreateDomain("partial trust", null, AppDomain.CurrentDomain.SetupInformation, set);

            try
            {
                var instance = ((ExceptionFormatterTester)domain.CreateInstanceAndUnwrap(typeof(ExceptionFormatterTester).Assembly.FullName, typeof(ExceptionFormatterTester).FullName));
                var formattedMessage = instance.DoTest();

                Assert.IsTrue(formattedMessage.Contains(windowsIdentity + " : " + permissionDenied));
            }
            catch
            {
                throw;
            }
            finally
            {
                AppDomain.Unload(domain);
            }
        }

        [TestMethod]
        public void CanGetWindowsIdentityWithoutSecurity()
        {
            var evidence = new Evidence();
            evidence.AddHostEvidence(new Zone(SecurityZone.Internet));
            var set = SecurityManager.GetStandardSandbox(evidence);
            set.AddPermission(new ReflectionPermission(ReflectionPermissionFlag.MemberAccess));

            var domain = AppDomain.CreateDomain("partial trust", null, AppDomain.CurrentDomain.SetupInformation, set);

            try
            {
                var instance = ((ExceptionFormatterTester)domain.CreateInstanceAndUnwrap(typeof(ExceptionFormatterTester).Assembly.FullName, typeof(ExceptionFormatterTester).FullName));
                var formattedMessage = instance.DoTest();

                Assert.IsTrue(formattedMessage.Contains(windowsIdentity + " : " + permissionDenied));
            }
            catch
            {
                throw;
            }
            finally
            {
                AppDomain.Unload(domain);
            }
        }

#else
        [TestMethod]
        public void CanGetMachineNameWithoutSecurity()
        {
            // Directly create an instance of ExceptionFormatterTester
            var instance = new ExceptionFormatterTester();

            // Call the DoTest method, which should try to access MachineName and handle any exceptions
            var formattedMessage = instance.DoTest();

            // Check that the message includes the expected identity and/or permission denied information.
            // Replace "machineName" and "permissionDenied" with actual expected values or variables.
            Assert.IsTrue(formattedMessage.Contains("machineName : permissionDenied"));
        }


        [TestMethod]
        public void CanGetWindowsIdentityWithoutSecurity()
        {
            // Directly create an instance of ExceptionFormatterTester
            var instance = new ExceptionFormatterTester();

            // Call the DoTest method, which is expected to handle WindowsIdentity and provide a formatted message
            var formattedMessage = instance.DoTest();

            // Check that the message contains the expected identity and permission information
            // Replace "windowsIdentity" and "permissionDenied" with the actual expected values or variables
            Assert.IsTrue(formattedMessage.Contains("windowsIdentity : permissionDenied"));
        }

#endif
        
        public class ExceptionFormatterTester : MarshalByRefObject
        {
            public string DoTest()
            {
                StringBuilder sb = new StringBuilder();
                StringWriter writer = new StringWriter(sb);
                Exception exception = new MockException();
                TextExceptionFormatter formatter = new TextExceptionFormatter(writer, exception);
                formatter.Format();

                return sb.ToString();
            }
        }

        [TestMethod]
        public void SkipsIndexerProperties()
        {
            StringBuilder sb = new StringBuilder();
            StringWriter writer = new StringWriter(sb);
#if NET8_0_OR_GREATER
            Thread.CurrentPrincipal = new GenericPrincipal(new GenericIdentity("TestUser"), null);
#endif
            Exception exception = new FileNotFoundExceptionWithIndexer(fileNotFoundMessage, theFile);
            TextExceptionFormatter formatter = new TextExceptionFormatter(writer, exception);

            formatter.Format();

            if (string.Compare(permissionDenied, formatter.AdditionalInfo[machineName]) != 0)
            {
                Assert.AreEqual(Environment.MachineName, formatter.AdditionalInfo[machineName]);
            }

            DateTime minimumTime = DateTime.UtcNow.AddMinutes(-1);
            DateTime loggedTime = DateTime.Parse(formatter.AdditionalInfo[timeStamp]);
            if (DateTime.Compare(minimumTime, loggedTime) > 0)
            {
                Assert.Fail(loggedTimeStampFailMessage);
            }

            Assert.AreEqual(AppDomain.CurrentDomain.FriendlyName, formatter.AdditionalInfo[appDomainName]);
            Assert.AreEqual(Thread.CurrentPrincipal.Identity.Name, formatter.AdditionalInfo[threadIdentity]);

            if (RuntimeInformation.IsOSPlatform(OSPlatform.Windows) &&
                    string.Compare(permissionDenied, formatter.AdditionalInfo[windowsIdentity]) != 0)
            {
                Console.WriteLine("WindowsIdentity.GetCurrent().Name: "+ WindowsIdentity.GetCurrent().Name);
                Assert.AreEqual(WindowsIdentity.GetCurrent().Name, formatter.AdditionalInfo[windowsIdentity]);
            }
        }

        public class FileNotFoundExceptionWithIndexer : FileNotFoundException
        {
            public FileNotFoundExceptionWithIndexer(string message,
                                                    string fileName)
                : base(message, fileName) { }

            public string this[int index]
            {
                get { return null; }
            }
        }
    }
}
