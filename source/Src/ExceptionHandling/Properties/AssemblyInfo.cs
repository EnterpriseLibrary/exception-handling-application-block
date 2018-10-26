// Copyright (c) Microsoft Corporation. All rights reserved. See License.txt in the project root for license information.

using System.Runtime.ConstrainedExecution;
using System.Runtime.InteropServices;
using System.Security;
using Microsoft.Practices.EnterpriseLibrary.Common.Configuration.Design;
using Microsoft.Practices.EnterpriseLibrary.ExceptionHandling.Configuration;

[assembly: ReliabilityContract(Consistency.WillNotCorruptState, Cer.None)]
[assembly: SecurityTransparent]

[assembly: ComVisible(false)]

[assembly: HandlesSection(ExceptionHandlingSettings.SectionName)]
[assembly: AddApplicationBlockCommand(
            ExceptionHandlingSettings.SectionName,
            typeof(ExceptionHandlingSettings),
            TitleResourceType = typeof(DesignResources),
            TitleResourceName = "AddExceptionHandlingSettingsCommandTitle",
            CommandModelTypeName = ExceptionHandlingDesignTime.CommandTypeNames.AddExceptionHandlingBlockCommand)]
