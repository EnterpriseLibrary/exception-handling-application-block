// Copyright (c) Microsoft Corporation. All rights reserved. See License.txt in the project root for license information.

using System.Reflection;
using System.Runtime.ConstrainedExecution;
using System.Runtime.InteropServices;
using System.Security;

[assembly: ReliabilityContract(Consistency.WillNotCorruptState, Cer.None)]
[assembly: SecurityTransparent]
[assembly: ComVisible(false)]
